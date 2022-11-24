using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    public GameObject target;

    void Update()
    {
        transform.right = target.transform.position - transform.position;
    }
}
