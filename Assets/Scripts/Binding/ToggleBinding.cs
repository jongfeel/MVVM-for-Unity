using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.Scripts.Binding
{
    public class ToggleBinding : BindingBase
    {
        public ToggleProperty Property;

        protected override void Start()
        {
            Source = GetComponent<Toggle>();           
            SetPropertyInfo(Property.ToString());

            base.Start();
        }
    }
}
