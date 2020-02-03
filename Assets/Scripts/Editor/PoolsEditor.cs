using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using SOS.Pools;

[CustomPropertyDrawer (typeof (NativeTypePool<>), true)]
public class PoolPropertyDrawer : PropertyDrawer {
	private ReorderableList _List;
	private string _CurrentPropertyName;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		//base.OnGUI(position, property, label);
		InitList (property);
		_List.DoList (position);
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		InitList (property);

		property = property.FindPropertyRelative ("_Pool");
		//Debug.LogFormat("{0} : {1} ({2})", property.name, res, _List.serializedProperty);
		if (property.arraySize <= 0)
			return 68f;
		return 47f + property.arraySize * 18f;
	}

	private void InitList (SerializedProperty property) {
		_CurrentPropertyName = property.displayName;
		property = property.FindPropertyRelative ("_Pool");
		if (_List == null) {
			_List = new ReorderableList (property.serializedObject, property, false, true, true, true) {
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
					float labelWidth = EditorGUIUtility.labelWidth;
					EditorGUIUtility.labelWidth = 20f;
					EditorGUI.PropertyField (rect, _List.serializedProperty.GetArrayElementAtIndex (index), new GUIContent (index.ToString ()));
					EditorGUIUtility.labelWidth = labelWidth;
				},
				elementHeightCallback = (int index) => {
					return EditorGUI.GetPropertyHeight (_List.serializedProperty.GetArrayElementAtIndex (index));
				},
				drawHeaderCallback = (Rect rect) => {
					EditorGUI.LabelField (rect, _CurrentPropertyName);
				}
			};

		}
		_List.serializedProperty = property;
	}
}
