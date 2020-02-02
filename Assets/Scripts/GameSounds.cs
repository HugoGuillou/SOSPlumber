using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(fileName = "GameSounds", menuName = "SOS/Game Sounds")]
public class GameSounds : ScriptableObject
{
    [Serializable]
    public enum SoundType
    {
        Music,
        Action,
        Signe,
        Feedback,
        Fin,
    }

    public struct Sound
    {
        public AudioClip Clip { get; private set; }
        public SoundType Type { get; private set; }

        public Sound(AudioClip clip, SoundType type)
        {
            Clip = clip;
            Type = type;
        }
    }

    [Serializable]
    public struct SoundPool
    {
        [SerializeField]
        private List<AudioClip> _Pool;

        [SerializeField]
        private SoundType _Type;

        public Sound Pick()
        {
            return new Sound(_Pool[UnityEngine.Random.Range(0, _Pool.Count)], _Type);
        }
    }

    [Header("Music")]
    [SerializeField]
    private SoundPool _MenuMusics;
    public Sound MenuMusic => _MenuMusics.Pick();
    [SerializeField]
    private SoundPool _GameMusics;
    public Sound GameMusic => _GameMusics.Pick();

    [Header("Action")]
    [SerializeField]
    private SoundPool _SwipeSounds;
    public Sound SwipeSound => _SwipeSounds.Pick();
    [SerializeField]
    private SoundPool _AcceptSounds;
    public Sound AcceptSound => _AcceptSounds.Pick();
    [SerializeField]
    private SoundPool _DeclineSounds;
    public Sound DeclineSound => _DeclineSounds.Pick();

    [Header("Signe")]
    [SerializeField]
    private SoundPool _TextSounds;
    public Sound TextSound => _TextSounds.Pick();
    [SerializeField]
    private SoundPool _NewCharSounds;
    public Sound NewCharSound => _NewCharSounds.Pick();
    [SerializeField]
    private SoundPool _DingDongSounds;
    public Sound DingDongSound => _DingDongSounds.Pick();
    [SerializeField]
    private SoundPool _CardFlipSounds;
    public Sound CardFlipSound => _CardFlipSounds.Pick();

    [Header("Fin")]
    [SerializeField]
    private SoundPool _KinkySounds;
    public Sound KinkySound => _KinkySounds.Pick();
    [SerializeField]
    private SoundPool _PerfectSounds;
    public Sound PerfectSound => _PerfectSounds.Pick();
    [SerializeField]
    private SoundPool _AlmostSounds;
    public Sound AlmostSound => _AlmostSounds.Pick();
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(GameSounds.SoundPool), true)]
public class SoundPoolPropertyDrawer : PropertyDrawer
{
    private ReorderableList _List;
    private string _CurrentPropertyName;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        InitList(property);
        _List.DoList(position);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        InitList(property);

        property = property.FindPropertyRelative("_Pool");
        //Debug.LogFormat("{0} : {1} ({2})", property.name, res, _List.serializedProperty);
        if (property.arraySize <= 0)
            return 68f;
        return 47f + property.arraySize * 18f;
    }

    private void InitList(SerializedProperty property)
    {
        _CurrentPropertyName = property.displayName;
        property = property.FindPropertyRelative("_Pool");
        if (_List == null)
        {
            _List = new ReorderableList(property.serializedObject, property, false, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 20f;
                    EditorGUI.PropertyField(rect, _List.serializedProperty.GetArrayElementAtIndex(index), new GUIContent(index.ToString()));
                    EditorGUIUtility.labelWidth = labelWidth;
                },
                elementHeightCallback = (int index) =>
                {
                    return EditorGUI.GetPropertyHeight(_List.serializedProperty.GetArrayElementAtIndex(index));
                },
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, _CurrentPropertyName);
                }
            };

        }
        _List.serializedProperty = property;
    }
}
#endif
