using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    private static void CreateInstance()
    {
        if (instance == null && FindObjectOfType<AudioManager>() == null)
        {
            new GameObject("AudioManager", typeof(AudioManager));
        }
    }

    public static AudioManager instance { get; private set; }

    private static GameSounds _Sounds;
    public static ref readonly GameSounds Sounds => ref _Sounds;

    private static AudioMixer _Mixer;
    private static AudioMixerGroup _MusicGroup;
    private static AudioMixerGroup _ActionGroup;
    private static AudioMixerGroup _SigneGroup;
    private static AudioMixerGroup _FeedbackGroup;
    private static AudioMixerGroup _FinGroup;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (_Mixer == null)
        {
            _Mixer = Resources.Load("AudioMixer") as AudioMixer;
            _MusicGroup = _Mixer.FindMatchingGroups("Music")[0];
            _ActionGroup = _Mixer.FindMatchingGroups("Action")[0];
            _SigneGroup = _Mixer.FindMatchingGroups("Signe")[0];
            _FeedbackGroup = _Mixer.FindMatchingGroups("Feedback")[0];
            _FinGroup = _Mixer.FindMatchingGroups("Fin")[0];
        }
        if (_Sounds == null)
        {
            _Sounds = Resources.Load<GameSounds>("GameSounds");
        }
    }

    public static void PlaySingleShot(GameSounds.Sound sound, GameObject from = null, Action callback = null)
    {
        PlaySound(sound, false, from, callback);
    }

    private static void PlaySound(GameSounds.Sound sound, bool loop, GameObject from = null, Action callback = null)
    {
        if (sound.Clip == null)
            return;

        bool destroy = from == null;
        if (destroy)
        {
            from = new GameObject("Sound_" + sound.Clip.name, typeof(AudioSource));
        }

        AudioSource source = from.GetComponent<AudioSource>();
        if (source == null)
        {
            source = from.AddComponent<AudioSource>();
        }

        AudioMixerGroup group = null;
        switch (sound.Type)
        {
            case GameSounds.SoundType.Music:
                group = _MusicGroup;
                break;
            case GameSounds.SoundType.Action:
                group = _ActionGroup;
                break;
            case GameSounds.SoundType.Signe:
                group = _SigneGroup;
                break;
            case GameSounds.SoundType.Feedback:
                group = _FeedbackGroup;
                break;
            case GameSounds.SoundType.Fin:
                group = _FinGroup;
                break;
        }
        source.outputAudioMixerGroup = group;
        source.clip = sound.Clip;
        source.loop = loop;

        if (loop)
            source.Play();
        else
            instance.StartCoroutine(instance.PlayClip(source, callback, destroy));
    }

    private IEnumerator PlayClip(AudioSource source, Action callback, bool destroy = false)
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
        callback?.Invoke();
        if (destroy)
            Destroy(source.gameObject);
    }
}
