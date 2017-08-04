using Assets.Scripts.Helper;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Binding
{
    public abstract class BindingBase : MonoBehaviour, INotifyPropertyChanged
    {
        [Helper.ReadOnly]
        public UnityEngine.Component Source;
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
                    PropertyInfo pi = t.GetProperty(SubPropertyName);
                    if (pi != null)
                    {
                        setValue = pi.GetValue(value, null);
                    }
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
                Highlighter.Highlight("Hierarchy", gameObject.name);
                EditorGUIUtility.PingObject(gameObject);
                Debug.LogError($"Component not found, gameObject.name={gameObject.name}, Property={propertyName}, Path={Path}");
            }
        }

        protected virtual void Start()
        {
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
