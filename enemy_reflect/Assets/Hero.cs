using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private AudioSource jumpy;
    [SerializeField] private AudioSource walking;
    [SerializeField] private AudioSource jumping;

    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        realSpeed = speed;
        WallcheckRadius = WallCheck.GetComponent<CircleCollider2D>().radius;
        gravityDef = rb.gravityScale;
    }


    private void Update()
    {
        walk();
        Run();
        Reflect();
        
        CheckingGround();
        Lunge();
        //sit();
        CheckingWall();
        MoveOnWall();
        WallJump();

        
    }

    private void FixedUpdate()
    {
        //Jump();
        LiteJump();
        jumpMove();
    }


    public Vector2 moveVector;
    public int speed = 4;
    public int fastSpeed = 7;
    private int realSpeed;
    public bool faceRight = true;

    bool blockForJump = false;
    void walk()
    {
        //moveVector.x = Input.GetAxis("Horizontal");
        if (!blockForJump)
        {
            moveVector.x = Input.GetAxisRaw("Horizontal"); // Значение будет изменяться не плавно (0.1 - 0.2 - ... - 1), а моментально (0 - 1)
        }
        rb.velocity = new Vector2(moveVector.x * realSpeed, rb.velocity.y);

        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
    }


    void Reflect()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
    }


    //public float jumpForce = 620;
    public bool jumpControl;
    public float jumpTime = 0;
    private int jumpCount = 0;
    public float doubleJumpVelocity = 10f;
    public int maxJumpValue = 1;
    public float jumpControlTime = 0.7f;
    bool PlatformDownProcess;
    void Jump()
    {
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            Physics2D.IgnoreLayerCollision(9, 12, true);
            PlatformDownProcess = true;
            Invoke("IgnoreLayerOff", 0.5f);
        }
        else if (!PlatformDownProcess)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (onGround) { jumpControl = true; anim.Play("jump"); }
            }
            else { jumpControl = false; }


            if (jumpControl)
            {
                if ((jumpTime += Time.fixedDeltaTime) < jumpControlTime)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
            else { jumpTime = 0; }


            if (Input.GetKeyDown(KeyCode.Space) && !onGround && !onWall)
            {
                if (jumpCount < maxJumpValue)
                {
                    rb.velocity = new Vector2(rb.velocity.x, doubleJumpVelocity); anim.Play("doubleJump");
                    jumpCount++;
                }
            }
        }

        if (onGround || onWall) { jumpCount = 0; }

        if (rb.velocity.y == 0) { anim.SetBool("zeroVelocityY", true); }
        else { anim.SetBool("zeroVelocityY", false); }
    }


    public int jumpForce = 10;
    void LiteJump()
    {
        if (onGround && Input.GetKeyDown(KeyCode.Space))
        {
            blockForJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void autoLiteJump()
    {
        //blockForJump = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((Ground.value & 1 << collision.gameObject.layer) != 0) { blockForJump = false; }
    }


    void IgnoreLayerOff()
    {
        Physics2D.IgnoreLayerCollision(9, 12, false);
        PlatformDownProcess = false;
    }


    public bool onGround;
    public LayerMask Ground;
    public Transform GroundCheck;
    public float GroundcheckRadius;
    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, GroundcheckRadius, Ground);
        anim.SetBool("onGround", onGround);
    }


    public int lungeimpulse = 5000;
    void Lunge()
    {
        if (Input.GetKeyDown(KeyCode.K) && !lockLunge)
        {
            lockLunge = true;
            Invoke("LungeLock", 2.5f);
            anim.StopPlayback();
            anim.Play("lunge");

            rb.velocity = new Vector2(0, 0);

            if (!faceRight) { rb.AddForce(Vector2.left * lungeimpulse); }
            else { rb.AddForce(Vector2.right * lungeimpulse); }
        }
    }


    private bool lockLunge = false;
    void LungeLock()
    {
        lockLunge = false;
    }


    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("run", true);
            realSpeed = fastSpeed;
        }
        else
        {
            anim.SetBool("run", false);
            realSpeed = speed;
        }
    }


    void sit()
    {
        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("sit", true);

            rb.velocity = new Vector2(0, 0);
        }
        else
        {
            anim.SetBool("sit", false);
        }
    }


    public bool onWall;
    public LayerMask Wall;
    public Transform WallCheck;
    public float WallcheckRadius;
    void CheckingWall()
    {
        if (rb.velocity.y < 0)
        {
            onWall = Physics2D.OverlapCircle(WallCheck.position, WallcheckRadius, Wall);
        }
        else { onWall = false; }

        anim.SetBool("onWall", onWall);
    }


    public float slideSpeed = -1;
    private float gravityDef;
    void MoveOnWall()
    {
        if (onWall && !onGround && Input.GetAxisRaw("Horizontal") == Mathf.Round(transform.localScale.x))
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, slideSpeed);
        }
        else { rb.gravityScale = gravityDef; }
    }


    public float wallJumpVelocity = 20f;
    void WallJump()
    {
        if (onWall && !onGround && Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 0)
        {
            anim.Play("wallJump");
            rb.gravityScale = gravityDef;
            rb.velocity = new Vector2(0, 0);
            
            rb.velocity = new Vector2(rb.velocity.x, wallJumpVelocity);
        }
    }



    public bool jumpMoveNow = false;
    Vector3 newPosition;
    float progress = 0;
    void jumpMove()
    {
        if (Input.GetKeyDown(KeyCode.C) && !jumpMoveNow) { jumpMoveNow = true; newPosition = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z); Jump(); }

        //if (jumpMoveNow) { progress = (transform.position.x - newPosition.x) / 2; transform.position = new Vector2(transform.position.x, Mathf.Sin(progress * 4f)); }

        if (jumpMoveNow && transform.position != newPosition) { transform.position = Vector3.MoveTowards(transform.position, new Vector2(newPosition.x, transform.position.y), speed * Time.deltaTime); }
        else if (jumpMoveNow && transform.position == newPosition) { jumpMoveNow = false; progress = 0; }
    }

}