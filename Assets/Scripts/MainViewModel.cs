using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using UnityEngine;

namespace Assets.Scripts
{
    public class TestModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int value1;
        public int Value1
        {
            get
            {
                return value1;
            }
            set
            {
                if (value1 != value)
                {
                    value1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value1"));
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
    }

    public class MainViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string testText;
        public string TestText
        {
            get
            {
                return testText;
            }
            set
            {
                if (testText != value)
                {
                    testText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TestText"));
                }
            }
        }

        private string currentDateTime;
        public string CurrentDateTime
        {
            get
            {
                return currentDateTime;
            }
            set
            {
                if (currentDateTime != value)
                {
                    currentDateTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentDateTime"));
                }
            }
        }

        private UnityEngine.UI.Button.ButtonClickedEvent currnetDateTimeClick;
        public UnityEngine.UI.Button.ButtonClickedEvent CurrnetDateTimeClick
        {
            get
            {
                return currnetDateTimeClick;
            }
            set
            {
                if (currnetDateTimeClick != value)
                {
                    currnetDateTimeClick = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrnetDateTimeClick"));
                }
            }
        }

        private string inputText;
        public string InputText
        {
            get
            {
                return inputText;
            }
            set
            {
                if (inputText != value)
                {
                    inputText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputText"));
                }
            }
        }

        private TestModel test1;
        public TestModel Test1
        {
            get
            {
                return test1;
            }
            set
            {
                if (test1 != value)
                {
                    test1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Test1"));
                }
            }
        }

        private TestModel test2;
        public TestModel Test2
        {
            get
            {
                return test2;
            }
            set
            {
                if (test2 != value)
                {
                    test2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Test2"));
                }
            }
        }

        private bool buttonIsEnabled;
        public bool ButtonIsEnabled
        {
            get
            {
                return buttonIsEnabled;
            }
            set
            {
                if (buttonIsEnabled != value)
                {
                    buttonIsEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonIsEnabled"));
                }
            }
        }

        private bool toggleIsOn;
        public bool ToggleIsOn
        {
            get
            {
                return toggleIsOn;
            }
            set
            {
                if (toggleIsOn != value)
                {
                    toggleIsOn = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ToggleIsOn"));

                    if (toggleIsOn)
                    {
                        ButtonIsEnabled = true;
                    }
                    else
                    {
                        ButtonIsEnabled = false;
                    }
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            Test1 = new TestModel();
            Test2 = new TestModel();

            CurrentDateTime = "현재시간 출력";
            CurrnetDateTimeClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            CurrnetDateTimeClick.AddListener(() =>
            {
                TestText = DateTime.Now.ToString();
                Test1.Name = "야옹 " + DateTime.Now.ToString();
                Test1.Value1 = new System.Random().Next(10, 14);
                Test2.Name = "멍멍";
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }}