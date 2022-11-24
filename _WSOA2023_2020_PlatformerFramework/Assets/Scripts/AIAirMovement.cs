using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAirMovement : MonoBehaviour
{
    public enum AirMovementState
    { 
        Stop,
        Dash,
        Float,
        MoveTowards,
        Move
    }

    public enum CollisionBehaviour
    { 
        None,
        Rebound,
        Fall,
        Explode,
        Disappear
    }

    //Generic 
    #region General Controls
    public AirMovementState airMovementState;
    public CollisionBehaviour collisionBehaviour;

    public bool usePhysics = true;
    public float thrust = 10.0f;
    public float rotationSpeed = 10.0f;

    public GameObject explosionEffect;
    public bool alwaysUp = true;
    public float attackRange = 5.0f;
    #endregion

    //Move Towards
    #region Move Towards
    public Transform target;
    public bool autoTargetPlayer = true;
    #endregion

    //Dash
    #region Dash
    public LayerMask playerAndObstacles;
    public float dashAmount = 2.0f;
    public float dashPause = 1.0f;
    public float dashDuration = 1.0f;
    #endregion

    //Float (Do NOT use Physics)
    #region Float
    public float floatUpTime = 2.0f;
    public float floatDownTime = 4.0f;
    #endregion

    ////// PRIVATE FIELDS //////
    private Rigidbody2D _rb;
    private bool _tracking = true;
    private float _size;
    private bool _floatUp;
    private float _floatTimer;
    private bool _moving = true;
    private SpriteRenderer _spriteRenderer;


    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();

        Transform enemyPos = transform;

        if (autoTargetPlayer)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else 
        {
            target = null;
        }
        _size = GetComponent<CircleCollider2D>().radius;
        _floatTimer = floatUpTime;
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (distanceToPlayer <= attackRange)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (target && alwaysUp)
        {
            Vector2 direction = transform.position - target.position;
            if (direction.x > 0)
            {
                _spriteRenderer.flipY = false;
            }
            else 
            {
                _spriteRenderer.flipY = false;
            }
        }

        if (airMovementState.Equals(AirMovementState.Stop))
        {

            if (_moving)
            {
                _rb.velocity = new Vector2(0, 0);
                _rb.angularVelocity = 0f;
                _moving = false;
            }
        }
        else if (airMovementState.Equals(AirMovementState.Dash))
        {
            if (_tracking)
            {
                LookAt2D(target);
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * (_size * 0.5f), 100f, playerAndObstacles);
            if (hit)
            {
                if (hit.collider.tag == "Player" && _tracking)
                {
                    StartCoroutine(Dash());
                }
            }
        }
        else if (airMovementState.Equals(AirMovementState.Float))
        {
            _rb.gravityScale = 1;
            if (_floatUp)
            {
                Move(Vector3.up);
            }

            _floatTimer -= Time.deltaTime;

            if (_floatTimer < 0)
            {
                if (_floatUp)
                {
                    _floatUp = false;
                    _floatTimer = floatDownTime;
                }
                else
                {
                    _floatUp = true;
                    _floatTimer = floatUpTime;
                }
            }
        }
        else if (airMovementState.Equals(AirMovementState.MoveTowards))
        {
            MoveTowards(target);
        }
        else if (airMovementState.Equals(AirMovementState.Move))
        {
            Move(transform.right);
        }
    }

    public void Move(Vector3 moveDirection)
    {
        if (usePhysics)
        {
            _rb.AddForce(moveDirection * thrust);
        }
        else 
        {
            //_rb.velocity = new Vector2(0, 0);
            //_rb.angularVelocity = 0f;
            _rb.MovePosition(transform.position + (moveDirection * thrust) * Time.deltaTime);
        }
    }

    public void MoveTowards(Transform target)
    {
        LookAt2D(target);
        Move(transform.right);
    }

    private void LookAt2D(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collisionBehaviour.Equals(CollisionBehaviour.None))
        {
            return;
        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Rebound))
        {
            Vector2 reflectedPosition = Vector2.Reflect(transform.right, collision.contacts[0].normal);
            _rb.velocity = reflectedPosition.normalized * thrust;
            Vector3 direction = _rb.velocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rb.MoveRotation(angle);
            _rb.angularVelocity = 0;
        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Fall))
        {
            airMovementState = AirMovementState.Stop;
            _rb.gravityScale = 9.81f;

        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Explode))
        {
            if (collision.gameObject.tag == "Player") //Remove this branch statement if you want this behaviour for all collisions
            {
                Debug.Log(collision.gameObject.tag);
                Instantiate(explosionEffect, transform.position, transform.rotation); //Instantiate explosion effect, etc. here!
                Destroy(gameObject);
            }
        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Disappear))
        {
            if (collision.gameObject.tag == "Player") //Remove this branch statement if you want this behaviour for all collisions
                Destroy(gameObject);
        }

    }



    ///// CO-ROUTINES /////

    IEnumerator Dash()
    {
        _tracking = false;
        yield return new WaitForSeconds(dashPause);
        _rb.AddForce(transform.right * dashAmount, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashDuration);
        _rb.velocity = new Vector2(0, 0);
        _rb.angularVelocity = 0;
        _tracking = true;
    }


}
