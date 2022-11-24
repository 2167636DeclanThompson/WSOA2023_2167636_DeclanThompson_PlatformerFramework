using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 difference;
    private Vector3 _lastPosition;

    void Start()
    {
        _lastPosition = transform.position;
    }

    void Update()
    {
        difference = transform.position - _lastPosition;
        difference /= Time.deltaTime;
        _lastPosition = transform.position;
    }
}
