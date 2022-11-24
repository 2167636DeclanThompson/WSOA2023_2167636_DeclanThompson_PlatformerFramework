using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    public float cameraPosZ = -10.0f;
    public float cameraOffsetX = 1.5f;
    public float cameraOffsetY = 0.5f;

    public float horizontalSpeed = 5.0f;
    public float verticalSpeed = 10.0f;

    private Transform _camTransform;
    private PlayerController _playerController;

    void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        _playerController = player.GetComponent<PlayerController>();

        _camTransform = Camera.main.transform;

        _camTransform.position = new Vector3(
            player.transform.position.x - cameraOffsetX, 
            player.transform.position.y + cameraOffsetY, 
            cameraPosZ
            );

    }

    void Update()
    {
        if (_playerController.isFacingRight)
        {
            _camTransform.position = new Vector3(
                Mathf.Lerp(_camTransform.position.x, player.transform.position.x + cameraOffsetX, horizontalSpeed * Time.deltaTime),
                Mathf.Lerp(_camTransform.position.y, player.transform.position.y + cameraOffsetY, horizontalSpeed * Time.deltaTime),
                cameraPosZ
                );
        }
        else
        {
            _camTransform.position = new Vector3(
               Mathf.Lerp(_camTransform.position.x, player.transform.position.x - cameraOffsetX, horizontalSpeed * Time.deltaTime),
               Mathf.Lerp(_camTransform.position.y, player.transform.position.y + cameraOffsetY, horizontalSpeed * Time.deltaTime),
               cameraPosZ
               );
        }
    }
}
