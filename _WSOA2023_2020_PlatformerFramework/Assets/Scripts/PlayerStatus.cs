using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Anima2d;

public class PlayerStatus : MonoBehaviour
{
    ///// PUBLIC FIELDS /////

    public int health;
    public int maxHealth = 100;
    public int score = 0;
    public bool isDead;
    public bool isInvulnerable;
    public float invulnerabilityTime = 1.0f;
    public GameObject deathEffect;

    ///// PRIVATE FIELDS /////

    private PlayerController _playerController;
    private BoxCollider2D _playerCollider;
    private Scene _scene;
    void Start()
    {
        _playerController = gameObject.GetComponent<PlayerController>();
        _playerCollider = gameObject.GetComponent<BoxCollider2D>();
        _scene = SceneManager.GetActiveScene();
        health = maxHealth;
    }

    public void AdjustHealth(int amount)
    {
        if (amount < 0) //Take damage
        {
            if (!isInvulnerable)
            {
                health = health + amount;
            }
            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    Die();
                }
            }
            else
            {
                //Instantiate any DAMAGE effects (sounds, particle effects, etc.) here!
                StartCoroutine(Invulnerable(invulnerabilityTime));
            }
        }
        else if (amount > 0) // Heal
        {
            health += amount;
            //Instantiate any HEALING effects (sounds, particle effects, etc.) here!
        }

        if (health < 0)
        {
            health = 0;
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void StartInvulnerability(float time)
    {
        StartCoroutine(Invulnerable(time));
    }

    public void Die()
    {
        // Instantiate the deathEffect here

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        
        _playerController.enabled = false;
        _playerCollider.enabled = false;
        
        StartCoroutine("RestartLevel");
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Coin") // Check to see if it's a coin colliding
        {   
            PointCoin coin = other.GetComponent<PointCoin>();
            score += coin.PointsToAdd;
        }

        
    }



    ///// CO-ROUTINES /////

    private IEnumerator Invulnerable(float time)
    {
        isInvulnerable = true;

         
           /* SpriteMeshInstance[] sprites = gameObject.GetComponentInChildren<SpriteMeshInstance>();
           
           for(int i = 0; i < invulnerabilityTime * 5; i++)
           {
               foreach(SpriteMeshInstance sprite in sprites)
               {
                   sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.25f)
               }
               yield return new WaitForSeconds(0.1f);
               foreach(SpriteMeshInstance sprite in sprites)
               {
                   sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f)
               }
               yield return new WaitForSeconds(0.1f);
           } */
        

        SpriteRenderer playerSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0.25f);
        
        yield return new WaitForSeconds(time);

        playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f);

        isInvulnerable = false;
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        SceneManager.LoadScene(_scene.name);
        isDead = false;
        health = maxHealth;

    }
}
