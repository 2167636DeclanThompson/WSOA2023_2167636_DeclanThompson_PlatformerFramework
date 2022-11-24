using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorReset01 : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void DoorResetToBase()
    {
        anim.SetInteger("AnimState", 0);
    }
}
