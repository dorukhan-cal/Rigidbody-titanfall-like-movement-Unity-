using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float crouchedSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundDrag;
    [SerializeField] private float maxSlopeAngle;


    [Header("References")]
    [SerializeField] private Transform playerReference;
    [SerializeField] private Transform groundReference;

    [Header("Layers")]
    [SerializeField] private LayerMask groundMask;

    private float horizontalInput, verticalInput;
    private float normalYScale;
    private float moveSpeed;
    private float groundDistance = 0.2f;
    private float crouchedYScale = 0.5f;
    private float jumpCooldown = 0.25f;
    private float maxSlideTime = 0.75f;
    private bool isGrounded;
    private bool canJump;
    private bool jumpOffSlope;
    public bool slope; //Debug

    private Vector3 movementDirection;
    private Rigidbody rb;
    public MovementState moveState; //made public for debugging
    private RaycastHit slopeHit;

    public enum MovementState
    {
        idle,
        walking,
        running,
        crouched,
        sliding,
        inAir

    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalYScale = transform.localScale.y;
        canJump = true;
        moveState = MovementState.idle;
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        slope = onSlope(); //Debug
        checkGround();
        inputHandler();
        speedLimiter();
        handleDrag();
        handlePlayerMovement();
        jump();

       switch (moveState) {
            default:
            case MovementState.idle:

            case MovementState.walking:
                walk();
            break;
            case MovementState.crouched:
                crouch();
            break;
            case MovementState.running:
                run();
            break;
            case MovementState.sliding:
                if(!onSlope())
                    Invoke(nameof(resetToCrouch), maxSlideTime);
                slide();
            break;
        }
        
    }

    void FixedUpdate()
    {

    }

    private void inputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(isGrounded)
        {   
            if(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput) > 0f) 
            {

                if(Input.GetButton("Sprint") && moveState != MovementState.crouched)
                {
                    moveState = MovementState.running;
                    if(Input.GetButton("Crouch"))
                        moveState = MovementState.sliding; 
                }

                else if(Input.GetButton("Crouch"))
                {
                    if(moveState != MovementState.running)
                        moveState = MovementState.crouched;           
                }
                else   
                    moveState = MovementState.walking;
            }

            else
            {
                moveState = MovementState.idle;
                if(Input.GetButton("Crouch"))
                {
                    moveState = MovementState.crouched;     
                }

            }
            if(!Input.GetButton("Crouch"))
                standUp();
        }

        else
           moveState =  MovementState.inAir; 
    }

    private void handlePlayerMovement()
    {  
        //The actual movement 
        movementDirection = playerReference.forward * verticalInput + playerReference.right * horizontalInput;
        
        if(onSlope() && !jumpOffSlope)
        {
            rb.AddForce(getSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y != 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
               
        }

        rb.useGravity = !onSlope();

        if(isGrounded)
            rb.AddForce(movementDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!isGrounded)
            rb.AddForce(movementDirection.normalized * moveSpeed * 3f, ForceMode.Force);
    
    }

    /*Utility functions*/
    private void checkGround()
    {
        isGrounded = Physics.Raycast(groundReference.position, Vector3.down, 0.3f, groundMask);
    }

    private void speedLimiter()
    {
        //Speed limiter here      
        if(onSlope() && !jumpOffSlope)
        {
            if(rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        } 

        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if(flatVelocity.magnitude > moveSpeed)
            {
                Vector3 constrainedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(constrainedVelocity.x, rb.velocity.y, constrainedVelocity.z);
            }
        }

       /*Alternative speed limiter using Mathf.Clamp
        visually works fine, 
        however makes the diagonal speed of the rigidbody exceed moveSpeed
        could be good optimization if worked on a little bit.
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveSpeed , moveSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveSpeed , moveSpeed)); */
   
    }

    
    private bool onSlope()
    {
       if(Physics.Raycast(groundReference.position, Vector3.down, out slopeHit, 0.5f))
       {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle !=0;          
       }

       return false;
    }

    private Vector3 getSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(movementDirection, slopeHit.normal).normalized;
    }

    private void handleDrag()
    {
        if(isGrounded) rb.drag = groundDrag;
        else rb.drag = 0f;
    }


    /*Movement modifiers*/

    private void walk()
    {
        moveSpeed = walkSpeed;
    }

    private void run()
    {
        moveSpeed = runSpeed;
    }

    private void crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchedYScale, transform.localScale.z);
        moveSpeed = crouchedSpeed;
        rb.AddForce(Vector3.down * 0.75f, ForceMode.Impulse);
    }

    private void slide()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchedYScale, transform.localScale.z);
        moveSpeed = slideSpeed;
        rb.AddForce(Vector3.down * 0.75f, ForceMode.Impulse);
    }

    private void standUp()
    {
        transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
    }

    private void jump()
    {
            if(Input.GetButtonDown("Jump") && canJump)
            {
                canJump = false;
                 //reset y velocity to keep consistent jump height
                jumpOffSlope = true;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                Invoke(nameof(resetJump), jumpCooldown);
            }
    }

    private void resetJump()
    {
        canJump = true;
        jumpOffSlope = false;
    }

    private void resetToCrouch()
    {
        moveState = MovementState.crouched;
    }
}
