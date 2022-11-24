using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject player;
    public GameObject Menu;
    public KeyCode Escape;
    void Start()
    {
        if (player = null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        if (Input.GetKey(Escape))
        {
            Menu.SetActive(true);
        }
    }
}
