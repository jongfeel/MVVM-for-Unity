using Assets.Scripts.Binding;
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class View : MonoBehaviour
    {
        [Tooltip("DataContext inherit INotifyPropertyChanged.")]
        public GameObject DataContext;

        private List<PropertyNameValuePair<string, object>> PropertyNameValuePairList = new List<PropertyNameValuePair<string, object>>();
        private List<BindingBase> ViewBindingList = new List<BindingBase>();
        private INotifyPropertyChanged viewModel;

        private void Start()
        {
            if (DataContext != null)
            {
                viewModel = DataContext.GetComponent<INotifyPropertyChanged>();
                if (viewModel != null)
                {
                    GetProperties(viewModel);   
                    viewModel.PropertyChanged += ViewModel_PropertyChanged;
                }
                else
                {
                    Debug.LogError("ViewModel did not inherit INotifyPropertyChanged.");
                }
            }
            else
            {
                Debug.LogError("DataContext is null.");
            }

            FindAllBindingComponents(transform);
        }

        private void OnDestroy()
        {
            if (viewModel != null)
            {
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            for (int i = 0; i < ViewBindingList.Count; i++)
            {
                ViewBindingList[i].PropertyChanged -= ViewBinding_PropertyChanged;
            }

            ViewBindingList.Clear();
        }

        private void GetProperties(object sender)
        {
            #region Check null, primitive, string
            if (sender == null)
            {
                return;
            }
            else
            {
                if (sender.GetType().IsPrimitive || sender.GetType() == typeof(string))
                {
                    return;
                }
            }
            #endregion

            #region Register property name and value
            PropertyInfo[] infos = sender.GetType().GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].DeclaringType == sender.GetType())
                {
                    string name = infos[i].Name;
                    object value = infos[i].GetValue(sender, null);

                    var findItem = PropertyNameValuePairList.Find(item => item.Key == name);
                    if (findItem == null)
                    {
                        PropertyNameValuePairList.Add(new PropertyNameValuePair<string, object>(name, value));
                    }
                    else
                    {
                        findItem.Value = value;
                    }

                    if (value != null)
                    {
                        GetProperties(value);
                    }
                }
            }
            #endregion
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null && e != null)
            {
                PropertyInfo pi = sender.GetType().GetProperty(e.PropertyName);
                if (pi != null)
                {
                    #region Register new object and binding property
                    GetProperties(sender);
                    #endregion

                    #region Get view bindings of property name
                    List<BindingBase> findBindings = ViewBindingList.FindAll(find => find.PropertyName == e.PropertyName);
                    if (findBindings.Count == 0)
                    {
                        var findItem = PropertyNameValuePairList.Find(item => item.Value == sender);
                        if (findItem != null)
                        {
                            findBindings = ViewBindingList.FindAll(find => find.PropertyName == findItem.Key && find.SubPropertyName == e.PropertyName);
                        }
                    }
                    #endregion

                    #region Change view value
                    for (int i = 0; i < findBindings.Count; i++)
                    {
                        object value = pi.GetValue(sender, null);
                        INotifyPropertyChanged inpc = value as INotifyPropertyChanged;
                        if (inpc != null)
                        {
                            inpc.PropertyChanged -= ViewModel_PropertyChanged;
                            inpc.PropertyChanged += ViewModel_PropertyChanged;
                        }
                        findBindings[i].Value = value;
                    }
                    #endregion
                }
                else
                {
                    Debug.LogError("Can not find property, name=" + e.PropertyName);
                }
            }
            else
            {
                Debug.LogError("sender or PropertyChangedEventArgs is null.");
            }
        }

        private void FindAllBindingComponents(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);

                #region Register BindingBase type and PropertyChanged event handler
                BindingBase[] viewBindings = t.GetComponents<BindingBase>();
                if (viewBindings != null)
                {
                    for (int j = 0; j < viewBindings.Length; j++)
                    {
                        viewBindings[j].PropertyChanged += ViewBinding_PropertyChanged;
                        ViewBindingList.Add(viewBindings[j]);
                    }
                }
                #endregion

                FindAllBindingComponents(t);
            }
        }

        private void ViewBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (viewModel != null)
            {
                Type type = viewModel.GetType();
                PropertyInfo pi = type.GetProperty(e.PropertyName);
                if (pi != null)
                {
                    pi.SetValue(viewModel, (sender as BindingBase).Value, null);
                }
                else
                {
                    int pointIndex = e.PropertyName.IndexOf(".");
                    if (pointIndex > -1)
                    {
                        string[] pathArray = e.PropertyName.Split('.');
                        string propertyName = pathArray[0];
                        string subPropertyName = pathArray[pathArray.Length - 1];

                        PropertyInfo subPI = type.GetProperty(propertyName);
                        if (subPI != null)
                        {
                            object propertyValue = subPI.GetValue(viewModel, null);
                            if (propertyValue != null)
                            {
                                PropertyInfo subPI2 = propertyValue.GetType().GetProperty(subPropertyName);
                                if (subPI2 != null)
                                {
                                    subPI2.SetValue(propertyValue, (sender as BindingBase).Value, null);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}