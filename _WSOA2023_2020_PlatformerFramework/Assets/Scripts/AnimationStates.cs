using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates : MonoBehaviour
{
    private PlayerController _playerController;
    private Animator _anim;


    private void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        AnimationController(); //Update animation state
    }



    public void AnimationController()
    {
        if (_playerController.isGrounded &&
            _playerController._characterController.velocity.x == 0.0f) //Set idle animation
        {
            _anim.SetInteger("AnimState", 0);
        }
        else if (_playerController.isGrounded &&
                (_playerController._characterController.velocity.x > 0.1 ||
                _playerController._characterController.velocity.x < 0.1)) //Set walk animation
        {
            _anim.SetInteger("AnimState", 1);
            //Debug.Log("walking");
        }
        else if (!_playerController.isGrounded &&
                _playerController.isJumping) //Set jump animation
        {
            _anim.SetInteger("AnimState", 2);
            //Debug.Log("jumping");
        }

        if (_playerController.isGrounded &&
            _playerController._characterController.velocity.x == 0.0f) //Set idle animation
        {
            _anim.SetInteger("AnimState", 0);
        }
        else if (_playerController.isGrounded &&
                (_playerController._characterController.velocity.x > 0.1 ||
                _playerController._characterController.velocity.x < 0.1)
                && _playerController.walkSpeed > 6)
        {
            _anim.SetInteger("AnimState", 3);

        }

        
    }

    }



