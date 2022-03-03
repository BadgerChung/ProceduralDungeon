using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    private Vector2 direction;

    [SerializeField]
    private Rigidbody2D rb;

    RaycastHit2D raycast;

    [SerializeField]
    private LayerMask dashLayerMask;

    private bool dash;
    [SerializeField]
    private float dashLength = 10f;
    private Vector2 dashPosition;
    private float dashCooldown;

    private void Start()
    {
        GameManager.instance.player = gameObject;
        rb = GetComponent<Rigidbody2D>();
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

        float cursorX = Input.mousePosition.x;
        if (cursorX > Screen.width / 2) cursorX = 1;
        else cursorX = -1;

        direction = new Vector2(moveX, moveY).normalized;

        transform.localScale = new Vector3(cursorX, 1, 1);

        if (Input.GetKeyDown(KeyCode.Space))
            dash = true;
    }

    private void Move()
    {
        rb.velocity = direction * moveSpeed;
    }

    private void Dash()
    {
        dashCooldown -= Time.deltaTime;
        if (dash && dashCooldown < 0)
        {
            dashCooldown = 1f;
            dashPosition = (Vector2)transform.position + direction * dashLength;

            raycast = Physics2D.Raycast(transform.position, direction, dashLength, dashLayerMask);
            if (raycast.collider != null)
                dashPosition = raycast.point;

            rb.MovePosition(dashPosition);
        }
        dash = false;
    }
}