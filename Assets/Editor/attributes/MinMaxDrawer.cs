using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    MinMaxAttribute minMax = (MinMaxAttribute)this.attribute;

    var minProperty = property.FindPropertyRelative("min");
    var maxProperty = property.FindPropertyRelative("max");

    float minValue = minProperty.intValue;
    float maxValue = maxProperty.intValue;

    EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minMax.MinLimit, minMax.MaxLimit);

    minProperty.intValue = (int)(minValue = Mathf.Round(minValue));
    maxProperty.intValue = (int)(maxValue = Mathf.Round(maxValue));

    GUI.enabled = false;
    position.y += EditorGUIUtility.singleLineHeight;
    EditorGUI.indentLevel++;
    EditorGUI.Vector2IntField(position, "Selected Range", new Vector2Int((int)minValue, (int)maxValue));
    EditorGUI.indentLevel--;
    GUI.enabled = true;
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    return EditorGUIUtility.singleLineHeight * 2;
  }
}