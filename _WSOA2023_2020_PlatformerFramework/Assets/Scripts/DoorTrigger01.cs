using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger01 : MonoBehaviour
{
    public Animator anim;

    public float doorDelay = 5.0f;


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            anim.SetInteger("AnimState", 1); //Opens Door
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(DoorClose());
        }
    }

    IEnumerator DoorClose()
    {
        yield return new WaitForSeconds(doorDelay);
        anim.SetInteger("AnimState", 2); //Closes Door
    }
}
