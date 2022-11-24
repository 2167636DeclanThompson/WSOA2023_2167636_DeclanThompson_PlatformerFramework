using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    float xDirection, Speed;
    bool moveUp = true;

    private void Update()
    {
        if (transform.position.y > 0)
        {
            moveUp = true;
        }
        else if (transform.position.y < 0)
        {
            moveUp = false;
        }

        if (moveUp == true)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + Speed * Time.deltaTime); 
        }
        else if (moveUp == false)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - Speed * Time.deltaTime);
        }
    }

}
