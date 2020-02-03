using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Player instance { get; private set; }

	public delegate void StatsEvent (StatType type, float value, float oldValue);

	[Serializable]
	public enum StatType {
		Chimney = 0x00,
		Plumbing = 0x01,
		Kitchen = 0x02,
		Boiler = 0x03,
		Hotness = 0x04,
	}

	[Serializable]
	public struct Stat {
		[SerializeField]
		private float _StartValue;
		public float StartValue { get { return _StartValue; } }

		[SerializeField, HideInInspector]
		private StatType _Type;
		public StatType Type { get { return _Type; } internal set { _Type = value; } }

		private float _Value;
		public float Value {
			get { return _Value; }
			set {
				value = Mathf.Clamp (value, 0f, 1f);
				if (!Mathf.Approximately (value, _Value)) {
					Debug.LogFormat ("Stat changed {0} ({1})", _Value, _Type);
					OnStatChanged?.Invoke (_Type, value, _Value);
					_Value = value;
				}
			}
		}

		public Stat (float start, StatType type) {
			_StartValue = Mathf.Clamp (start, 0f, 1f);
			_Value = _StartValue;
			_Type = type;
		}
	}

	[Header ("Temperature")]
	[Space]

	[SerializeField]
	private Stat temperatureStat = new Stat (0.5f, StatType.Hotness);
	public static Stat TemperatureStat => instance.kitchenStat;

	[Header ("Chimney Level")]
	[Space]

	[SerializeField]
	private Stat chimneyStat = new Stat (0.5f, StatType.Chimney);
	public static Stat ChimneyStat => instance.kitchenStat;

	[Header ("Plumbing Level")]
	[Space]

	[SerializeField]
	private Stat plumbingStat = new Stat (0.5f, StatType.Plumbing);
	public static Stat PlumbingStat => instance.kitchenStat;

	[Header ("Kitchen Level")]
	[Space]

	[SerializeField]
	private Stat kitchenStat = new Stat (0.5f, StatType.Kitchen);
	public static Stat KitchenStat => instance.kitchenStat;

	[Header ("Boiler Level")]
	[Space]

	[SerializeField]
	private Stat boilerStat = new Stat (0.5f, StatType.Boiler);
	public static Stat BoilerStat => instance.kitchenStat;

	[Header ("Player levels")]
	[Space]

	public static StatsEvent OnStatChanged;

	private void Awake () {
		instance = this;
		temperatureStat.Type = StatType.Hotness;
		chimneyStat.Type = StatType.Chimney;
		plumbingStat.Type = StatType.Plumbing;
		kitchenStat.Type = StatType.Kitchen;
		boilerStat.Type = StatType.Boiler;
	}

	private void OnEnable () {
		Card.OnCardAccepted += CardAccepted;
	}

	private void OnDisable () {
		Card.OnCardAccepted -= CardAccepted;
	}

	void CardAccepted (Card card) {
		Debug.LogFormat ("{0} : {1} ({2})", card.SexyStat, card.HouseStat, card.HouseType);
		temperatureStat.Value += card.SexyStat;

		switch (card.HouseType) {
			case StatType.Chimney:
				chimneyStat.Value += card.HouseStat;
				break;
			case StatType.Plumbing:
				plumbingStat.Value += card.HouseStat;
				break;
			case StatType.Kitchen:
				kitchenStat.Value += card.HouseStat;
				break;
			case StatType.Boiler:
				boilerStat.Value += card.HouseStat;
				break;
		}

        if(card.HouseStat > 0)
            AudioManager.PlaySingleShot(AudioManager.Sounds.RepairUpSound);
        else if(card.HouseStat < 0)
            AudioManager.PlaySingleShot(AudioManager.Sounds.RepairDownSound);

        if (card.SexyStat > 0)
            AudioManager.PlaySingleShot(AudioManager.Sounds.HotnessUpSound);
        else if (card.HouseStat < 0)
            AudioManager.PlaySingleShot(AudioManager.Sounds.HotnessDownSound);

        if (!Mathf.Approximately (0f, card.HouseStat)) {
			if (!Mathf.Approximately (0f, card.SexyStat)) {
				AudioManager.PlaySingleShot ((card.HouseStat < 0f) ? AudioManager.Sounds.RepairDownSound : AudioManager.Sounds.RepairUpSound, () => {
					AudioManager.PlaySingleShot ((card.SexyStat < 0f) ? AudioManager.Sounds.HotnessDownSound : AudioManager.Sounds.HotnessUpSound);
				});
			}
			else
				AudioManager.PlaySingleShot ((card.HouseStat < 0f) ? AudioManager.Sounds.RepairDownSound : AudioManager.Sounds.RepairUpSound);
		}
		else
		if (!Mathf.Approximately (0f, card.SexyStat))
			AudioManager.PlaySingleShot ((card.SexyStat < 0f) ? AudioManager.Sounds.HotnessDownSound : AudioManager.Sounds.HotnessUpSound);

	}

	float ShiftedValue (float value) {
		return (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) ? -value : value;
	}
}
