﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements game physics for some in game entity.
/// </summary>
public class KinematicObject : MonoBehaviour
{
    public float minGroundNormalY = .65f;

    public float gravityModifier;

    public Vector2 velocity;

    public bool IsGrounded { get; private set; }

    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    protected virtual void OnEnable()
    {
        body = GetComponent<Rigidbody2D>();
        body.isKinematic = true;
    }

    protected virtual void OnDisable()
    {
        body.isKinematic = false;
    }

    protected virtual void Start()
    {
        gravityModifier = 2;
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    protected virtual void Update()
    {
        //targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {
    }

    protected virtual void FixedUpdate()
    {
        //if already falling, fall faster than the jump speed, otherwise use normal gravity.
        if (velocity.y < 0)
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        else
            velocity += Physics2D.gravity * Time.deltaTime;

        velocity.x = targetVelocity.x;

        IsGrounded = false;

        var deltaPosition = velocity * Time.deltaTime;

        var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        var move = moveAlongGround * deltaPosition.x;

        PerformMovement(move, false);

        move = Vector2.up * deltaPosition.y;

        PerformMovement(move, true);
    }

    private void PerformMovement(Vector2 move, bool yMovement)
    {
        var distance = move.magnitude;
        if (distance > minMoveDistance)
        {
            //check if we hit anything in current direction of travel
            var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            for (var i = 0; i < count; i++)
            {
                var currentNormal = hitBuffer[i].normal;
                //is this surface flat enough to land on?
                if (currentNormal.y > minGroundNormalY)
                {
                    IsGrounded = true;
                    // if moving up, change the groundNormal to new surface normal.
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                if (IsGrounded)
                {
                    targetVelocity = Vector2.zero;

                    //how much of our velocity aligns with surface normal?
                    var projection = Vector2.Dot(velocity, currentNormal);
                    if (projection < 0)
                    {
                        //slower velocity if moving against the normal (up a hill).
                        velocity = velocity - projection * currentNormal;
                    }
                }
                else
                {
                    //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                    velocity.x *= 0;
                    velocity.y = Mathf.Min(velocity.y, 0);
                }
                //remove shellDistance from actual move distance.
                var modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        body.position = body.position + move.normalized * distance;
    }
}