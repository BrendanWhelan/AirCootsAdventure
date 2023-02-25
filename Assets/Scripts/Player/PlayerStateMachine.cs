using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;
using DG.Tweening;

public class PlayerStateMachine : MonoBehaviour
{
    private StateMachine<PlayerStateMachine> StateMachine;

    //States
    private IdleState idleState;
    private MoveState moveState;
    private CrouchState crouchState;
    private SlidingState slidingState;
    private GrapplingState grapplingState;
    private SwingState swingingState;
    private JumpState jumpingState;
    private IceJumpState iceJumpingState;
    private DyingState dyingState;
    private BounceState bounceState;
    private MovePositionState movePositionState;

    public Transform orientation;
    
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float defaultGroundDrag = 0;

    private float groundDrag = 0f;

    [SerializeField]
    private Collider playerCollider;
    
    [SerializeField]
    private PhysicMaterial defaultPhysicsMat;
    [SerializeField]
    private PhysicMaterial slidingPhysicsMat;
    [SerializeField]
    private PhysicMaterial rotatingMaterial;
    [SerializeField]
    private PhysicMaterial iceMaterial;

    [SerializeField]
    private Animator animator;
    
    public enum PlayerPhysics
    {
        Standard,
        Sliding,
        Rotating,
        Ice
    }
    
    [SerializeField]
    private float jumpForce;
    private float jumpCooldown = 0.2f;
    private bool readyToJump = true;

    [SerializeField]
    private float speedIncreaseMultiplier;
    [SerializeField]
    private float slopeIncreaseMultiplier;
    
    
    [Header("Ground Check")]
    [SerializeField]
    private float playerHeight;
    [SerializeField]
    private LayerMask groundLayer;
    public bool isGrounded;
    public bool coyoteGrounded;

    [SerializeField]
    private LayerMask pickupLayer;
    
    [Header("Slope Check")]
    [SerializeField]
    private float maxSlopeAngle;
    private RaycastHit slopeHit;

    private float desiredMaxSpeed;
    float currentSpeed;
    private static readonly int MoveSpeedAnimationValue = Animator.StringToHash("MoveSpeed");

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    //Grappling
    [Header("Grappling")]
    //Maximum reach of grapple
    [SerializeField]
    private float maxGrappleDistance = 10f;
    //Delay before firing the grappling hook
    [SerializeField]
    private float grappleDelay = 0.25f;

    [SerializeField]
    private float grappleFOV = 60;
    
    [SerializeField]
    private LineRenderer grappleLine;

    [SerializeField]
    private Transform grappleShootPoint;
    
    //Current grapple point
    private Vector3 grapplePoint;

    [SerializeField]
    private float grappleCooldown = 0.5f;
    private float grapplingCooldownTimer;
    [SerializeField]
    private LayerMask grappleLayer;

    public bool threwGrapplingHook = false;
    public bool activeGrapple = false;
    private RaycastHit predictionHit;
    [SerializeField]
    private float predictionSphereCastRadius;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private ThirdPersonCam playerCam;

    private SpringJoint joint;
    private bool isSwinging = false;
    public bool isJumping = false;
    public bool isBouncing = false;
    
    public bool jumpButtonPressed = false;

    private float jumpBufferTime = 0.1f;
    private float jumpButtonTime = 0f;

    public bool mouseClicked = false;
    
    private float clickBufferTime = 0.1f;
    private float currentClickBufferTime = 0f;
    
    private float coyoteTime = 0.1f;
    private float currentCoyoteTime = 0f;

    public bool isDying = false;

    private float deathHeight = -100f;

    public bool isSliding = false;

    [SerializeField]
    private Material grappleDefaultMat;
    [SerializeField]
    private Material grappleGlowMat;

    [SerializeField]
    private Material grappleTriggerDefaultMat;
    [SerializeField]
    private Material grappleTriggerGlowMat;
    
    private MeshRenderer currentGrappleMesh;

    private bool manuallyJumped = false;
    private bool onFallingPlatform = false;

    public bool onIce = false;

    public bool forcedMove = false;
    
    private void Awake()
    {
        //Initialize States
        idleState = new IdleState(this);
        moveState = new MoveState(this);
        crouchState = new CrouchState(this);
        slidingState = new SlidingState(this);
        grapplingState = new GrapplingState(this);
        swingingState = new SwingState(this);
        jumpingState = new JumpState(this);
        dyingState = new DyingState(this);
        bounceState = new BounceState(this);
        iceJumpingState = new IceJumpState(this);
        movePositionState = new MovePositionState(this);

        groundDrag = defaultGroundDrag;
        StateMachine = new StateMachine<PlayerStateMachine>(this);
        //Set the initial state
        StateMachine.ChangeState(idleState);
    }

    private void Start()
    {
        deathHeight = RespawnManager.instance.GetDeathHeight();
        GameManager.instance.SubscribeToPauseEvent(GamePaused);
        GamePaused(GameManager.instance.GamePaused);
    }

    private Rigidbody currentHitRigidbody = null;
    private bool isOnRotatingSurface = false;
    //private bool wasOnRotatingSurface = false;
    private RotateTransform currentRotateTransform;
    
    private void Update()
    {
        float speedStrength = Mathf.Clamp(rb.velocity.magnitude.Remap(10, 50, 0, 1),0,1);
        AudioManager.instance.AdjustWindVolume(speedStrength);
        if(playerCam != null)
            playerCam.SetNoise(speedStrength);
        if (isDying || GameManager.instance.GamePaused) return;

        if (dyingFromDeathPlane && !isSwinging)
        {
            dyingFromDeathPlane = false;
            if (transform.position.y <= deathPlaneHeight)
            {
                Die();
                return;
            }
            deathPlaneHeight = 0f;
        }
        
        if (forcedMove)
        {
            StateMachine.Update();
            return;
        }
        
        if (controlsDisabled) return;
        
        //Check for death plane
        if ((transform.position.y <= deathHeight && !isSwinging) || Input.GetKey(KeyCode.R))
        {
            Die();
            return;
        }
        
        if (jumpButtonPressed)
        {
            jumpButtonTime += Time.deltaTime;
            if (jumpButtonTime >= jumpBufferTime)
            {
                jumpButtonPressed = false;
            }
        }

        if (mouseClicked)
        {
            currentClickBufferTime += Time.deltaTime;
            if (currentClickBufferTime >= clickBufferTime)
                mouseClicked = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump)
        {
            jumpButtonPressed = true;
            jumpButtonTime = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked = true;
            currentClickBufferTime = 0f;
        }
        
        RaycastHit hit;
        isGrounded = Physics.BoxCast(playerCollider.bounds.center,  new Vector3(0.1f,0.1f,0.1f),-orientation.up, out hit, Quaternion.identity,playerCollider.bounds.size.y/2-0.025f, groundLayer) || onFallingPlatform;

        animator.SetBool(IsGrounded, isGrounded);
        //Debug.DrawRay(playerCollider.bounds.center,-orientation.up,Color.red);

        if (!isSwinging)
        {
            if (hit.rigidbody != null)
            {
                if (hit.rigidbody != currentHitRigidbody)
                {
                    onIce = false;
                    currentRotateTransform = hit.rigidbody.GetComponent<RotateTransform>();
                    isOnRotatingSurface = currentRotateTransform != null;
                    if (isOnRotatingSurface)
                    {
                        ChangePhysicsMat(PlayerPhysics.Rotating);
                    }
                    else
                    {
                        ChangePhysicsMat(PlayerPhysics.Standard);
                    }
                }
            }
            else if (hit.transform != null)
            {
                if (hit.transform.tag.Equals("Ice"))
                {
                    onIce = true;
                    ChangePhysicsMat(PlayerPhysics.Ice);
                }
                else
                {
                    onIce = false;
                    ChangePhysicsMat(PlayerPhysics.Standard);
                }
                isOnRotatingSurface = false;
            }
        }
        currentHitRigidbody = hit.rigidbody;

        bool onSlope = OnSlope();
        if (!onSlope && slopeAngle >= maxSlopeAngle) isGrounded = false;
        
        if (!coyoteGrounded && isGrounded)
        {
            //If we just touched the ground reset coyote time
            coyoteGrounded = true;
            currentCoyoteTime = 0f;
            //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        else if (!isGrounded && coyoteGrounded)
        {
            //If we were on the ground but just stepped off, increment coyote time
            currentCoyoteTime += Time.deltaTime;
            if (currentCoyoteTime >= coyoteTime)
            {
                coyoteGrounded = false;
            }
        }

        if (!isGrounded && !isJumping && !isSwinging && !isSliding)
        {
            //Switch to jump state if we're not on the ground and not swinging
            if (manuallyJumped || !onFallingPlatform)
            {
                if(onIce)
                    ChangeState("IceJump");
                else
                    ChangeState("Jump");
            }
        }
        else if (isGrounded && isJumping && readyToJump && !isSwinging && !isSliding)
        {
            manuallyJumped = false;
            //if we're on the ground and we are in the jumping state, we want to swap to the move state unless we're trying to jump
            if (jumpButtonPressed)
            {
                Jump();
            }
            else
            {
                if(!onSlope)
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                ChangeState("Move");
            }
        }
        
        CheckForSwingPoints();
        
        StateMachine.Update();
        //Limit speed to current maximum
        SpeedControl();

        if (grapplingCooldownTimer > 0)
            grapplingCooldownTimer -= Time.deltaTime;

        CheckForPickups();
    }

    private float pickupRadius = 0.4f;
    private Collider[] pickupResults = new Collider[5];
    private void CheckForPickups()
    {
        int pickups = Physics.OverlapSphereNonAlloc(transform.position, pickupRadius, pickupResults, pickupLayer);
        if (pickups == 0) return;

        Pickup pickup;
        for (int i = 0; i < pickups; i++)
        {
            pickup = pickupResults[i].GetComponent<Pickup>();
            if(pickup != null)
                pickup.PickUp();
        }
    }
    
    public bool dragManuallyChangedThisFrame = false;

    private void FixedUpdate()
    {
        if (isDying||GameManager.instance.GamePaused) return;
        if(isSwinging)
            UpdateGrapplePoint();
        
        RaycastHit fallingHit;
        if (Physics.Raycast(transform.position, -orientation.up, out fallingHit, 2f, groundLayer) && !manuallyJumped)
        {
            if (fallingHit.transform.tag.Equals("Falling"))
            {
                onFallingPlatform = true;
            }
            else
            {
                onFallingPlatform = false;
            }
        }
        else
        {
            onFallingPlatform = false;
        }

        StateMachine.FixedUpdate();

        if (onIce && !isJumping && !isSwinging) rb.mass = 10;
        else rb.mass = 1;
        
        
        if (!dragManuallyChangedThisFrame)
        {
            if (isGrounded && !activeGrapple && !isBouncing && !onFallingPlatform && !onIce && !isSliding) rb.drag = groundDrag;
            else rb.drag = 0;
        }
        else
        {
            dragManuallyChangedThisFrame = false;
        }
    }

    public void CheckForGrapple()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //Stop swinging if we release
            if (isSwinging && joint)
            {
                StopSwing();
            }

            mouseClicked = false;
        }
        else if (mouseClicked)
        {
            StartSwing();
        }
    }
    
    private void SpeedControl()
    {
        //Disable speed limitation when grappling
        if (activeGrapple) return;
        
        if (OnSlope() && readyToJump)
        {
            if (rb.velocity.magnitude > currentSpeed)
                rb.velocity = rb.velocity.normalized * currentSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //limit velocity to maximum
            if (flatVel.magnitude > currentSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * currentSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        
        animator.SetFloat(MoveSpeedAnimationValue,GetSpeed());

        float moveDir = Input.GetAxisRaw("Horizontal");
        if (moveDir < -0.5f) spriteRenderer.flipX = true;
        else if (moveDir > 0.5f) spriteRenderer.flipX = false;
    }

    private Coroutine speedLerpCoroutine;
    public void SetMaxSpeed(float newSpeed, bool instant=false)
    {
        desiredMaxSpeed = newSpeed;

        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        if (Mathf.Abs(desiredMaxSpeed - speed) > 10f && currentSpeed != 0 && !instant)
        {
            //currentSpeed = speed;
            if (speedLerpCoroutine != null)
            {
                StopCoroutine(speedLerpCoroutine);
                speedLerpCoroutine = null;
            }
            speedLerpCoroutine = StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            currentSpeed = desiredMaxSpeed;
        }
    }
    
    public void MovePlayer(Vector3 intendedDirection, float speed, ForceMode forceMode)
    {
        //Disable forces while grappling
        if (activeGrapple) return;

        if (OnSlope() && readyToJump)
        {
            rb.AddForce(GetSlopeAdjustedMove(intendedDirection) * (speed * 20f), ForceMode.Force);
        }
        else
        {
            Vector3 direction = intendedDirection * (speed * 10f);
            rb.AddForce(direction, forceMode);
        }
    }

    public void Jump()
    {
        if (!readyToJump || !coyoteGrounded) return;
        AudioManager.instance.PlaySound(Sounds.Jump);
        rb.mass = 1;
        dragManuallyChangedThisFrame = true;
        manuallyJumped = true;
        rb.drag = defaultGroundDrag;
        // reset y velocity
        jumpButtonPressed = false;
        coyoteGrounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
        readyToJump = false;
        Invoke(nameof(ResetJump),jumpCooldown);
        if(onIce)
            ChangeState("IceJump");
        else
            ChangeState("Jump");
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    
    private void ResetJump()
    {
        readyToJump = true;
    }

    public void AdjustGroundDrag(float adjustedDrag)
    {
        groundDrag = adjustedDrag;
    }

    public void ResetGroundDrag()
    {
        groundDrag = defaultGroundDrag;
    }

    public float GetSpeed()
    {
        return new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
    }

    public void ChangePhysicsMat(PlayerPhysics physicsMat)
    {
        switch (physicsMat)
        {
            case PlayerPhysics.Standard:
                if (isOnRotatingSurface)
                    playerCollider.material = rotatingMaterial;
                else
                    playerCollider.material = defaultPhysicsMat;
                break;
            case PlayerPhysics.Sliding:
                playerCollider.material = slidingPhysicsMat;
                break;
            case PlayerPhysics.Rotating:
                playerCollider.material = rotatingMaterial;
                break;
            case PlayerPhysics.Ice:
                playerCollider.material = iceMaterial;
                break;
        }
    }

    private float slopeAngle;
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position,-orientation.up,out slopeHit,playerCollider.bounds.size.y+0.2f,groundLayer))
        {
            //Calculate how steep slope is based on the angle between up and the normal of the slope
            slopeAngle = Vector3.Angle(orientation.up, slopeHit.normal);
            return slopeAngle < maxSlopeAngle && slopeAngle != 0;
        }
        slopeAngle = 0f;
        return false;
    }

    public Vector3 GetSlopeAdjustedMove(Vector3 intendedMovement)
    {
        return Vector3.ProjectOnPlane(intendedMovement, slopeHit.normal).normalized;
    }

    public Vector3 GetSlopeSlideMove(Vector3 intendedMovement)
    {
        Vector3 left = Vector3.Cross(slopeHit.normal, orientation.up);
        Vector3 slope = Vector3.Cross(slopeHit.normal, left);
        
        float intendedAngle = Vector3.Angle(intendedMovement, slope);
        return Vector3.Lerp(intendedMovement,slope,intendedAngle/60).normalized;
    }
    
    public void ChangeState(string state)
    {
        switch (state)
        {
            case "Idle":
                StateMachine.ChangeState(idleState);
                break;
            case "Move":
                StateMachine.ChangeState(moveState);
                break;
            case "Crouch":
                StateMachine.ChangeState(crouchState);
                break;
            case "Sliding":
                StateMachine.ChangeState(slidingState);
                break;
            case "Grappling":
                StateMachine.ChangeState(grapplingState);
                break;
            case "Swinging":
                StateMachine.ChangeState(swingingState);
                break;
            case "Jump":
                StateMachine.ChangeState(jumpingState);
                break;
            case "IceJump":
                StateMachine.ChangeState(iceJumpingState);
                break;
            case "Dying":
                StateMachine.ChangeState(dyingState);
                break;
            case "Bounce":
                StateMachine.ChangeState(bounceState);
                break;
            case "MovePosition":
                StateMachine.ChangeState(movePositionState);
                break;
        }
    }

    #region Grappling
    public void StartGrapple()
    {
        if (grapplingCooldownTimer > 0) return;

        threwGrapplingHook = true;
        
        //Throw grappling hook
        if (predictionHit.point != Vector3.zero)
        {
            grapplePoint = predictionHit.point;
            
            Invoke(nameof(ExecuteGrapple), grappleDelay);

            grapplingState.SetGrapple(grapplePoint);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            grapplingState.SetGrapple(grapplePoint);
            Invoke(nameof(StopGrapple),grappleDelay);
        }

        grappleLine.enabled = true;
    }

    private bool disableGrapplingOnNextTouch = false;
    
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        
        velocityToSet = grapplingState.CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        //Set the jump slightly later so that speed control is disabled properly
        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 velocityToSet;

    private void OnCollisionEnter(Collision collision)
    {
        if (disableGrapplingOnNextTouch)
        {
            StopGrapple();
        }
    }

    public void SetVelocity(Vector3 vel)
    {
        rb.velocity = vel;
    }
    
    private void SetVelocity()
    {
        disableGrapplingOnNextTouch = true;
        rb.velocity = velocityToSet;
        playerCam.DoFov(grappleFOV);
    }

    private void ExecuteGrapple()
    {
        ChangeState("Grappling");
        grapplingState.ExecuteGrapple();
    }
    
    public void DelayedStopGrapple()
    {
        Invoke(nameof(StopGrapple), 1f);
    }
    private void StopGrapple()
    {
        playerCam.ResetFov();
        activeGrapple = false;
        disableGrapplingOnNextTouch = false;
        threwGrapplingHook = false;
        grapplingCooldownTimer = grappleCooldown; 
        grappleLine.enabled = false;
        ChangeState("Move");
    }
    #endregion
    
    #region Swinging

    private Transform grappledTransform;
    private Vector3 grappleOffset;
    
    public void StartSwing()
    {
        if (predictionHit.point == Vector3.zero || grappledTransform == null) return;

        if (joint != null)
        {
            Destroy(joint);
        }
        
        AudioManager.instance.PlaySound(Sounds.Grapple);
        mouseClicked = false;

        playerCam.DoFov(grappleFOV);
        threwGrapplingHook = true;
        isSwinging = true;
        grapplePoint = predictionHit.point;
        
        grappleOffset = grapplePoint - grappledTransform.position;
        
        // joint = transform.gameObject.AddComponent<ConfigurableJoint>();
        joint = transform.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;
        
        // joint.anchor = grapplePoint - transform.position;
        // joint.xMotion = ConfigurableJointMotion.Limited;
        // joint.zMotion = ConfigurableJointMotion.Limited;
        // joint.yMotion = ConfigurableJointMotion.Limited;

        joint.spring = 0.1f;//4.5f;
        joint.damper = 0.1f;//7f;
        joint.massScale = 100f;//4.5f;

        joint.maxDistance = 0f;
        joint.minDistance = 0f;
        
        if (grappledTransform.tag.Equals("Trigger"))
        {
            GrappleSpot grappleSpot = grappledTransform.GetComponent<GrappleSpot>();
            if(grappleSpot != null) grappleSpot.Grappled();
        }
        
        ChangeState("Swinging");

        Vector3 force = GetGrappleDirection(rb.velocity.normalized);
        rb.velocity = rb.velocity.magnitude * force;
    }

    public float dotThreshold = 0.5f;
    private float rotationAngle = 45f;
    private float heightForce = 5f;

    public Vector3 GetGrappleDirection(Vector3 forwardDirection)
    {
        if (!grappledTransform) return Vector3.zero;
        
        Vector3 directionToGrapple = grapplePoint - transform.position;
        float dot = Vector3.Dot(forwardDirection, directionToGrapple.normalized);
        
        Vector3 forceDirection;
        if (dot >= dotThreshold)
        {
            // The vectors are similar, so adjust horizontal
            Vector3 cross = Vector3.Cross(orientation.right, directionToGrapple.normalized);
            Quaternion rotation = Quaternion.AngleAxis(rotationAngle, cross);
            forceDirection = rotation * forwardDirection;
        
            cross = Vector3.Cross(orientation.up, directionToGrapple.normalized);
            rotation = Quaternion.AngleAxis(rotationAngle,cross);
            forceDirection += rotation * orientation.up;
        }
        else
        {
            // The vectors are dissimilar, so apply the force directly toward the character's transform.forward
            forceDirection = forwardDirection;
            float heightDiff = directionToGrapple.y-transform.position.y;
            if (heightDiff > 0 && heightDiff < 0.5f)
            {
                forceDirection += orientation.up * heightForce;
            }
            else if (heightDiff < 0 && heightDiff > -0.5f)
            {
                forceDirection += -orientation.up * heightForce;
            }
        }

        return forwardDirection;
    }
    
    public void UpdateGrapplePoint()
    {
        if (grappledTransform == null) return;
        grapplePoint = grappledTransform.position + grappleOffset;
        joint.connectedAnchor = grapplePoint;
    }
    
    public void RotateCam(Vector3 normalizedRotateDirection)
    {
        playerCam.Tilt(normalizedRotateDirection);
    }
    
    public void StopSwing()
    {
        grappledTransform = null;
        
        playerCam.ResetFov();
        isSwinging = false;
        threwGrapplingHook = false;
        Destroy(joint);
        if(isGrounded)
            ChangeState("Move");
        else
            ChangeState("Jump");
    }

    // public void ShortenCable(float reelForce)
    // {
    //     Vector3 directionToCable = grapplePoint - transform.position;
    //     rb.AddForce(directionToCable.normalized * reelForce);
    //
    //     float distanceFromPoint = Vector3.Distance(grapplePoint, transform.position);
    //     
    //     
    //     if(distanceFromPoint < 1f)
    //         StopSwing();
    // }

    private float minimumGrappleDistance = 2f;
    private void CheckForSwingPoints()
    {
        if (isSwinging || activeGrapple) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxGrappleDistance, grappleLayer);
        
        RaycastHit hit;
        Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleLayer);

        Transform newGrappleTransform = null;
        if (hit.point != Vector3.zero && Vector3.Distance(cam.position,hit.point) > minimumGrappleDistance)
        {
            newGrappleTransform = hit.transform;
        }
        else if (sphereCastHit.point != Vector3.zero && Vector3.Distance(cam.position,sphereCastHit.point) > minimumGrappleDistance)
        {
            newGrappleTransform = sphereCastHit.transform;
        }
        
        if (grappledTransform != null && grappledTransform == newGrappleTransform) return;

        grappledTransform = newGrappleTransform;

        if (currentGrappleMesh != null)
        {
            currentGrappleMesh.sharedMaterial = currentGrappleMesh.tag.Equals("Trigger") ? grappleTriggerDefaultMat : grappleDefaultMat;
        }

        if (grappledTransform != null)
        {
            MenuManager.instance.SetCrossfurOpacity(1f);
            currentGrappleMesh = grappledTransform.GetComponent<MeshRenderer>();

            if (currentGrappleMesh != null)
            {
                currentGrappleMesh.sharedMaterial = currentGrappleMesh.tag.Equals("Trigger") ? grappleTriggerGlowMat : grappleGlowMat;
            }
        }
        else
        {
            MenuManager.instance.SetCrossfurOpacity(0.3f);
        }
        
        predictionHit = hit.point == Vector3.zero ? sphereCastHit : hit;
    }
    #endregion
    public void SetAnimationBool(string animName, bool animBool)
    {
        animator.SetBool(animName,animBool);
    }

    public Vector3 GetGrapplePoint()
    {
        return grappledTransform.position + grappleOffset;
    }

    public Vector3 GetGrappleShootPoint()
    {
        return grappleShootPoint.position;
    }
    
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //Lerp the speed with the desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMaxSpeed - currentSpeed);
        float startValue = currentSpeed;

        while (time < difference)
        {
            currentSpeed = Mathf.Lerp(startValue, desiredMaxSpeed, time / difference);

            if (Mathf.Abs(desiredMaxSpeed - GetSpeed()) < 1)
                break;
            
            if (OnSlope())
            {
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }else
                time += Time.deltaTime * speedIncreaseMultiplier;
            yield return null;
        }

        currentSpeed = desiredMaxSpeed;
    }

    public void Respawn()
    {
        AudioManager.instance.PlaySound(Sounds.Respawn);
        SetAnimationBool("isRespawning",true);
        RespawnPoint respawnPoint = RespawnManager.instance.GetRespawnPoint();
        
        transform.position = respawnPoint.position;
        playerCam.SetView(respawnPoint.cameraValues);
    }

    public void FinishedRespawning()
    {
        rb.detectCollisions = true;
        gravityControl.UseGravity(true);
        SetAnimationBool("isRespawning", false);
        ChangeState("Idle");
    }

    public void Die()
    {
        GameManager.instance.PlayerDied();
        AudioManager.instance.PlaySound(Sounds.Death);
        if (isSwinging)
        {
            grappledTransform = null;
        
            if (joint != null)
            {
                Destroy(joint);
            }
            
            playerCam.ResetFov();
            isSwinging = false;
            threwGrapplingHook = false;
        }

        RotateCam(Vector3.zero);
        dyingFromDeathPlane = false;
        deathPlaneHeight = 0f;
        rb.detectCollisions = false;
        rb.velocity = Vector3.zero;
        gravityControl.UseGravity(false);
        ChangeState("Dying");
        playerCam.CameraExplosion();
    }
    
    public void ApplyForce(Vector3 force, ForceMode mode, bool zeroGravity, bool cancelJumps)
    {
        ChangeState("Bounce");
        rb.drag = 0;
        dragManuallyChangedThisFrame = true;
        rb.mass = 1;

        if (cancelJumps)
        {
            coyoteGrounded = false;
            jumpButtonPressed = false;
            readyToJump = false;
            Invoke(nameof(ResetJump),jumpCooldown);
        }
        if (zeroGravity)
        {
            rb.velocity = Vector3.zero;
        }
        
        SetMaxSpeed(force.magnitude,true);
        rb.AddForce(force,mode);
    }

    public void ApplyForceNoBounce(Vector3 force, ForceMode mode, bool zeroGravity, bool cancelJumps)
    {
        rb.drag = 0;
        dragManuallyChangedThisFrame = true;
        rb.mass = 1;

        if (cancelJumps)
        {
            coyoteGrounded = false;
            jumpButtonPressed = false;
            readyToJump = false;
            Invoke(nameof(ResetJump),jumpCooldown);
        }
        
        SetMaxSpeed(force.magnitude,true);
        rb.AddForce(force,mode);
    }
    
    private Vector3 velocityBeforePause = Vector3.zero;
    private float prePauseDamper = 0f;
    private float prePauseSpring = 0f;
    private void GamePaused(bool paused)
    {
        if (paused)
        {
            if (joint != null)
            {
                prePauseDamper = joint.damper;
                prePauseSpring = joint.spring;
                joint.damper = 0f;
                joint.spring = 0f;
            };
            velocityBeforePause = rb.velocity;
            rb.velocity = Vector3.zero;
        }
        else
        {
            if (joint != null)
            {
                joint.damper = prePauseDamper;
                joint.spring = prePauseSpring;
                prePauseDamper = prePauseSpring = 0f;
            }
            rb.velocity = velocityBeforePause;
        }
        gravityControl.UseGravity(!paused && !isDying);
        rb.detectCollisions = !isDying;
        animator.enabled = !paused;
    }

    private bool dyingFromDeathPlane = false;
    private float deathPlaneHeight = 0f;
    public void DieFromDeathPlane(float heightOfPlane)
    {
        if (!isSwinging)
        {
            Die();
        }
        else
        {
            dyingFromDeathPlane = true;
            deathPlaneHeight = heightOfPlane;
        }
    }

    private Vector3 newIntendedDownDirection;
    
    public void SetDownDirection(Vector3 down)
    {
        orientation.up = -down;
    }

    public void ForceMovePlayerPosition(Vector3 position)
    {
        rb.isKinematic = true;
        transform.position = position;
        rb.isKinematic = false;
    }

    public void MoveToPositionDynamically(Vector3 worldPos)
    {
        rb.velocity = Vector3.zero;
        
        movePositionState.desiredPosition = worldPos;
        forcedMove = true;
        ChangeState("MovePosition");
    }

    private bool controlsDisabled = false;
    public void DisableControls(bool disable)
    {
        controlsDisabled = disable;
    }

    public void FadePlayer()
    {
        spriteRenderer.DOFade(0f, 1f);
    }
    
    [SerializeField]
    private GravityControl gravityControl;
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
}

public static class ExtensionMethods {
 
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
   
}