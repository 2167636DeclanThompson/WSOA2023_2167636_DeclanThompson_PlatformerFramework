using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerStatus _playerStatus;

    private Slider _slider;
    void Start()
    {
        _playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        _slider = GetComponent<Slider>();
    }

    void Update()
    {     
        _slider.value = _playerStatus.health;
    }
}
