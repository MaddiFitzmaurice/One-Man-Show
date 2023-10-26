using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MoveBeat))]
public class MoveBeatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Create property fields.
        var beatField = property.FindPropertyRelative("beat");
        var percField = property.FindPropertyRelative("percentage");

		float bw = Mathf.Min(75, position.width / 3f);
        var beatRect = new Rect(position.x, position.y, bw, position.height);
        var percRect = new Rect(position.x + bw + 10, position.y, position.width - (bw + 10), position.height);

        EditorGUI.PropertyField(beatRect, beatField, GUIContent.none);
        EditorGUI.PropertyField(percRect, percField, GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}

[CustomEditor(typeof(Enemy))]
[CanEditMultipleObjects]
public class EnemyEditor : Editor
{

	void OnSceneGUI()
	{
		Enemy e = target as Enemy;

		if (e == null)
			return;

		/*EditorGUI.BeginChangeCheck();

		Vector3 offset = e.transform.InverseTransformPoint(Handles.PositionHandle(e.transform.TransformPoint(e._spawnOffset), Quaternion.identity));

		if (!EditorGUI.EndChangeCheck()) return;

		Undo.RecordObject(e, "Changed death particle offset");
		e._spawnOffset = offset;*/

		Vector3 roffset = e.transform.TransformPoint(e._rightSpawnOffset);
		Vector3 foffset = e.transform.TransformPoint(e._forwardSpawnOffset);

		Handles.DrawLine(roffset + Vector3.up * 0.5f, roffset + Vector3.down * 0.5f);
		Handles.DrawLine(roffset + Vector3.left * 0.5f, roffset + Vector3.right * 0.5f);

		Handles.DrawLine(foffset + Vector3.up * 0.5f, foffset + Vector3.down * 0.5f);
		Handles.DrawLine(foffset + Vector3.left * 0.5f, foffset + Vector3.right * 0.5f);
	}
}
