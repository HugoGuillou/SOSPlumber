using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using SOS.Pools;

[CreateAssetMenu (fileName = "GameSounds", menuName = "SOS/Game Sounds")]
public class GameSounds : ScriptableObject {
	[Serializable]
	public enum SoundType {
		Music,
		Action,
		Signe,
		Feedback,
		Fin,
	}

	public struct Sound {
		public AudioClip Clip { get; private set; }
		public SoundType Type { get; private set; }
		public AudioMixerGroup Group { get; private set; }

		public Sound (AudioClip clip, SoundType type, AudioMixerGroup group) {
			Clip = clip;
			Type = type;
			Group = group;
		}
	}

	[Serializable]
	public class SoundPool : Pool<AudioClip> {
		[SerializeField]
		private AudioMixerGroup _Group;

		[SerializeField]
		private SoundType _Type;

		public new Sound Pick () {
			return new Sound (_Pool[UnityEngine.Random.Range (0, _Pool.Count)], _Type, _Group);
		}
	}

	[Header ("Music")]
	[SerializeField]
	private SoundPool _MenuMusics;
	public Sound MenuMusic => _MenuMusics.Pick ();
	[SerializeField]
	private SoundPool _GameMusics;
	public Sound GameMusic => _GameMusics.Pick ();

	[Header ("Action")]
	[SerializeField]
	private SoundPool _SwipeSounds;
	public Sound SwipeSound => _SwipeSounds.Pick ();
	[SerializeField]
	private SoundPool _AcceptSounds;
	public Sound AcceptSound => _AcceptSounds.Pick ();
	[SerializeField]
	private SoundPool _DeclineSounds;
	public Sound DeclineSound => _DeclineSounds.Pick ();

	[Header ("Signe")]
	[SerializeField]
	private SoundPool _TextSounds;
	public Sound TextSound => _TextSounds.Pick ();
	[SerializeField]
	private SoundPool _NewCharSounds;
	public Sound NewCharSound => _NewCharSounds.Pick ();
	[SerializeField]
	private SoundPool _DingDongSounds;
	public Sound DingDongSound => _DingDongSounds.Pick ();
	[SerializeField]
	private SoundPool _CardFlipSounds;
	public Sound CardFlipSound => _CardFlipSounds.Pick ();

	[Header ("Feedback")]
	[SerializeField]
	private SoundPool _HotnessUpSounds;
	public Sound HotnessUpSound => _HotnessUpSounds.Pick ();
	[SerializeField]
	private SoundPool _HotnessDownSounds;
	public Sound HotnessDownSound => _HotnessDownSounds.Pick ();
	[SerializeField]
	private SoundPool _RepairUpSounds;
	public Sound RepairUpSound => _RepairUpSounds.Pick ();
	[SerializeField]
	private SoundPool _RepairDownSounds;
	public Sound RepairDownSound => _RepairDownSounds.Pick ();

	[Header ("Fin")]
	[SerializeField]
	private SoundPool _KinkySounds;
	public Sound KinkySound => _KinkySounds.Pick ();
	[SerializeField]
	private SoundPool _PerfectSounds;
	public Sound PerfectSound => _PerfectSounds.Pick ();
	[SerializeField]
	private SoundPool _AlmostSounds;
	public Sound AlmostSound => _AlmostSounds.Pick ();
}
