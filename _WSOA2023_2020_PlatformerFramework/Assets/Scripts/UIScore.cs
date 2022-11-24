using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    private GameObject _player;
    private int _score;
    public Text _scoreReadout;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _scoreReadout = GetComponent<Text>();
    }

    void Update()
    {
        _score = _player.GetComponent<PlayerStatus>().score;
        _scoreReadout.text = "Score: " + _score;
    }
}
