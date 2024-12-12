using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NhoxMonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] BoxCollider2D box2d;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 3.7f;
    [SerializeField] float wallJumpForce = 10f;

    [Header("Dash Settings")]
    protected bool isDashing;
    [SerializeField] protected float dashTime;
    [SerializeField] protected float dashSpeed = 10f;
    [SerializeField] protected float distanceBwImages;
    [SerializeField] protected float dashCoolDown;
    [SerializeField] protected float dashTimeLeft;
    [SerializeField] protected float lastImageXpos;
    [SerializeField] protected float lastDash = -100f;

    [Header("Detection Settings")]
    [SerializeField] float raycastDistance = 0.05f;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;
    bool isFacingRight = true;
    bool isWallSliding = false;  

    [SerializeField] protected PlayerState stateManager = PlayerState.Idle;
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
        HandleMovement();
        UpdateState();
    }

    #region Ground and Wall Check
    void CheckGroundedAndWalls()
    {
        isGrounded = PerformGroundCheck();
        if (!isGrounded)
        {
            CheckWalls();
        }
        else
        {
            isWallSliding = false;
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

        // Kiểm tra phạm vi từ trên xuống dưới BoxCollider2D để đảm bảo đúng tường
        Vector2 wallCheckSize = new Vector2(raycastDistance, box2d.bounds.size.y);

        // Kiểm tra tường ở bên trái và bên phải
        Collider2D leftWallHit = Physics2D.OverlapBox(leftCheck, wallCheckSize, 0f, groundLayer);
        Collider2D rightWallHit = Physics2D.OverlapBox(rightCheck, wallCheckSize, 0f, groundLayer);

        // Nếu có tường ở bên trái hoặc bên phải và không chạm đất, thì trượt tường
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
    #endregion

    void HandleMovement()
    {
        if (isDashing) return;
        HandleHorizontalMovement();
        HandleJump();
    }

    void HandleHorizontalMovement()
    {
        if (isWallSliding)
        {
            // Giảm tốc độ di chuyển ngang khi đang trượt tường
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y); 
        }
        else
        {
            float horizontalInput = InputManager.Instance.Direction.y - InputManager.Instance.Direction.x;
            if (horizontalInput != 0)
            {
                if ((horizontalInput > 0) != isFacingRight) Flip();
                rb2d.velocity = new Vector2(horizontalInput * moveSpeed, rb2d.velocity.y);
            }
            else
            {
                rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
            }
        }
    }

    void HandleJump()
    {
        if (InputManager.Instance.Direction.z > 0 && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            stateManager = PlayerState.Jump;
        }
        else if (InputManager.Instance.Direction.z > 0 && isWallSliding)
        {
            float wallJumpDirection = isFacingRight ? -1f : 1f;
            rb2d.velocity = new Vector2(wallJumpDirection * wallJumpForce, jumpSpeed);
            isWallSliding = false;
            stateManager = PlayerState.WallJumping;
        }
    }

    protected virtual void HandleDash()
    {
        if(InputManager.Instance.DashInput && Time.time >= (lastDash + dashCoolDown))
        {
            if (Time.time >= (lastDash + dashCoolDown)) 
                AttemptToDash();
        }
    }

    protected virtual void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        stateManager = PlayerState.Dash;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    protected virtual void CheckDash()
    {
        if (isDashing)
        {
            // Tiến hành Dash
            if (dashTimeLeft > 0)
            {
                rb2d.velocity = new Vector2(isFacingRight ? dashSpeed : -dashSpeed, 0f);
                dashTimeLeft -= Time.fixedDeltaTime;

                // Tạo AfterImage khi di chuyển
                if (Mathf.Abs(transform.parent.position.x - lastImageXpos) > distanceBwImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.parent.position.x;
                }
            }
            else
            {
                isDashing = false;
            }
        }
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
            stateManager = PlayerState.Dash; // Giữ trạng thái Dash
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
