using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject Door;
    public Animator anim;
    public bool Button;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Button = false;
    }

    private void Update()
    {
        DoorAnimation();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            Button = true;
            Destroy(Door);
        }
    }

    public void DoorAnimation()
    {
        if (Button == true)
        {
            anim.SetInteger("Button", 1);
        }
    }
}
