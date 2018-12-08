using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer {
  private MethodInfo eventMethodInfo = null;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
    Rect buttonRect = new Rect(position.x + (position.width - inspectorButtonAttribute.ButtonWidth) * 0.5f, position.y, inspectorButtonAttribute.ButtonWidth, position.height);
    if (GUI.Button(buttonRect, label.text)) {
      System.Type eventOwnerType = property.serializedObject.targetObject.GetType();
      string eventName = inspectorButtonAttribute.MethodName;

      if (eventMethodInfo == null) {
        eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      }

      if (eventMethodInfo != null) {
        eventMethodInfo.Invoke(property.serializedObject.targetObject, null);
      } else {
        Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
      }
    }
  }
}