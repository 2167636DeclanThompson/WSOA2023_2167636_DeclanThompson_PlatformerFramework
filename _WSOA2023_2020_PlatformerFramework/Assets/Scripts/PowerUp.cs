using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    ///// PUBLIC FIELDS /////
    [Header("Invulnerability Controls")]
    public float duration = 5.0f;

    [Header("Health")]
    public int healthToAdd;

    [Header("Effects")]
    public GameObject powerupEffect;

    public enum PowerUpType
    { 
        Invulnerability,
        Health,
        Ability
    }
    [Header("Types")]
    public PowerUpType powerupType;

    public enum AbilityType
    {
        DoubleJump,
        WallJump,
        WallRun,
        Glide
    }
    public AbilityType abilityType;
    
    ///// PRIVATE FIELDS /////
    private GameObject _player;
    private PlayerStatus _playerStatus;
    private PlayerController _playerController;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerStatus = _player.GetComponent<PlayerStatus>();
        _playerController = _player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (powerupType.Equals(PowerUpType.Invulnerability))
            {
                _playerStatus.StartInvulnerability(duration);
            }
            else if (powerupType.Equals(PowerUpType.Health))
            {
                _playerStatus.AdjustHealth(healthToAdd);
            }
            else if (powerupType.Equals(PowerUpType.Ability))
            {
                if (abilityType.Equals(AbilityType.DoubleJump))
                {
                    _playerController.canDoubleJump = true;
                }
                else if (abilityType.Equals(AbilityType.WallJump))
                {
                    _playerController.canWallJump = true;
                }
                else if (abilityType.Equals(AbilityType.WallRun))
                {
                    _playerController.canWallRun = true;
                }
                else if (abilityType.Equals(AbilityType.Glide))
                {
                    _playerController.canGlide = true;
                }
            }

            Instantiate(powerupEffect, transform.position, Quaternion.identity); //Instantiate your effects here!
            Destroy(gameObject);
        }
    }
    
}
