using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CollapsablePlatform : MonoBehaviour
{
    public float platformWaitTime = 1.0f;
    public float platformMass = 10.0f;
    public float timeTillDestruction = 3.0f;

    BoxCollider2D box2d;

    void Start()
    {
        box2d = GetComponent<BoxCollider2D>();
    }
    public void CollapsePlatform()
    {
        StartCoroutine(PlatformWaitTime());
    }

    IEnumerator PlatformWaitTime()
    {
        yield return new WaitForSeconds(platformWaitTime);
        
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.mass = platformMass;
        box2d.enabled = false;

        yield return new WaitForSeconds(timeTillDestruction);
        Destroy(gameObject);
    }
}
