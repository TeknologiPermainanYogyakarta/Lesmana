using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main class used to implement control of the player.
/// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
/// </summary>
public class PlayerController : KinematicObject
{
    public AudioClip jumpAudio;
    public AudioClip respawnAudio;
    public AudioClip ouchAudio;

    /// <summary>
    /// Max horizontal speed of the player.
    /// </summary>
    public float maxSpeed = 7;

    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
    public float jumpTakeOffSpeed = 7;

    public JumpState jumpState = JumpState.Grounded;
    private bool stopJump;
    /*internal new*/
    public Collider2D collider2d;
    /*internal new*/
    public AudioSource audioSource;
    public Health health;
    public bool controlEnabled = true;

    public bool jump;
    private Vector2 move;
    private SpriteRenderer spriteRenderer;
    internal Animator animator;
    private readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public Bounds Bounds => collider2d.bounds;

    public bool facingRight;

    public float dashTime;
    private float dashTimeTimer;
    public bool dashing;
    public int dashCount;
    public float dashCooldown;
    private float dashCooldownTimer;

    public Transform wallDetect;
    public LayerMask wallLayer;
    public bool grabWall;

    public bool inWallJump;
    public bool wallJumping;

    public float wallJumpTime;

    private void Awake()
    {
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        grabWall = Physics2D.OverlapCircle(wallDetect.position, .2f, wallLayer);
        if (controlEnabled)
        {
            move.x = Input.GetAxis("Horizontal");
            if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                jumpState = JumpState.PrepareToJump;
            else if (Input.GetButtonUp("Jump"))
            {
                stopJump = true;
                //Schedule<PlayerStopJump>().player = this;
            }
        }
        //else
        //{
        //    move.x = 0;
        //}

        UpdateJumpState();
        Dash();

        WallSlide();
        base.Update();
    }

    private void UpdateJumpState()
    {
        jump = false;
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                jump = true;
                stopJump = false;
                break;

            case JumpState.Jumping:
                if (!IsGrounded)
                {
                    //Schedule<PlayerJumped>().player = this;
                    jumpState = JumpState.InFlight;
                }
                break;

            case JumpState.InFlight:
                if (IsGrounded)
                {
                    //Schedule<PlayerLanded>().player = this;
                    jumpState = JumpState.Landed;
                }
                break;

            case JumpState.Landed:
                jumpState = JumpState.Grounded;
                break;
        }
    }

    protected override void ComputeVelocity()
    {
        if (jump && IsGrounded)
        {
            velocity.y = jumpTakeOffSpeed * model.jumpModifier;
            jump = false;
        }
        if (stopJump)
        {
            stopJump = false;
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * model.jumpDeceleration;
            }
        }

        if (move.x > 0.01f && !facingRight)
            Flip();
        else if (move.x < -0.01f && facingRight)
            Flip();

        //animator.SetBool("grounded", IsGrounded);
        //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        WallJump(facingRight);
        if (dashing)
        {
            if (facingRight)
            {
                targetVelocity.x = 50;
            }
            if (!facingRight)
            {
                targetVelocity.x = -50;
            }
            gravityModifier = -1.5f;
        }
        else if (!dashing && !wallJumping)
        {
            if (move.x != 0)
            {
                targetVelocity = move * maxSpeed;
            }
            else
            {
                if (inWallJump)
                {
                    targetVelocity = move * 1;
                }
            }
        }
    }

    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Landed
    }

    public void Dash()
    {
        //if (!dashing)
        //{
        //    dashCooldownTimer -= Time.deltaTime;
        //    dashTime = 0.1f;
        //    gravityModifier = 2;
        //}
        //if (/*jumpState == JumpState.InFlight &&*/ Input.GetButtonDown("Dash") && !grabWall && move.x != 0 /*&& dashCount == 0 */&& dashCooldownTimer <= 0)
        //{
        //    //dashCooldownTimer = dashCooldown;
        //    dashing = true;
        //    //dashCount += 1;
        //}
        //if (IsGrounded)
        //{
        //    //dashCount = 0;
        //}
        //else
        //{
        //    dashTime -= Time.deltaTime;
        //    if (dashTime <= 0)
        //    {
        //        dashing = false;
        //    }
        //}
        if (IsGrounded)
        {
            dashCount = 0;
        }
        if (Input.GetButtonDown("Dash") && !grabWall && move.x != 0)
        {
            if (jumpState == JumpState.Grounded && dashCooldownTimer <= 0)
            {
                dashing = true;
                dashTimeTimer = dashTime;
                dashCooldownTimer = dashCooldown;
            }
            if (jumpState == JumpState.InFlight && dashCount == 0)
            {
                dashing = true;
                dashCount += 1;
                dashTimeTimer = dashTime;
            }
        }
        else
        {
            dashTimeTimer -= Time.deltaTime;
            dashCooldownTimer -= Time.deltaTime;
            if (dashTimeTimer <= 0)
            {
                dashing = false;
                gravityModifier = 2;
            }
        }
    }

    public void WallJump(bool facingRight)
    {
        if (wallJumping)
        {
            if (facingRight)
            {
                targetVelocity.x = 5;
                velocity.y = 5;
            }
            else
            {
                targetVelocity.x = -5;
                velocity.y = 5;
            }
        }
        if (jumpState == JumpState.InFlight && Input.GetButtonDown("Jump") && grabWall)
        {
            wallJumping = true;
            inWallJump = true;
            wallJumpTime = 0.5f;
        }
        else
        {
            wallJumpTime -= Time.deltaTime;
            if (wallJumpTime <= 0)
            {
                wallJumping = false;
            }
        }
    }

    public void WallSlide()
    {
        if (jumpState == JumpState.InFlight && grabWall)
        {
            //gravityModifier = 0.05f;
            velocity.y = -0.2f;
        }
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }
}