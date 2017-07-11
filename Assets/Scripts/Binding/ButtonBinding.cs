﻿using Assets.Scripts.Enums;
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
    public class ButtonBinding : BindingBase
    {
        public ButtonProperty Command;

        protected override void Start()
        {
            Source = GetComponent<Button>();
            SetPropertyInfo(Command.ToString());
            
            base.Start();
        }
    }
}