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
        private List<ViewBinderBase> ViewBindingList = new List<ViewBinderBase>();
        private INotifyPropertyChanged[] viewModels;

        private void Start()
        {
            viewModels = DataContext?.GetComponents<INotifyPropertyChanged>();
            foreach (var viewModel in viewModels)
            {
                GetProperties(viewModel);
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
            
            FindAllBindingComponents(transform);
        }

        private void OnDestroy()
        {
            if (viewModels != null)
            {
                foreach (var viewModel in viewModels)
                {
                    viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
            }

            foreach (var viewBinding in ViewBindingList)
            {
                viewBinding.PropertyChanged -= ViewBinding_PropertyChanged;
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
            foreach (var propertyInfo in infos)
            {
                if (propertyInfo.DeclaringType == sender.GetType())
                {
                    string name = propertyInfo.Name;
                    object value = propertyInfo.GetValue(sender, null);

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
                    List<ViewBinderBase> findBindings = ViewBindingList.FindAll(find => find.PropertyName == e.PropertyName);
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
                    foreach (var findBinding in findBindings)
                    {
                        object value = pi.GetValue(sender, null);
                        INotifyPropertyChanged inpc = value as INotifyPropertyChanged;
                        if (inpc != null)
                        {
                            inpc.PropertyChanged -= ViewModel_PropertyChanged;
                            inpc.PropertyChanged += ViewModel_PropertyChanged;
                        }
                        findBinding.Value = value;
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
            foreach (Transform childTransform in transform)
            {
                #region Register BindingBase type and PropertyChanged event handler
                ViewBinderBase[] viewBindings = childTransform.GetComponents<ViewBinderBase>();
                foreach (var viewBinding in viewBindings)
                {
                    viewBinding.PropertyChanged += ViewBinding_PropertyChanged;
                    ViewBindingList.Add(viewBinding);
                }
                #endregion

                FindAllBindingComponents(childTransform);
            }
        }

        private void ViewBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var viewModel in viewModels)
            {
                Type type = viewModel.GetType();
                PropertyInfo pi = type.GetProperty(e.PropertyName);
                if (pi != null)
                {
                    pi.SetValue(viewModel, (sender as ViewBinderBase).Value, null);
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
                            PropertyInfo subPI2 = propertyValue?.GetType().GetProperty(subPropertyName);
                            subPI2?.SetValue(propertyValue, (sender as ViewBinderBase).Value, null);
                        }
                    }
                }
            }
        }
    }
}