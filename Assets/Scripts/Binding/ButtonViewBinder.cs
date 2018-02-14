using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Binding
{
    public class ButtonViewBinder : ViewBinderBase
    {
        public ButtonProperty Property;
        public List<ButtonBinding> Binding;

        protected override void Start()
        {
            Source = GetComponent<Button>();
            SetPropertyInfo(Property.ToString());
            
            base.Start();
        }
    }
}
