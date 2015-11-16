using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class WidgetIdentity{
	[SerializeField] private string name;
	public int id;
	public Vector3 flagCoordinate;
}


#if UNITY_EDITOR
[CustomPropertyDrawer (typeof(WidgetIdentity))]
public class WidgetIdentityDrawer : PropertyDrawer {
	
	int propHeight = 0;
	
	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) {
		SerializedProperty name = prop.FindPropertyRelative ("name");
		SerializedProperty id = prop.FindPropertyRelative ("id");
		SerializedProperty coordinate = prop.FindPropertyRelative ("flagCoordinate");
		
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		// Set the name of the flag (only for inspector purpose)
		EditorGUI.LabelField (new Rect (pos.x, pos.y, 40, pos.height - propHeight), "Name");
		EditorGUI.PropertyField (new Rect (pos.x + 40, pos.y, pos.width / 3 - 20, pos.height - propHeight), name, GUIContent.none);
		
		// Set the id of the flag (int)
		EditorGUI.LabelField (new Rect (pos.x + pos.width / 3 + 40, pos.y, 20, pos.height - propHeight), "ID");
		EditorGUI.PropertyField (new Rect (pos.x + pos.width / 3 + 60, pos.y, 30, pos.height - propHeight), id, GUIContent.none);
		
		// Set the vector3 coordinate ofthe flag
		EditorGUI.PropertyField (new Rect (pos.x + pos.width / 3 + 110, pos.y, pos.width / 3 * 2 - 110, pos.height - propHeight), coordinate, GUIContent.none);
		
		EditorGUI.indentLevel = indent;
	}
	
	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
		return base.GetPropertyHeight (prop, label) + propHeight;
	}
	
}
#endif