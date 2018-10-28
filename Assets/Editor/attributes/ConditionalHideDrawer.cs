using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHideDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    ConditionalHideAttribute condtionalAttribute = (ConditionalHideAttribute)this.attribute;
    bool enabled = GetConditionalResult(condtionalAttribute, property);

    bool wasEnabled = GUI.enabled;
    GUI.enabled = enabled;
    if (!condtionalAttribute.HideInInsepctor || enabled) {
      EditorGUI.PropertyField(position, property, label, true);
    }
    GUI.enabled = wasEnabled;
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    ConditionalHideAttribute condtionalAttribute = (ConditionalHideAttribute)this.attribute;
    bool enabled = GetConditionalResult(condtionalAttribute, property);

    if (!condtionalAttribute.HideInInsepctor || enabled) {
      return EditorGUI.GetPropertyHeight(property, label);
    }
    return -EditorGUIUtility.standardVerticalSpacing;
  }

  private bool GetConditionalResult(ConditionalHideAttribute conditional, SerializedProperty property) {
    SerializedProperty src = property.serializedObject.FindProperty(property.propertyPath.Replace(property.name,
      conditional.ConditionalSource));
    if (src != null) {
      return conditional.Inverse ? !src.boolValue : src.boolValue;
    } else {
      Debug.LogWarning("Attempting to use a ConditionalHide but no matching condition found in object: "
          + conditional.ConditionalSource);
    }
    return !conditional.Inverse;
  }
}