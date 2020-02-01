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


    [Header("Chimney Level")]
    [Space]

    [SerializeField]
    private float maxChimneyLevel = 100;
    [SerializeField]
    private float minChimneyLevel = 0;
    [SerializeField]
    private float startChimneyLevel = 50;

    [Header("Plumbing Level")]
    [Space]

    [SerializeField]
    private float maxPlumbingLevel = 100;
    [SerializeField]
    private float minPlumbingLevel = 0;
    [SerializeField]
    private float startPlumbingLevel = 50;

    [Header("Kitchen Level")]
    [Space]

    [SerializeField]
    private float maxKitchenLevel = 100;
    [SerializeField]
    private float minKitchenLevel = 0;
    [SerializeField]
    private float startKitchenLevel = 50;

    [Header("Boiler Level")]
    [Space]

    [SerializeField]
    private float maxBoilerLevel = 100;
    [SerializeField]
    private float minBoilerLevel = 0;
    [SerializeField]
    private float startBoilerLevel = 50;

    [Header("Player levels")]
    [Space]

    [SerializeField]
    private float tempLevel;
    [SerializeField]
    private float chimneyLevel;
    private float plumbingLevel;
    private float kitchenLevel;
    private float boilerLevel;

    // Start is called before the first frame update
    void Start()
    {
        tempLevel = startTemperature;
        chimneyLevel    = startChimneyLevel;
        plumbingLevel   = startPlumbingLevel;
        kitchenLevel    = kitchenLevel;
        boilerLevel     = boilerLevel;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
