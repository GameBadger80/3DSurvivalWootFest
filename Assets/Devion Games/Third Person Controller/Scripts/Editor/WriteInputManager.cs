using UnityEngine;
using UnityEditor;

namespace DevionGames
{
    [InitializeOnLoad]
    public class WriteInputManager
    {
        static WriteInputManager()
        {
            TryAddAxis("Change Speed", "left shift");
            TryAddAxis("Crouch", "c");
            TryAddAxis("No Control", "left ctrl");
            TryAddAxis("Evade", "left alt");
        }

        private static void TryAddAxis(string name, string key)
        {
            if (AxisDefined(name)) return;

            AddAxis(new InputAxis()
            {
                name = name,
                positiveButton = key,
                gravity = 1000,
                dead = 0.1f,
                sensitivity = 1000f,
                type = AxisType.KeyOrMouseButton,
                axis = 1
            });
        }

        private static bool AxisDefined(string axisName)
        {
            Object inputManager = AssetDatabase.LoadAllAssetsAtPath(
                "ProjectSettings/InputManager.asset")[0];

            if (inputManager == null)
                return false;

            SerializedObject so = new SerializedObject(inputManager);
            SerializedProperty axes = so.FindProperty("m_Axes");

            if (axes == null || !axes.isArray)
                return false;

            for (int i = 0; i < axes.arraySize; i++)
            {
                SerializedProperty axis = axes.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = axis.FindPropertyRelative("m_Name");

                if (nameProp != null && nameProp.stringValue == axisName)
                    return true;
            }

            return false;
        }

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            if (parent == null) return null;
            return parent.FindPropertyRelative(name);
        }

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        }

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap;
            public bool invert;

            public AxisType type;
            public int axis;
            public int joyNum;
        }

        private static void AddAxis(InputAxis axis)
        {
            Object inputManager = AssetDatabase.LoadAllAssetsAtPath(
                "ProjectSettings/InputManager.asset")[0];

            if (inputManager == null)
                return;

            SerializedObject so = new SerializedObject(inputManager);
            SerializedProperty axes = so.FindProperty("m_Axes");

            axes.arraySize++;
            so.ApplyModifiedProperties();

            SerializedProperty axisProp =
                axes.GetArrayElementAtIndex(axes.arraySize - 1);

            GetChildProperty(axisProp, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProp, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProp, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProp, "dead").floatValue = axis.dead;
            GetChildProperty(axisProp, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProp, "type").intValue = (int)axis.type;
            GetChildProperty(axisProp, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProp, "joyNum").intValue = axis.joyNum;

            so.ApplyModifiedProperties();
        }
    }
}
