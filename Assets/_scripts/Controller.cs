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

    private GameObject[] Arrows;

    public int CurrentPowerUp;

    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public float rememberGroundedFor;
    float lastTimeGrounded;

    bool onWall = false;
    public Transform onWallChecker;
    public float CheckWallRadius;
    public float rememberWallFor;
    float lastTimeOnWall;

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
        CheckIfGrounded();
        toggleArrows();
        if (CurrentPowerUp == 1 || CurrentPowerUp == 4)
        {
            Jump();
            if(CurrentPowerUp == 4)
            {
                WallJump();
            }

        }
        if (CurrentPowerUp == 3)
        {
            ShootGrapple();
        }
        else
        {
            Move();
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

    void WallJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (onWall || Time.time - lastTimeOnWall <= rememberWallFor))
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
            speed = 5;
        }
        if (other.gameObject.CompareTag("KeyPowerUp"))
        {
            SetPowerUp(2);
            speed = 5;
        }
        if (other.gameObject.CompareTag("GrapplePowerUp"))
        {
            SetPowerUp(3);
            speed = 35;
        }
        if (other.gameObject.CompareTag("WallJumpPowerUp"))
        {
            SetPowerUp(4);
            speed = 5;
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
                if(g.gameObject.GetComponent<Renderer>().enabled == false)
                {
                    g.gameObject.GetComponent<Renderer>().enabled = true;
                    g.gameObject.transform.position = doorLocation + offset;
                }
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

    void CheckIfOnWall()
    {
        Collider2D collider = Physics2D.OverlapCircle(onWallChecker.position, CheckWallRadius, groundLayer);
        if (collider != null)
        {
            onWall = true;
        }
        else
        {
            if (onWall)
            {
                lastTimeOnWall = Time.time;
            }
            onWall = false;
        }
    }

    void ShootGrapple()
    {
        Vector2 startingPostion = rb.position;
        string direction = null;
        if (rb.velocity.x == 0 && rb.velocity.y == 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction = "Vertical";
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                direction = "Vertical";
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = "Horizontal";
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = "Horizontal";
            }
        }

        if (direction == "Horizontal")
        {
            float x = Input.GetAxisRaw("Horizontal");
            float moveBy = x * speed;
            rb.velocity = new Vector2(moveBy, rb.velocity.y);
        }
        if (direction == "Vertical")
        {
            float y = Input.GetAxisRaw(direction);
            float moveBy = y * speed;
            rb.velocity = new Vector2(rb.velocity.x, moveBy);
        }

    }


    //adjust with direction and confirmation
    void toggleArrows()
    {
        bool available;
        if(CurrentPowerUp != 3)
        {
            available = false;
            rb.gravityScale = 1;
        }
        else
        {
            available = true;
            rb.gravityScale = 0;
        }
        Arrows = GameObject.FindGameObjectsWithTag("grappleArrow");
        foreach (GameObject g in Arrows)
        {
            g.gameObject.GetComponent<Renderer>().enabled = available;
        }
    }

    //Number is used to indicate what powerup is currently available
    void SetPowerUp(int PowerUp)
    {
        CurrentPowerUp = PowerUp;
    }
}
