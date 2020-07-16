using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Controller : MonoBehaviour
{


    Rigidbody2D rb;
    public float speed;

    public Vector2 initalPosition;
    private GameObject[] Doors;

    public float jumpForce;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;


    public int CurrentPowerUp;

    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public float rememberGroundedFor;
    float lastTimeGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initalPosition = rb.position;
        CurrentPowerUp = 0;
        speed = 5;
        jumpForce = 10;

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckIfGrounded();
        if (CurrentPowerUp == 1)
        {
            Jump();
        }
            
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float moveBy = x * speed;
        rb.velocity = new Vector2(moveBy, rb.velocity.y);
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 doorLocation;
        Vector2 offset;
        offset.x = 0;
        offset.y = -50;

        if (other.gameObject.CompareTag("JumpPowerUp"))
        {
            SetPowerUp(1);
        }
        if (other.gameObject.CompareTag("KeyPowerUp"))
        {
            SetPowerUp(2);
        }
        if (other.gameObject.CompareTag("Door") && CurrentPowerUp == 2)
        {
            doorLocation = other.gameObject.transform.position;
            other.gameObject.GetComponent<Renderer>().enabled = false;
            other.gameObject.transform.position = doorLocation - offset;
        }
        if (other.gameObject.CompareTag("KillBox"))
        {
            rb.position = initalPosition;
            Doors = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject g in Doors)
            {
                doorLocation = g.gameObject.transform.position;
                g.gameObject.GetComponent<Renderer>().enabled = true;
                g.gameObject.transform.position = doorLocation + offset;
            }
            SetPowerUp(0);
        }


    }

    void CheckIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);
        if (collider != null)
        {
            isGrounded = true;
        }
        else
        {
            if(isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

    //Number is used to indicate what powerup is currently available
    void SetPowerUp(int PowerUp)
    {
        CurrentPowerUp = PowerUp;
    }
}
