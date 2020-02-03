using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (GameSounds.SoundPool))]
public class SoundPoolPropertyDrawer : PoolPropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		Rect rect = position;
		rect.height = EditorGUIUtility.singleLineHeight - 2f;
		EditorGUI.PropertyField (rect, property.FindPropertyRelative ("_Group"));

		rect.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField (rect, property.FindPropertyRelative ("_Type"));

		position.y += EditorGUIUtility.singleLineHeight * 2f;
		position.height -= EditorGUIUtility.singleLineHeight * 2f;
		base.OnGUI (position, property, label);
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		return base.GetPropertyHeight (property, label) + EditorGUIUtility.singleLineHeight * 2f;
	}
}
