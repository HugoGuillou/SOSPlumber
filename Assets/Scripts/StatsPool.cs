using System;
using System.Collections.Generic;
using UnityEngine;

using SOS.Pools;

[Serializable]
[CreateAssetMenu (fileName = "StatsPool", menuName = "SOS/Stats Pool")]
public class StatsPool : ScriptableObject {
	private static StatsPool _instance;
	public static StatsPool instance {
		get {
			if (_instance == null)
				_instance = Resources.Load<StatsPool> ("StatsPool");
			return _instance;
		}
	}

	[SerializeField]
	private StringPool _QuotePool;
	public string Quote => _QuotePool.PickOnce ();

	public static readonly float MaxSexAppeal = 70f;
	[SerializeField]
	private IntPool _BonusSexAppealPool;
	public float BonusSexAppeal => _BonusSexAppealPool.Pick (MaxSexAppeal);

	public static readonly float MaxRepair = 5f;
	[SerializeField]
	private IntPool _BonusRepairPool;
	public float BonusRepair => _BonusRepairPool.Pick (MaxRepair);
}
