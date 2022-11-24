using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject projectileDestructionEffect;
    public float projectileSpeed = 10.0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //rb.velocity = new Vector3(projectileSpeed, 0, 0);
        rb.velocity = transform.TransformDirection(Vector3.right * projectileSpeed);
    }

    void OnCollisionEnter2D (Collision2D collider)
    {
        Instantiate(projectileDestructionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
