using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
    #region Fields

    // Reference to the attribute on the property.
    private DrawIfAttribute _drawIf;

    // Field that is being compared.
    private SerializedProperty _comparedField;

    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        !ShowMe(property) && _drawIf.DisablingType == DrawIfAttribute.DisablingTypes.DontDraw
            ? 0f
            : base.GetPropertyHeight(property, label);

    /// <summary>
    /// Errors default to showing the property.
    /// </summary>
    private bool ShowMe(SerializedProperty property)
    {
        _drawIf = attribute as DrawIfAttribute;
        // Replace propertyName to the value from the parameter
        string path = property.propertyPath.Contains(".")
            ? System.IO.Path.ChangeExtension(property.propertyPath, _drawIf?.ComparedPropertyName)
            : _drawIf?.ComparedPropertyName;

        _comparedField = property.serializedObject.FindProperty(path);

        if (_comparedField == null)
        {
            Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        // get the value & compare based on types
        switch (_comparedField.type)
        {
            // Possible extend cases to support your own type
            case "bool":
                return _comparedField.boolValue.Equals(_drawIf?.ComparedValue);
            case "Enum":
                return _drawIf != null && _comparedField.enumValueIndex.Equals((int)_drawIf.ComparedValue);
            default:
                Debug.LogError("Error: " + _comparedField.type + " is not supported of " + path);
                return true;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // If the condition is met, simply draw the field.
        if (ShowMe(property))
        {
            EditorGUI.PropertyField(position, property);
        } //...check if the disabling type is read only. If it is, draw it disabled
        else if (_drawIf.DisablingType == DrawIfAttribute.DisablingTypes.ReadOnly)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }
}