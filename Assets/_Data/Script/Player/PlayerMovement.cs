using UnityEngine;

public class PlayerMovement : NhoxMonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private BoxCollider2D box2d;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 100f; // Higher for snappier response
    [SerializeField] private float deceleration = 100f; // Higher for quick stops
    [SerializeField] private float airAcceleration = 60f; // Slightly lower in air
    [SerializeField] private float airDeceleration = 60f;
    [SerializeField] private float maxFallSpeed = 15f; // Terminal velocity

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f; // Higher initial jump
    [SerializeField] private float shortJumpMultiplier = 2f; // For variable jump height
    [SerializeField] private float fallMultiplier = 2.5f; // Faster falling
    [SerializeField] private float coyoteTime = 0.1f; // Short coyote time like Megaman
    [SerializeField] private int maxAirJumps = 0; // No double jump by default
    private int remainingAirJumps;

    [Header("Wall Movement")]
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float wallJumpForce = 12f;
    [SerializeField] private float wallJumpTime = 0.2f;
    private float wallJumpTimeLeft;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCoolDown = 0.5f;
    [SerializeField] private float distanceBwImages = 0.1f;

    [Header("Detection Settings")]
    [SerializeField] private float raycastDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    // State tracking
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWallSliding;
    private bool isDashing;
    private bool isJumping;
    private bool wasJumpPressed;
    private bool isWallJumping;

    // Timers
    private float lastGroundedTime;
    private float dashTimeLeft;
    private float lastDash = -100f;
    private float lastImageXpos;

    private PlayerState stateManager = PlayerState.Idle;
    public PlayerState StateManager => stateManager;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        LoadRigidbody2D();
        LoadBoxCollider2D();
        LoadLayerMask();
    }

    private void FixedUpdate()
    {
        HandleDash();
        CheckDash();
        CheckGroundedAndWalls();

        if (!isDashing)
        {
            ApplyMovement();
            ApplyWallSliding();
            HandleJump();
            ApplyFallMultiplier();
        }

        ClampFallSpeed();
        UpdateState();
    }

    private void ApplyMovement()
    {
        if (isWallJumping)
        {
            wallJumpTimeLeft -= Time.fixedDeltaTime;
            if (wallJumpTimeLeft <= 0)
            {
                isWallJumping = false;
            }
            return;
        }

        float horizontalInput = InputManager.Instance.Direction.y - InputManager.Instance.Direction.x;

        // Use different acceleration values for ground and air
        float currentAccel = isGrounded ? acceleration : airAcceleration;
        float currentDecel = isGrounded ? deceleration : airDeceleration;

        // Calculate target speed
        float targetSpeed = horizontalInput * moveSpeed;

        // Calculate speed difference
        float speedDiff = targetSpeed - rb2d.velocity.x;

        // Calculate acceleration rate
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? currentAccel : currentDecel;

        // Apply movement force
        float movement = speedDiff * accelRate;
        rb2d.AddForce(movement * Vector2.right);

        // Instant velocity correction for snappy movement
        if (Mathf.Abs(rb2d.velocity.x) > moveSpeed)
        {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * moveSpeed, rb2d.velocity.y);
        }

        // Handle direction facing
        if (horizontalInput != 0 && (horizontalInput > 0) != isFacingRight)
        {
            Flip();
        }
    }

    private void HandleJump()
    {
        bool jumpInput = InputManager.Instance.Direction.z > 0;

        // Start jump
        if (jumpInput && !wasJumpPressed)
        {
            if (isGrounded || lastGroundedTime > 0)
            {
                ExecuteJump(jumpForce);
            }
            else if (remainingAirJumps > 0)
            {
                ExecuteJump(jumpForce * 0.8f); // Slightly weaker air jump
                remainingAirJumps--;
            }
            else if (isWallSliding)
            {
                ExecuteWallJump();
            }
        }

        // Variable jump height
        if (!jumpInput && rb2d.velocity.y > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / shortJumpMultiplier);
        }

        wasJumpPressed = jumpInput;
    }

    private void ExecuteJump(float force)
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, force);
        isJumping = true;
        lastGroundedTime = 0;
        stateManager = PlayerState.Jump;
    }

    private void ExecuteWallJump()
    {
        isWallSliding = false;
        isWallJumping = true;
        wallJumpTimeLeft = wallJumpTime;

        float wallJumpDirection = isFacingRight ? -1f : 1f;
        rb2d.velocity = new Vector2(wallJumpDirection * wallJumpForce, jumpForce);

        Flip(); // Always flip when wall jumping
        stateManager = PlayerState.WallJumping;
    }

    private void ApplyWallSliding()
    {
        if (isWallSliding && rb2d.velocity.y < -wallSlideSpeed)
        {
            rb2d.velocity = new Vector2(0, -wallSlideSpeed);
        }
    }

    private void ApplyFallMultiplier()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void ClampFallSpeed()
    {
        if (rb2d.velocity.y < -maxFallSpeed)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, -maxFallSpeed);
        }
    }

    protected virtual void HandleDash()
    {
        if (InputManager.Instance.DashInput && Time.time >= (lastDash + dashCoolDown))
        {
            AttemptToDash();
        }
    }

    protected virtual void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        rb2d.gravityScale = 0;
        stateManager = PlayerState.Dash;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    protected virtual void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                rb2d.velocity = new Vector2(isFacingRight ? dashSpeed : -dashSpeed, 0f);
                dashTimeLeft -= Time.fixedDeltaTime;

                if (Mathf.Abs(transform.parent.position.x - lastImageXpos) > distanceBwImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.parent.position.x;
                }
            }
            else
            {
                isDashing = false;
                rb2d.gravityScale = 1;
                rb2d.velocity = new Vector2(rb2d.velocity.x * 0.5f, rb2d.velocity.y); // Smooth dash exit
            }
        }
    }

    private void CheckGroundedAndWalls()
    {
        bool wasGrounded = isGrounded;
        isGrounded = PerformGroundCheck();

        if (isGrounded)
        {
            lastGroundedTime = coyoteTime;
            isJumping = false;
            remainingAirJumps = maxAirJumps;

            if (!wasGrounded) // Landing
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x * 0.5f, rb2d.velocity.y); // Landing friction
            }
        }
        else
        {
            lastGroundedTime -= Time.fixedDeltaTime;
            CheckWalls();
        }
    }


    bool PerformGroundCheck()
    {
        Vector3 boxOrigin = GetGroundCheckBoxOrigin();
        Vector3 boxSize = GetGroundCheckBoxSize();

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, raycastDistance, groundLayer);
        DebugGroundCheck(boxOrigin, boxSize, raycastHit.collider != null);

        return raycastHit.collider != null;
    }

    void CheckWalls()
    {
        Vector2 leftCheck = new Vector2(box2d.bounds.min.x, box2d.bounds.center.y);
        Vector2 rightCheck = new Vector2(box2d.bounds.max.x, box2d.bounds.center.y);
        Vector2 wallCheckSize = new Vector2(raycastDistance, box2d.bounds.size.y);

        Collider2D leftWallHit = Physics2D.OverlapBox(leftCheck, wallCheckSize, 0f, groundLayer);
        Collider2D rightWallHit = Physics2D.OverlapBox(rightCheck, wallCheckSize, 0f, groundLayer);

        if ((leftWallHit != null || rightWallHit != null) && !isGrounded)
        {
            isWallSliding = true;
            stateManager = PlayerState.WallSliding;
        }
        else
        {
            isWallSliding = false;
        }
    }

    Vector3 GetGroundCheckBoxOrigin()
    {
        return new Vector3(box2d.bounds.center.x, box2d.bounds.min.y + (box2d.bounds.extents.y / 4f));
    }

    Vector3 GetGroundCheckBoxSize()
    {
        return new Vector3(box2d.bounds.size.x, box2d.bounds.size.y / 4f);
    }

    void DebugGroundCheck(Vector3 origin, Vector3 size, bool isHit)
    {
        Color raycastColor = isHit ? Color.green : Color.red;
        Debug.DrawRay(origin + new Vector3(size.x / 2, 0), Vector2.down * (size.y / 2 + raycastDistance), raycastColor);
        Debug.DrawRay(origin - new Vector3(size.x / 2, 0), Vector2.down * (size.y / 2 + raycastDistance), raycastColor);
        Debug.DrawRay(origin - new Vector3(size.x / 2, size.y / 2 + raycastDistance), Vector2.right * size.x, raycastColor);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.parent.Rotate(0f, 180f, 0f);
    }

    void UpdateState()
    {
        if (isDashing)
        {
            stateManager = PlayerState.Dash;
            return;
        }

        if (stateManager == PlayerState.WallJumping)
        {
            return;
        }

        if (isGrounded)
        {
            if (Mathf.Abs(rb2d.velocity.x) > 0.1f)
            {
                stateManager = PlayerState.Run;
            }
            else
            {
                stateManager = PlayerState.Idle;
            }
        }
        else
        {
            if (rb2d.velocity.y > 0)
            {
                stateManager = PlayerState.Jump;
            }
            else if (rb2d.velocity.y < 0)
            {
                stateManager = PlayerState.Fall;
            }

            if (isWallSliding)
            {
                stateManager = PlayerState.WallSliding;
            }
        }
    }

    protected virtual void LoadRigidbody2D()
    {
        if (rb2d != null) return;
        rb2d = GetComponentInParent<Rigidbody2D>();
        Debug.Log(transform.name + " Load Rigidbody2D", gameObject);
    }

    protected virtual void LoadBoxCollider2D()
    {
        if (box2d != null) return;
        box2d = GetComponentInParent<BoxCollider2D>();
        Debug.Log(transform.name + " Load BoxCollider2D", gameObject);
    }

    protected virtual void LoadLayerMask()
    {
        if (groundLayer != 0) return;
        groundLayer = LayerMask.GetMask("Ground");
        Debug.Log(transform.name + " Load LayerMask", gameObject);
    }
}
