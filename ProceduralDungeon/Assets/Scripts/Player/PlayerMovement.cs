using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    private Vector2 direction;

    private float activeMoveSpeed;
    private bool dash = false;
    [SerializeField]
    private float dashSpeed = 15f, dashLength = 0.5f;
    [SerializeField]
    private float dashCooldown = 3f;
    private float dashCounter, dashCooldownCounter;

    [SerializeField]
    private Rigidbody2D rb;

    private void Start()
    {
        activeMoveSpeed = moveSpeed;
    }

    void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        Move();
        Dash();
    }

    private void GetInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        direction = new Vector2(moveX, moveY).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
            dash = true;

        if (moveX != 0)
            transform.localScale = new Vector3(moveX, 1, 1);
    }

    private void Move()
    {
        rb.velocity = new Vector2(direction.x * activeMoveSpeed, direction.y * activeMoveSpeed);
    }

    private void Dash()
    {
        if (dash)
        {
            if (dashCooldownCounter <= 0 && dashCounter <= 0)
            {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;
            }
        }

        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0)
            {
                dash = false;
                activeMoveSpeed = moveSpeed;
                dashCooldownCounter = dashCooldown;
            }
        }

        if (dashCooldownCounter > 0)
            dashCooldownCounter -= Time.deltaTime;
    }
}
