using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    public delegate void StatsEvent(StatType type, float value, float oldValue);

    [Serializable]
    public enum StatType
    {
        Chimney = 0x00,
        Plumbing = 0x01,
        Kitchen = 0x02,
        Boiler = 0x03,
        Hotness = 0x04,
    }

    [Serializable]
    public struct Stat
    {
        [SerializeField]
        private float _StartValue;
        public float StartValue { get { return _StartValue; } }

        [SerializeField, HideInInspector]
        private StatType _Type;
        public StatType Type { get { return _Type; } }

        private float _Value;
        public float Value
        {
            get { return _Value; }
            set
            {
                value = Mathf.Clamp(value, 0f, 1f);
                if (!Mathf.Approximately(value, _Value))
                {
                    OnStatChanged?.Invoke(_Type, value, _Value);
                    _Value = value;
                }
            }
        }

        public Stat(float start, StatType type)
        {
            _StartValue = Mathf.Clamp(start, 0f, 1f);
            _Value = _StartValue;
            _Type = type;
        }
    }

    [Header("Temperature")]
    [Space]

    [SerializeField]
    private Stat temperatureStat = new Stat(0.5f, StatType.Hotness);
    public static Stat TemperatureStat => instance.kitchenStat;

    [Header("Chimney Level")]
    [Space]

    [SerializeField]
    private Stat chimneyStat = new Stat(0.5f, StatType.Chimney);
    public static Stat ChimneyStat => instance.kitchenStat;

    [Header("Plumbing Level")]
    [Space]

    [SerializeField]
    private Stat plumbingStat = new Stat(0.5f, StatType.Plumbing);
    public static Stat PlumbingStat => instance.kitchenStat;

    [Header("Kitchen Level")]
    [Space]

    [SerializeField]
    private Stat kitchenStat = new Stat(0.5f, StatType.Kitchen);
    public static Stat KitchenStat => instance.kitchenStat;

    [Header("Boiler Level")]
    [Space]

    [SerializeField]
    private Stat boilerStat = new Stat(0.5f, StatType.Boiler);
    public static Stat BoilerStat => instance.kitchenStat;

    [Header("Player levels")]
    [Space]

    public static StatsEvent OnStatChanged;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        Card.OnCardAccepted += CardAccepted;
    }

    private void OnDisable()
    {
        Card.OnCardAccepted -= CardAccepted;
    }

    void CardAccepted(Card card)
    {

    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            temperatureStat.Value += ShiftedValue(0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            chimneyStat.Value += ShiftedValue(0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            plumbingStat.Value += ShiftedValue(0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            kitchenStat.Value += ShiftedValue(0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            boilerStat.Value += ShiftedValue(0.1f);
        }
    }

    float ShiftedValue(float value)
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? -value : value;
    }
}
