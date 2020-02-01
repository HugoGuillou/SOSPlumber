using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Temperature")]
    [Space]

    [SerializeField]
    private float maxTemperature = 100;
    [SerializeField]
    private float minTemperature = 0;
    [SerializeField]
    private float startTemperature = 50;
    [SerializeField]
    private float tempDecreaseSpeed = 1;


    [Header("House Level")]
    [Space]

    [SerializeField]
    private float maxHouseLevel = 100;
    [SerializeField]
    private float minHouseLevel = 0;
    [SerializeField]
    private float startHouseLevel = 50;


    [Header("Player levels")]
    [Space]

    [SerializeField]
    private float tempLevel;
    [SerializeField]
    private float houseLevel;

    // Start is called before the first frame update
    void Start()
    {
        tempLevel = startTemperature;
        houseLevel= startHouseLevel;
    }

    // Update is called once per frame
    void Update()
    {

        houseLevel -= tempDecreaseSpeed * Time.deltaTime;

    }
}
