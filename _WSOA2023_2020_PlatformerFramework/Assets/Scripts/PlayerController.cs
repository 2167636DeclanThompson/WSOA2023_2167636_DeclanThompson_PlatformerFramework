using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerController : MonoBehaviour
{
    ///// PUBLIC FIELDS /////

    #region Player Controls
    [Header("Controls")]
    public KeyCode Sprint;    
    public float walkSpeed = 6.0f;
    public float jumpSpeed = 10.0f;
    public float gravity = 30.0f;
    public float doubleJumpSpeed = 16.0f;
    public float wallXJumpAmount = 0.5f;
    public float wallYJumpAmount = 0.5f;
    public float wallJumpWaitTime = 0.5f;
    public float wallRunAmount = 2.0f;
    public float wallRunWaitTime = 0.25f;
    public float slopeSlideSpeed = 10.0f;
    public float glideAmount = 2.0f;
    public float glideTimer = 1.0f;
    public float knockBack = 5.0f;
    public LayerMask groundLayerMask; //Layermask for player slope sliding and various other stuff...
    public LayerMask enemyLayerMask;
    #endregion

    #region Player Ability Toggles
    [Header("Ability Toggles")]
    public bool canDoubleJump = false;
    public bool canWallJump = false;
    public bool canJumpAfterWallJump = false;
    public bool canWallRun = false;
    public bool canRunAfterWallJump = false;
    public bool canSlopeSlide = false;
    public bool canGlide = false;
    public bool canDuck = false;

    #endregion

   
    #region Player States
    [Header("States")]
    /* These are currently public for debugging purposes, 
        * but you can make them private if they're causing 
        * too much clutter in the inspector! Just
        * delete the 'public' keyword.*/
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;
    public bool doubleJumped;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool isGliding;
    public bool isDucking;
    public bool isCreeping;
    public bool enemyContact = false;    
    

    public enum GroundType
    {
        None,
        LevelGeo,
        OneWayPlatforms,
        MovingPlatforms,
        CollapsablePlatform,
        JumpPads
    }
    #endregion

    ///// PRIVATE FIELDS /////

    #region Private Fields
    private CharacterController2D.CharacterCollisionState2D flags;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _lastJumpWasLeft;
    private float _slopeAngle;
    private Vector3 _slopeGradient = Vector3.zero;
    private bool _startGlide;
    private float _currentGlideTimer;
    private BoxCollider2D _boxCollider;
    private Vector2 _originalBoxColliderSize;
    private Vector3 _frontTopCorner;
    private Vector3 _backTopCorner;
    private Vector3 _frontBottomCorner;
    private Vector3 _backBottomCorner;
    private GroundType _groundType;
    private GameObject _tempOneWayPlatform;
    private GameObject _tempMovingPlatform;
    private Vector3 _movingPlatformVelocity = Vector3.zero;
    private float _jumpPadForce = 0;
    #endregion

    ///// NON-SERIALIZED PUBLIC FIELDS /////
    [HideInInspector]
    public CharacterController2D _characterController;


    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
        _boxCollider = GetComponent<BoxCollider2D>();

        _currentGlideTimer = glideTimer;
        _originalBoxColliderSize = _boxCollider.size;
    }

    void Update()
    {
        
        if (!wallJumped)
        {
            _moveDirection.x = Input.GetAxisRaw("Horizontal"); //Add acceleration/deceleration
            _moveDirection.x *= walkSpeed;
        }

        // Left & Right flip and housekeeping
        if (_moveDirection.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            isFacingRight = false;
        }
        else if (_moveDirection.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isFacingRight = true;
        }

        //_moveDirection.y = 2f;
        if (enemyContact)
        {
            KnockBack();
            StartCoroutine(KnockBackDelay());
        }

        // Jumping stuff
        if (isGrounded) 
        {
            _moveDirection.y = 0;
            isJumping = false;
            doubleJumped = false;
            _currentGlideTimer = glideTimer;
            _movingPlatformVelocity = Vector3.zero;

            if (isSlopeSliding)
            {
                _moveDirection = new Vector3(_slopeGradient.x * slopeSlideSpeed, -_slopeGradient.y * slopeSlideSpeed, 0.0f);
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (isDucking && _groundType.Equals(GroundType.OneWayPlatforms))
                {
                    StartCoroutine(DisableOneWayPlatform());
                }
                else
                {
                    _moveDirection.y = jumpSpeed;
                    isJumping = true;
                    isWallRunning = true;
                    StartCoroutine(WallJumpDelay());
                }
            }

            //Jump Pad stuff
            if (_groundType.Equals(GroundType.JumpPads))
            {
                _moveDirection.y += _jumpPadForce;
                _jumpPadForce = 0; //sanity check
            }
        }
        else // Player is in the air
        {
            if (Input.GetButtonUp("Jump"))
            {
                if (_moveDirection.y > 0)
                {
                    _moveDirection.y *= 0.5f;
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (canDoubleJump)
                {
                    if (!doubleJumped)
                    {
                        _moveDirection.y = doubleJumpSpeed;
                        doubleJumped = true;
                    }
                }
            }
        }        

        // Gravity and Gliding stuff
        if (canGlide && Input.GetAxis("Vertical") > 0.5f && _characterController.velocity.y < 0.2f)
        {
            if (_currentGlideTimer > 0)
            {
                isGliding = true;
                if (_startGlide)
                {
                    _moveDirection.y = 0;
                    _startGlide = false;
                }
                _moveDirection.y -= glideAmount * Time.deltaTime;
                _currentGlideTimer -= Time.deltaTime;
            }
            else 
            {
                isGliding = false;
                _moveDirection.y -= gravity * Time.deltaTime;
            }
        }
        else
        {
            isGliding = false;
            _startGlide = true;
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        #region Moving Platform Adjustments
        //Moving Platform Adjustments
        if (_tempMovingPlatform && _groundType.Equals(GroundType.MovingPlatforms))
        {
            _movingPlatformVelocity = _tempMovingPlatform.GetComponent<MovingPlatform>().difference;
        }

        if (!isGrounded && !isGliding)
        {
            if (_moveDirection.x > 0 && _movingPlatformVelocity.x > 0)
            {
                _moveDirection.x += (_movingPlatformVelocity.x * 0.5f);
            }
            else if (_moveDirection.x < 0 && _movingPlatformVelocity.x < 0)
            {
                _moveDirection.x += (_movingPlatformVelocity.x * 0.5f);
            }
        }
        #endregion

        //Update move and collision state
        _characterController.move(_moveDirection * Time.deltaTime);
        flags = _characterController.collisionState; //Test collision states from 'CharacterController2D'

        // Set Grounded state
        isGrounded = flags.below;
        GetGroundType(); //Calling a function further on in the code
     

        // Ducking and creeping stuff
        _frontTopCorner = new Vector3(transform.position.x + (_boxCollider.size.x * 0.5f), transform.position.y + (_boxCollider.size.y * 0.5f), 0);
        _backTopCorner = new Vector3(transform.position.x - (_boxCollider.size.x * 0.5f), transform.position.y + (_boxCollider.size.y * 0.5f), 0);

        RaycastHit2D hitFrontCeiling = Physics2D.Raycast(_frontTopCorner, Vector2.up, 2.0f, groundLayerMask);
        RaycastHit2D hitBackCeiling = Physics2D.Raycast(_backTopCorner, Vector2.up, 2.0f, groundLayerMask);

        if (Input.GetAxisRaw("Vertical") < 0 && _moveDirection.x == 0)
        {
            if (!isDucking && !isCreeping)
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, _originalBoxColliderSize.y * 0.5f);
                transform.position = new Vector3(transform.position.x, transform.position.y - (_originalBoxColliderSize.y * 0.25f), 0);
                _characterController.recalculateDistanceBetweenRays();
            }

            isDucking = true;
            isCreeping = false;
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && (_moveDirection.x < 0 || _moveDirection.x > 0))
        {
            if (!isDucking && !isCreeping)
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, _originalBoxColliderSize.y * 0.5f);
                transform.position = new Vector3(transform.position.x, transform.position.y - (_originalBoxColliderSize.y * 0.25f), 0);
                _characterController.recalculateDistanceBetweenRays();
            }

            isDucking = false;
            isCreeping = true;
        }
        else
        {
            if (!hitFrontCeiling.collider && !hitBackCeiling.collider && (isDucking || isCreeping))
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, _originalBoxColliderSize.y);
                transform.position = new Vector3(transform.position.x, transform.position.y + (_originalBoxColliderSize.y * 0.25f), 0);
                _characterController.recalculateDistanceBetweenRays();

                isDucking = false;
                isCreeping = false;
            }
        }

        if (flags.above)
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        // Wall Jumping Stuff
        if (flags.left || flags.right)
        {
            if (canWallRun)
            {
                if (Input.GetAxis("Vertical") > 0 && isWallRunning)
                {
                    _moveDirection.y = jumpSpeed / wallRunAmount;
                    StartCoroutine(WallRunWaiter());
                }
            }
            if (canWallJump) 
            {
                if (Input.GetButtonDown("Jump") && !wallJumped && !isGrounded)
                {
                    
                    if (_moveDirection.x < 0)
                    {
                        _moveDirection.x = jumpSpeed * wallXJumpAmount;
                        _moveDirection.y = jumpSpeed * wallYJumpAmount;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        _lastJumpWasLeft = false;
                    }
                    else if (_moveDirection.x > 0)
                    {
                        _moveDirection.x = -jumpSpeed * wallXJumpAmount;
                        _moveDirection.y = jumpSpeed * wallYJumpAmount;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        _lastJumpWasLeft = true;
                    }
                    

                    StartCoroutine(WallJumpWaiter());

                    if (canJumpAfterWallJump)
                    {
                        doubleJumped = false;
                    }
                }
            }
        }
        else
        {
            if (canRunAfterWallJump)
            {
                StopCoroutine(WallRunWaiter());
                isWallRunning = true;
            }
        }

        if (flags.below) // Jumping on enemies
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, 2.0f, enemyLayerMask);
            if (hit.collider != null)
            { 
                if (hit.collider.tag == "Enemy")
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        } 

        if (Input.GetKey(Sprint))
        {
            walkSpeed = 10f;
        }
        else
        {
            walkSpeed = 5.0f;
        }

        
      } 


    ///// CUSTOM FUNCTIONS /////

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            enemyContact = true;
        }
    }

    private void GetGroundType()
    {
        // Check ground angle for slope sliding
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, 2.0f, groundLayerMask);

        if (!hit.collider)
        {
            _frontBottomCorner = new Vector3(transform.position.x + _boxCollider.size.x * 0.5f, transform.position.y, 0);
            _backBottomCorner = new Vector3(transform.position.x - _boxCollider.size.x * 0.5f, transform.position.y, 0);

            RaycastHit2D hitFrontFloor = Physics2D.Raycast(_frontBottomCorner, -Vector2.up, 2.0f, groundLayerMask);
            RaycastHit2D hitBackFloor = Physics2D.Raycast(_backBottomCorner, -Vector2.up, 2.0f, groundLayerMask);

            if (hitFrontFloor.collider && !hitBackFloor.collider)
            {
                hit = hitFrontFloor;
            }
            else if (!hitFrontFloor.collider && hitBackFloor.collider)
            {
                hit = hitBackFloor;
            }

        }

        if (hit)
        {
            if (canSlopeSlide)
            {
                _slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                _slopeGradient = hit.normal;

                if (_slopeAngle > _characterController.slopeLimit)
                {
                    isSlopeSliding = true;
                }
                else
                {
                    isSlopeSliding = false;
                }
            }

            string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
            if (layerName == "OneWayPlatforms")
            {
                _groundType = GroundType.OneWayPlatforms;
                if (!_tempOneWayPlatform)
                {
                    _tempOneWayPlatform = hit.transform.gameObject;
                }
            }
            else if (layerName == "LevelGeo")
            {
                _groundType = GroundType.LevelGeo;
            }
            else if (layerName == "MovingPlatforms")
            {
                _groundType = GroundType.MovingPlatforms;
                if (!_tempMovingPlatform)
                {
                    _tempMovingPlatform = hit.transform.gameObject;
                    transform.SetParent(hit.transform);
                }
            }
            else if (layerName == "CollapsablePlatforms")
            {
                _groundType = GroundType.CollapsablePlatform;
                hit.transform.gameObject.GetComponent<CollapsablePlatform>().CollapsePlatform();
            }
            else if (layerName == "JumpPads")
            {
                _groundType = GroundType.JumpPads;
                _jumpPadForce = hit.transform.gameObject.GetComponent<JumpPad>().jumpPadForce;
            }
        }
        else
        {
            _groundType = GroundType.None;
        }

        if (!_groundType.Equals(GroundType.MovingPlatforms))
        {
            if (_tempMovingPlatform)
            {
                transform.SetParent(null);
                _tempMovingPlatform = null;
            }
        }
    }


    public void KnockBack()
    {
        if (isFacingRight)
        {
            _moveDirection.x = -knockBack;
        }
        else if (!isFacingRight)
        {
            _moveDirection.x += knockBack;
        }
    }

    #region Co-Routines
    ///// CO-ROUTINES /////

    IEnumerator WallJumpWaiter()
    {
        wallJumped = true;
        yield return new WaitForSeconds(wallJumpWaitTime);
        wallJumped = false;
    }

    IEnumerator WallRunWaiter()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(wallRunWaitTime);
        isWallRunning = false;
    }

    IEnumerator WallJumpDelay()
    {
        wallJumped = true;
        yield return new WaitForSeconds(0.1f);
        wallJumped = false;
    }

    IEnumerator DisableOneWayPlatform()
    {
        if (_tempOneWayPlatform)
        {
            _tempOneWayPlatform.GetComponent<EdgeCollider2D>().enabled = false;
        }
        yield return new WaitForSeconds(0.5f);
        if (_tempOneWayPlatform)
        {
            _tempOneWayPlatform.GetComponent<EdgeCollider2D>().enabled = true;
            _tempOneWayPlatform = null;
        }
    }

    IEnumerator KnockBackDelay()
    {
        yield return new WaitForSeconds(0.25f);
        enemyContact = false;
    }
    #endregion
}
