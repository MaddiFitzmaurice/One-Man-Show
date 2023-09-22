using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(EnemySpawn))]
public class EnemySpawnDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Create property fields.
		var indField = property.FindPropertyRelative("enemy_index");
		var dirField = property.FindPropertyRelative("direction");

		var indRect = new Rect(position.x, position.y, 30, position.height);
		var dirRect = new Rect(position.x + 35, position.y, 80, position.height);

		EditorGUI.PropertyField(indRect, indField, GUIContent.none);
		EditorGUI.PropertyField(dirRect, dirField, GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(SpawnEvent))]
public class SpawnEventDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Create property fields.
		var typeField = property.FindPropertyRelative("type");
		var dataField = property.FindPropertyRelative("data");

		var typeRect = new Rect(position.x, position.y, 150, position.height);
		var dataRect = new Rect(position.x + 155, position.y, 100, position.height);

		EditorGUI.PropertyField(typeRect, typeField, GUIContent.none);
		EditorGUI.PropertyField(dataRect, dataField, GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(TrackBeat))]
public class TrackBeatDrawer : PropertyDrawer
{
	public override VisualElement CreatePropertyGUI(SerializedProperty property)
	{
		// Create property container element.
		var container = new VisualElement();

		// Create property fields.
		var beatField = new PropertyField(property.FindPropertyRelative("beat"));
		var enemField = new PropertyField(property.FindPropertyRelative("enemies"));
		var evntField = new PropertyField(property.FindPropertyRelative("events"));

		// Add fields to the container.
		container.Add(beatField);
		container.Add(enemField);
		container.Add(evntField);

		return container;
	}
}