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

    [Header("Ground Detection")]
    [SerializeField] float raycastDistance = 0.05f;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;
    bool isFacingRight = true;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        LoadRigidbody2D();
        LoadBoxCollider2D();
        LoadLayerMask();
    }

    private void FixedUpdate()
    {
        Movement();
        CheckGrounded();
    }

    void CheckGrounded()
    {
        isGrounded = false;
        Vector3 boxOrigin = box2d.bounds.center;
        boxOrigin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 boxSize = box2d.bounds.size;
        boxSize.y = box2d.bounds.size.y / 4f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, raycastDistance, groundLayer);
        isGrounded = raycastHit.collider != null;

        // Debug lines
        Color raycastColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(boxOrigin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(boxOrigin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(boxOrigin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
    }

    void Movement()
    {
        float horizontalInput = InputManager.Instance.Direction.y - InputManager.Instance.Direction.x;
        bool jumpInput = InputManager.Instance.Direction.z > 0;

        if (horizontalInput != 0)
        {
            if ((horizontalInput > 0) != isFacingRight) Flip();
            rb2d.velocity = new Vector2(horizontalInput * moveSpeed, rb2d.velocity.y);
        }
        else rb2d.velocity = new Vector2(0f, rb2d.velocity.y);

        if (jumpInput && isGrounded)
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.parent.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    protected virtual void LoadRigidbody2D()
    {
        if (rb2d != null) return;
        rb2d = GetComponentInParent<Rigidbody2D>();
        Debug.Log(transform.name + "Load Rigidbody2D", gameObject);
    }

    protected virtual void LoadBoxCollider2D()
    {
        if (box2d != null) return;
        box2d = GetComponentInParent<BoxCollider2D>();
        Debug.Log(transform.name + "Load BoxCollider2D", gameObject);
    }

    protected virtual void LoadLayerMask()
    {
        if (groundLayer != 0) return;
        groundLayer = LayerMask.GetMask("Ground");
        Debug.Log(transform.name + "Load LayerMask", gameObject);
    }
}
