using UnityEngine;
using System.Collections;

public class PointCoin : MonoBehaviour
{
    public GameObject Effect;
    public int PointsToAdd = 1;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") // Check to see if it's the player colliding
            return;

        Instantiate(Effect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
