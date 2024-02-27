using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Jumping mechanic can be improved with jumpheight based on holding space.

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D playerBoxCollider;
    public Animator animator;
    public LayerMask groundLayer;

    public float jumpForce = 5f;
    public float playerSpeed = 10f;

    public float startAcceleration = 5f;
    public float stopAcceleration = 7.5f;

    public float risingGravity = 1f;
    public float fallingGravity = 2f;

    float moveDirection;
    float currentVelocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");
        isGrounded = IsGrounded();
        Jump();
        AnimationController();

        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = risingGravity;
        }
        else
        {
            rb.gravityScale = fallingGravity;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void AnimationController()
    {
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(moveDirection));
        if (isGrounded)
        {
            animator.SetBool("isGrounded", true);
        }
        else
        {
            if (rb.velocity.y >= 0)
            {
                animator.SetBool("isGrounded", false);
                animator.SetFloat("VerticalSpeed", 1);
            }
            else
            {
                animator.SetBool("isGrounded", false);
                animator.SetFloat("VerticalSpeed", -1);
            }
        }
    }

    void Move()
    {
        float targetSpeed = moveDirection * playerSpeed;

        if (moveDirection < 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, startAcceleration * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (moveDirection > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, startAcceleration * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0f, stopAcceleration * Time.fixedDeltaTime);
        }

        rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    bool IsGrounded()
    {
        float raycastLength = 0.02f;
        float edgeSpare = 0.0f;

        Vector2 raycastOriginLeft = new Vector2(playerBoxCollider.bounds.min.x - edgeSpare, playerBoxCollider.bounds.min.y);
        Vector2 raycastOriginRight = new Vector2(playerBoxCollider.bounds.max.x + edgeSpare, playerBoxCollider.bounds.min.y);

        RaycastHit2D raycastHitLeft = Physics2D.Raycast(raycastOriginLeft, Vector2.down, raycastLength, groundLayer);
        RaycastHit2D raycastHitRight = Physics2D.Raycast(raycastOriginRight, Vector2.down, raycastLength, groundLayer);

        Color rayColorLeft = raycastHitLeft.collider != null ? Color.green : Color.red;
        Color rayColorRight = raycastHitRight.collider != null ? Color.green : Color.red;

        Debug.DrawRay(raycastOriginLeft, Vector2.down * raycastLength, rayColorLeft);
        Debug.DrawRay(raycastOriginRight, Vector2.down * raycastLength, rayColorRight);

        return raycastHitLeft.collider != null || raycastHitRight.collider != null;
    }
}
