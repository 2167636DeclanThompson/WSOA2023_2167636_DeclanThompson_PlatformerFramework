using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class AIWalkingEnemy : MonoBehaviour
{
    ///// PUBLIC FIELDS /////

    public enum GroundMovementState
    {
        Stop,
        MoveForward,
        Jump,
        Patrol
    }

    [Header("Controls")]
    public GroundMovementState groundMovementState;

    public float moveSpeed;
    public float jumpSpeed = 10.0f;
    public float jumpDelay = 2.0f;
    public float gravity = 15.0f;

    public Transform[] waypoints;

    [Header("States")]
    public bool autoTurn = true;
    public bool jumpForward = true;
    public bool jumpAndWait = true;
    public bool startFacingLeft = true;


    ///// PRIVATE FIELDS /////

    private bool isGrounded;
    private CharacterController2D.CharacterCollisionState2D _flags;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController2D _characterController;
    private bool _isFacingLeft;
    private Transform _currentTarget;
    private int _waypointCounter = 0;

    void Start()
    {
        _characterController = gameObject.GetComponent<CharacterController2D>();
        if (startFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            _isFacingLeft = true;
        }
    }

    void Update()
    {
        if (isGrounded)
        {
            if (groundMovementState.Equals(GroundMovementState.Stop))
            {
                _moveDirection = Vector3.zero;
            }
            else if (groundMovementState.Equals(GroundMovementState.MoveForward))
            {
                

                if (_isFacingLeft)
                {
                    _moveDirection.x = -moveSpeed;
                }
                else 
                {
                    _moveDirection.x = moveSpeed;
                }
            }
            else if (groundMovementState.Equals(GroundMovementState.Jump))
            {
                _moveDirection.y = jumpSpeed;

                if (jumpForward && _isFacingLeft)
                {
                    _moveDirection.x = -moveSpeed;
                }
                else if (jumpForward && !_isFacingLeft)
                {
                    _moveDirection.x = moveSpeed;
                }

                if (jumpAndWait)
                {
                    StartCoroutine(JumpAndWait());
                }
            }
            else if (groundMovementState.Equals(GroundMovementState.Patrol))
            {
                if (!_currentTarget)
                {
                    _currentTarget = waypoints[_waypointCounter];
                }

                Vector3 difference = _currentTarget.position - transform.position;
                float distanceX = Mathf.Abs(difference.x);

                if (distanceX > 0.1f)
                {
                    if (difference.x > 0f)
                    {
                        _moveDirection.x = moveSpeed;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    else if (difference.x < 0f)
                    {
                        _moveDirection.x = -moveSpeed;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                }
                else 
                {
                    StartCoroutine(ArriveAtWaypoint());
                }
            }
        }

        //Apply gravity
        _moveDirection.y -= gravity * Time.deltaTime;

        //Handle movement
        _characterController.move(_moveDirection * Time.deltaTime);
        _flags = _characterController.collisionState;

        //Check state
        isGrounded = _flags.below;

        //Automatically turn when hitting an obstacle
        if (autoTurn)
        {
            if (_flags.left && _isFacingLeft)
            {
                StartCoroutine(Turn());
            }
            else if (_flags.right && !_isFacingLeft)
            {
                StartCoroutine(Turn());
            }
        }
    }

    ///// CUSTOM FUNCTIONS /////

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Triggered");
        if (_flags.above)
        {
            //Debug.Log("On Top");

            if (col.gameObject.tag == "Player")
                Destroy(gameObject);
        } 
    }

    ///// CO-ROUTINES /////

    IEnumerator JumpAndWait()
    {
        groundMovementState = GroundMovementState.Stop;
        yield return new WaitForSeconds(jumpDelay);
        groundMovementState = GroundMovementState.Jump;
    }

    IEnumerator ArriveAtWaypoint()
    {
        groundMovementState = GroundMovementState.Stop;
        yield return new WaitForSeconds(0.5f);
        _waypointCounter++;
        if (_waypointCounter > waypoints.Length -1)
        {
            _waypointCounter = 0;
        }
        _currentTarget = waypoints[_waypointCounter];
        groundMovementState = GroundMovementState.Patrol;
    }

    IEnumerator Turn()
    {
        yield return new WaitForEndOfFrame();
        if (_isFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            _isFacingLeft = false;
        }
        else if (!_isFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            _isFacingLeft = true;
        }

        _moveDirection.x = -_moveDirection.x;
    }
}
