using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SpawnBeat))]
public class EnemySpawnDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Create property fields.
		var beaField = property.FindPropertyRelative("beat");
		var typField = property.FindPropertyRelative("type");
		var dirField = property.FindPropertyRelative("direction");

		var beaRect = new Rect(position.x,       position.y,  50, position.height);
		var indRect = new Rect(position.x +  55, position.y, 120, position.height);
		var dirRect = new Rect(position.x + 180, position.y,  80, position.height);

		EditorGUI.PropertyField(beaRect, beaField, GUIContent.none);
		EditorGUI.PropertyField(indRect, typField, GUIContent.none);
		EditorGUI.PropertyField(dirRect, dirField, GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(EventBeat))]
public class SpawnEventDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Create property fields.
		var beatField = property.FindPropertyRelative("beat");
		var typeField = property.FindPropertyRelative("type");
		var dataField = property.FindPropertyRelative("data");

		var beatRect = new Rect(position.x,       position.y,  50, position.height);
		var typeRect = new Rect(position.x +  55, position.y, 120, position.height);
		var dataRect = new Rect(position.x + 180, position.y,  80, position.height);

		EditorGUI.PropertyField(beatRect, beatField, GUIContent.none);
		EditorGUI.PropertyField(typeRect, typeField, GUIContent.none);
		EditorGUI.PropertyField(dataRect, dataField, GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}

/*[CustomPropertyDrawer(typeof(TrackBeat))]
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
}*/