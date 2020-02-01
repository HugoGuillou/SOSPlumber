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

    public enum SoundType
    {
        Music,
        Action,
        Signe,
        Feedback,
        Fin,
    }

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
    }
    public static void PlaySingleShot(AudioClip clip, SoundType type, GameObject from = null, Action callback = null)
    {
        PlaySound(clip, type, false, from, callback);
    }

    private static void PlaySound(AudioClip clip, SoundType type, bool loop, GameObject from = null, Action callback = null)
    {
        bool destroy = from == null;
        if (destroy)
        {
            from = new GameObject("Sound_" + clip.name, typeof(AudioSource));
        }

        AudioSource source = from.GetComponent<AudioSource>();
        if (source == null)
        {
            source = from.AddComponent<AudioSource>();
        }

        AudioMixerGroup group = null;
        switch (type)
        {
            case SoundType.Music:
                group = _MusicGroup;
                break;
            case SoundType.Action:
                group = _ActionGroup;
                break;
            case SoundType.Signe:
                group = _SigneGroup;
                break;
            case SoundType.Feedback:
                group = _FeedbackGroup;
                break;
            case SoundType.Fin:
                group = _FinGroup;
                break;
        }
        source.outputAudioMixerGroup = group;
        source.clip = clip;
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
