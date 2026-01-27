using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovement))]
public class PlayerMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawGreenHeader("References");
        DrawProperty("playerCamera");
        DrawProperty("animator");
        DrawProperty("visualRoot");

        DrawGreenHeader("Movement");
        DrawProperty("walkSpeed");
        DrawProperty("runSpeed");
        DrawProperty("crouchSpeed");
        DrawProperty("jumpPower");
        DrawProperty("gravity");

        DrawGreenHeader("Crouch");
        DrawProperty("defaultHeight");
        DrawProperty("crouchHeight");

        DrawGreenHeader("Mouse Look");
        DrawProperty("lookSpeed");
        DrawProperty("lookXLimit");
        DrawProperty("invertLookY");

        DrawGreenHeader("Camera Crouch");
        DrawProperty("standingCameraHeight");
        DrawProperty("crouchCameraHeight");
        DrawProperty("cameraCrouchSpeed");

        serializedObject.ApplyModifiedProperties();
    }

    void DrawGreenHeader(string text)
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = Color.green;

        GUILayout.Space(8);
        GUILayout.Label(text, style);
    }

    void DrawProperty(string name)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), true);
    }
}
