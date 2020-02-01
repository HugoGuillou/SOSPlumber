using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSounds", menuName = "SOS/Game Sounds")]
public class GameSounds : ScriptableObject
{
    [Header("Music")]
    [SerializeField] private AudioClip _MenuMusic;
    public AudioClip MenuMusic => _MenuMusic;
    [SerializeField] private AudioClip _GameMusic;
    public AudioClip GameMusic => _GameMusic;

    [Header("Action")]
    [SerializeField]
    private AudioClip _SwipeSound;
    public AudioClip SwipeSound => _SwipeSound;
    [SerializeField] private AudioClip _AcceptSound;
    public AudioClip AcceptSound => _AcceptSound;
    [SerializeField] private AudioClip _DeclineSound;
    public AudioClip DeclineSound => _DeclineSound;

    [Header("Signe")]
    [SerializeField]
    private AudioClip _NewCharSound;
    public AudioClip NewCharSound => _NewCharSound;
    [SerializeField] private AudioClip _DingDongSound;
    public AudioClip DingDongSound => _DingDongSound;
    [SerializeField] private AudioClip _TextSound;
    public AudioClip TextSound => _TextSound;
    [SerializeField] private AudioClip _CardFlipSound;
    public AudioClip CardFlipSound => _CardFlipSound;
}
