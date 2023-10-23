using UnityEditor;

[CustomEditor(typeof(Fader))]
public class FaderEditor : Editor
{
	public SerializedProperty delay;
	public SerializedProperty lifetime;
	public SerializedProperty transparency;
	public SerializedProperty destroyObject;
	public SerializedProperty destroyScript;
	public SerializedProperty destroyGroup;

	void OnEnable()
	{
		delay         = serializedObject.FindProperty("delay"        );
		lifetime      = serializedObject.FindProperty("lifetime"     );
		transparency  = serializedObject.FindProperty("transparency" );
		destroyObject = serializedObject.FindProperty("destroyObject");
		destroyScript = serializedObject.FindProperty("destroyScript");
		destroyGroup  = serializedObject.FindProperty("destroyGroup" );
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(delay);
		EditorGUILayout.PropertyField(lifetime);
		EditorGUILayout.PropertyField(transparency);
		EditorGUILayout.PropertyField(destroyObject);

		if(!destroyObject.boolValue)
		{
			EditorGUILayout.PropertyField(destroyScript);

			if (destroyScript.boolValue)
			{
				EditorGUILayout.PropertyField(destroyGroup);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}