using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    Controls controls;
    float dir;

    public CharacterController2D controller;
    public Collider2D[] characterColliders;
    public Rigidbody2D rb2D;
    [SerializeField] private float grav = 7f;

    public float walkSpeed;
    bool isSprinting;
    bool isCrouching;
    Vector2 movement;

    public bool isHoldingJump;
    public float maxHoldJumpTime;
    [SerializeField] float holdJumpTimer;
    bool wasJumping;

    float doubleTapTimer = 0f;
    [SerializeField] float crouchTapTime = 0.1f;
    public string platformTag;

    private void Start()
    {
        controls = new Controls();
        //Movement controls setup
        controls.OperatorActionMap.Movement.performed += ctx => dir = ctx.ReadValue<float>();
        controls.OperatorActionMap.Movement.canceled += ctx => dir = 0f;
        controls.OperatorActionMap.Movement.Enable();
        //Jump controls setup
        controls.OperatorActionMap.Jump.performed += JumpStart;
        controls.OperatorActionMap.Jump.canceled += JumpEnd;
        controls.OperatorActionMap.Jump.Enable();
        //crouch controls setup
        controls.OperatorActionMap.Crouch.performed += CrouchStart;
        controls.OperatorActionMap.Crouch.canceled += CrouchEnd;
        controls.OperatorActionMap.Crouch.Enable();
        //sprint controls setup
        controls.OperatorActionMap.Sprint.performed += SprintStart;
        controls.OperatorActionMap.Sprint.canceled += SprintEnd;
        controls.OperatorActionMap.Sprint.Enable();
    }



    // Update is called once per frame
    void Update()
    {
        //Update double tap timer
        

        //reset gravity if it was changed

        //if (rb2D.gravityScale == 0)
        //{
        //    rb2D.gravityScale = grav;
        //}
        //if (isHoldingJump && (controller.ceilingObject() != null) && (controller.ceilingObject().GetComponent<FallThruPlatform>() != null))
        //{
        //    //Ledge Grab
        //    rb2D.gravityScale = 0;
        //    movement = Vector2.zero;
        //    FallThruPlatform platform = controller.ceilingObject().GetComponent<FallThruPlatform>();
        //    if (this.gameObject.CompareTag("Player")) {
        //        Vector2 pos = this.transform.position;
        //        Vector2.Lerp(pos, new Vector2(pos.x, platform.platformTarget.position.y), 100f);
        //    }
        //}
        //Debug.Log(isSprinting);
    }

    private void FixedUpdate()
    {
        holdJumpTimer += Time.fixedDeltaTime;
        doubleTapTimer += Time.fixedDeltaTime;

        movement.x = dir * walkSpeed;

        bool grounded = controller.isGrounded();
        if (!grounded)
        {
            if (holdJumpTimer >= maxHoldJumpTime)
            {
                isHoldingJump = false;
                wasJumping = false;
            }
        }
        if (grounded && !isHoldingJump)
        {
            holdJumpTimer = 0f;
        }
        if (!grounded && isHoldingJump)
        {
            wasJumping = true;
        }
        //Move character
        controller.Move(movement.x * Time.fixedDeltaTime, isCrouching, isSprinting, isHoldingJump, wasJumping);



    }

    void JumpStart(CallbackContext ctx) 
    {
        isHoldingJump = true;    
    }

    void JumpEnd(CallbackContext ctx)
    {
        isHoldingJump = false;
        wasJumping = false;
    }

    void CrouchStart(CallbackContext ctx)
    {
        if ((doubleTapTimer < crouchTapTime) && (controller.groundObject().CompareTag(platformTag)))
        {
            foreach (Collider2D c in characterColliders)
            {
                c.isTrigger = true;
            }
        }
        else
        {
            isCrouching = true;
            doubleTapTimer = 0;
        }
    }

    void CrouchEnd(CallbackContext ctx)
    {
        isCrouching = false;
    }

    void SprintStart(CallbackContext ctx)
    {
        isSprinting = true;
    }

    void SprintEnd(CallbackContext ctx)
    {
        isSprinting = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        foreach (Collider2D c in characterColliders)
        {
            c.isTrigger = false;
        }
    }
}