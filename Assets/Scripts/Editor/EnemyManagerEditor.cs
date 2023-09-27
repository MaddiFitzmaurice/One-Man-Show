using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyManager))]
public class EnemyManagerEditor : Editor
{
	void DrawArrow(Vector3 start, Vector3 end)
	{
		if (Vector3.Distance(start, end) < 0.01) return;

		float head_size = 0.5f;
		float angle = 22.5f / 180f * Mathf.PI;

		float rx = Mathf.Cos(angle);
		float ry = Mathf.Sin(angle);

		Vector3 head = (start - end).normalized * head_size;

		Vector3 head1 = end + new Vector3(head.x * rx - head.y * ry, head.x *  ry + head.y * rx);
		Vector3 head2 = end + new Vector3(head.x * rx + head.y * ry, head.x * -ry + head.y * rx);

		Handles.DrawPolyLine(new Vector3[]{ start, end, head1, head2, end });
	}
	
	void OnSceneGUI()
	{
		EnemyManager m = target as EnemyManager;

		if (m == null)
			return;

		EditorGUI.BeginChangeCheck();
		Vector3 newLeftStart = Handles.PositionHandle(m.leftStartPosition, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m, "Changed Left Start Position");
			m.leftStartPosition = newLeftStart;
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newForwardStart = Handles.PositionHandle(m.forwardStartPosition, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m, "Changed Right Start Position");
			m.forwardStartPosition = newForwardStart;
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newRightStart = Handles.PositionHandle(m.rightStartPosition, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m, "Changed Right Start Position");
			m.rightStartPosition = newRightStart;
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newLeftEnd = Handles.PositionHandle(m.leftEndPosition, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m, "Changed Left End Position");
			m.leftEndPosition = newLeftEnd;
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newForwardEnd = Handles.PositionHandle(m.forwardEndPosition, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m, "Changed Right End Position");
			m.forwardEndPosition = newForwardEnd;
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newRightEnd = Handles.PositionHandle(m.rightEndPosition, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m, "Changed Right End Position");
			m.rightEndPosition = newRightEnd;
		}

		Handles.color = Color.green;

		DrawArrow(m.leftStartPosition,  m.leftEndPosition );
		DrawArrow(m.forwardStartPosition, m.forwardEndPosition);
		DrawArrow(m.rightStartPosition, m.rightEndPosition);
	}
}