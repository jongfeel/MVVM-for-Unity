using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Assets.Scripts.Helper
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ComponentPropertyAttribute))]
    public class ComponentPropertyDrawer : PropertyDrawer
    {
        private int index = 0;

        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            MonoBehaviour monoBehaviour = property.serializedObject.targetObject as MonoBehaviour;
            if (monoBehaviour != null)
            {
                List<string> properties = new List<string>();
                properties.AddRange(GetProperties(monoBehaviour, typeof(Selectable)));           // Button, InputField, Toggle
                properties.AddRange(GetProperties(monoBehaviour, typeof(MaskableGraphic)));        // Text

                index = properties.FindIndex(match => match == property.stringValue);

                index = EditorGUI.Popup(position, "Property", index == -1 ? 0 : index, properties.ToArray());

                property.stringValue = properties[index];
            }
        }

        private List<string> GetProperties(MonoBehaviour monoBehaviour, Type type)
        {
            List<string> properties = new List<string>();

            Component component = monoBehaviour.GetComponent(type);
            if (component != null)
            {
                foreach (var item in component.GetType().GetProperties())
                {
                    if (item.CanWrite)
                    {
                        properties.Add(item.Name);
                    }
                }
            }

            return properties;
        }
    }
#endif
}