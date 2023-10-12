using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
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
