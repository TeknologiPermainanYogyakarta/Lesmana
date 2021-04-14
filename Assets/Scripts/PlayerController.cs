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
    public Animator animator;
    private readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public Bounds Bounds => collider2d.bounds;

    public bool facingRight;

    public float dashTime;
    private float dashTimeTimer;
    public bool dashing;
    public int dashCount;
    public float dashCooldown;
    private float dashCooldownTimer;

    public Transform wallDetectDepan;
    public Transform wallDetectBelakang;

    public LayerMask wallLayer;
    public bool grabWall;
    public bool grabWallDepan;
    public bool grabWallBelakang;

    public bool inWallJump;
    public bool wallJumping;

    public float wallJumpTime;
    public bool wallSlideFlip;

    public GameObject sakti;
    private bool canFlip;

    public void Awake()
    {
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        grabWallDepan = Physics2D.OverlapCircle(wallDetectDepan.position, .2f, wallLayer);
        grabWallBelakang = Physics2D.OverlapCircle(wallDetectBelakang.position, .2f, wallLayer);

        if (controlEnabled)
        {
            move.x = Input.GetAxisRaw("Horizontal");
            if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                jumpState = JumpState.PrepareToJump;
            else if (Input.GetButtonUp("Jump"))
            {
                stopJump = true;
                //Schedule<PlayerStopJump>().player = this;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Attack());
        }
        //else
        //{
        //    move.x = 0;
        //}

        UpdateJumpState();
        Dash();

        WallSlide();
        WallJump();

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
                    animator.SetBool("Jump", true);
                }
                break;

            case JumpState.InFlight:
                if (IsGrounded)
                {
                    grabWall = false;
                    animator.SetBool("Grab Wall", false);

                    //Schedule<PlayerLanded>().player = this;
                    jumpState = JumpState.Landed;
                }
                break;

            case JumpState.Landed:
                jumpState = JumpState.Grounded;
                animator.SetBool("Jump", false);
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

        if (move.x > 0.01f && !facingRight && !wallJumping && !grabWall)
            Flip();
        else if (move.x < -0.01f && facingRight && !wallJumping && !grabWall)
            Flip();

        //animator.SetBool("grounded", IsGrounded);
        //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        //WallJump();
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
                if (!wallJumping)
                {
                    targetVelocity = move * maxSpeed;
                    animator.SetBool("Run", true);
                }
            }
            else
            {
                animator.SetBool("Run", false);
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
        if (Input.GetButtonDown("Dash") && !grabWallBelakang && move.x != 0)
        {
            if (jumpState == JumpState.Grounded && dashCooldownTimer <= 0)
            {
                dashing = true;
                animator.SetBool("Dash", dashing);

                dashTimeTimer = dashTime;
                dashCooldownTimer = dashCooldown;
            }
            if (jumpState == JumpState.InFlight && dashCount == 0)
            {
                dashing = true;
                animator.SetBool("Dash", dashing);

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
                animator.SetBool("Dash", dashing);

                gravityModifier = 2;
            }
        }
    }

    public void WallJump()
    {
        if (wallJumping)
        {
            //if (canFlip == true)
            //{
            //    Flip();

            //    canFlip = false;
            //}
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
            //if (wallJumpFlip)
            //{
            //    //transform.Rotate(0, 180, 0);
            //    wallJumpFlip = false;
            //}
        }
        if (jumpState == JumpState.InFlight && Input.GetButtonDown("Jump") && grabWall)
        {
            //grabWall = false;
            Invoke("GrabWallFalse", 0.2f);

            wallJumping = true;
            inWallJump = true;
            wallJumpTime = 0.5f;

            canFlip = true;
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
        if (jumpState == JumpState.InFlight && grabWallDepan)
        {
            grabWall = true;
            velocity.y = -0.2f;
            Flip();
            //if (!wallSlideFlip)
            //{
            //    Flip();
            //    wallSlideFlip = true;
            //}
            //transform.Rotate(0, 180, 0);
        }

        if (jumpState == JumpState.InFlight && grabWallBelakang)
        {
            //    grabWall = grabWallBelakang;
            //    velocity.x = 0;

            //    //gravityModifier = 0.05f;
            velocity.y = -0.2f;
            animator.SetBool("Grab Wall", true);
        }
        else
        {
            animator.SetBool("Grab Wall", false);
            //Invoke("GrabWallFalse", 1);
            //grabWall = false;
        }
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }

    public IEnumerator Attack()
    {
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Attack", false);
    }

    public void GrabWallFalse()
    {
        grabWall = false;
    }
}