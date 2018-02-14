using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Binding
{
    public class ViewBinderBase : MonoBehaviour, INotifyPropertyChanged
    {
        [Helper.ReadOnly]
        public UnityEngine.Component Source;
        [ComponentProperty]
        public string BindingProperty;
        [SerializeField]
        private string Path = null;

        public string PropertyName
        {
            get
            {
                string path = Path;

                if (path.IndexOf(".") > -1)
                {
                    // ex) Monster.Name
                    // PropertyName=Monster
                    // SubPropertyName=Name
                    string[] pathArray = path.Split('.');
                    path = pathArray[0];
                    SubPropertyName = pathArray[pathArray.Length - 1];
                }

                return path;
            }
        }

        public string SubPropertyName { set; get; }

        protected object value;
        public object Value
        {
            get
            {
                return PropertyInfo?.GetValue(Source, null);
            }
            set
            {
                object setValue = value;
                if (setValue != null && !string.IsNullOrEmpty(SubPropertyName))
                {
                    Type t = value.GetType();
                    setValue = t.GetProperty(SubPropertyName)?.GetValue(value, null);
                }

                PropertyInfo?.SetValue(Source, setValue, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected PropertyInfo PropertyInfo;

        protected void SetPropertyInfo(string propertyName)
        {
            if (Source != null)
            {
                Type t = Source.GetType();
                PropertyInfo pi = t.GetProperty(propertyName);
                if (pi != null)
                {
                    PropertyInfo = pi;
                }
                else
                {
                    Debug.LogError("Property not found, Property=" + propertyName);
                }
            }
            else
            {
#if UNITY_EDITOR
                Highlighter.Highlight("Hierarchy", gameObject.name);
                EditorGUIUtility.PingObject(gameObject);
                Debug.LogError($"Component not found, gameObject.name={gameObject.name}, Property={propertyName}, Path={Path}");
#endif
            }
        }

        protected virtual void Start()
        {            
            Source = GetComponent<Text>();
            if (Source == null)
            {
                Source = GetComponent<Toggle>();
            }
            if (Source == null)
            {
                Source = GetComponent<InputField>();
            }
            if (Source == null)
            {
                Source = GetComponent<Button>();
            }

            SetPropertyInfo(BindingProperty);
            value = Value;
        }

        protected virtual void OnGUI()
        {
            object updatedValue = Value;

            if (value?.Equals(updatedValue) == false)
            {
                value = updatedValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Path));
            }
        }
    }
}
