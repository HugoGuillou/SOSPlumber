using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(fileName = "StatsPool", menuName = "SOS/Stats Pool")]
public class StatsPool : ScriptableObject
{
    private StatsPool _instance;
    public StatsPool instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<StatsPool>("StatsPool");
            return _instance;
        }
    }

    [SerializeField]
    private StringPool _QuotePool;
    public string Quote => _QuotePool.Pick();

    [SerializeField]
    private IntPool _BonusSexAppealPool;
    public float BonusSexAppeal => _BonusSexAppealPool.Pick(70f);
    [SerializeField]
    private IntPool _BonusRepairPool;
    public float BonusRepair => _BonusRepairPool.Pick(5f);
}

[Serializable]
public class StatPool<T>
{
    [SerializeField]
    private List<T> _Pool;

    public T Pick()
    {
        return _Pool[UnityEngine.Random.Range(0, _Pool.Count)];
    }
}

[Serializable]
public class FloatPool : StatPool<float> { }
[Serializable]
public class IntPool : StatPool<int>
{
    public float Pick(float max)
    {
        return base.Pick() / max;
    }
}
[Serializable]
public class StringPool : StatPool<string> { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StatPool<>), true)]
public class StatPoolPropertyDrawer : PropertyDrawer
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
