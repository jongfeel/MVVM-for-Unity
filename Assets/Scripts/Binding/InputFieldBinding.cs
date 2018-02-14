using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Binding
{
    public class InputFieldBinding : ViewBinderBase
    {
        public InputFieldProperty Property;

        protected override void Start()
        {
            Source = GetComponent<InputField>();
            SetPropertyInfo(Property.ToString());

            base.Start();
        }
    }
}
