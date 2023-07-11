using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChiChien.Core
{
    public class TriggerSystem : Singleton<TriggerSystem>
    {
        public TriggerListener<TriggerEvent> TriggerListener = new TriggerListener<TriggerEvent>();

        //private static object _lock = new object();
        //private static TriggerSystem _instance;
        //public static new TriggerSystem Instance
        //{
        //    get
        //    {
        //        if (!Application.isPlaying)
        //            return null;

        //        lock (_lock)
        //        {
        //            if (_instance == null)
        //            {
        //                TriggerSystem[] objects = FindObjectsOfType<TriggerSystem>();
        //                if (objects.Length == 0)
        //                {
        //                    GameObject go = new GameObject();
        //                    go.name = typeof(DevCommandSingleton).Name;
        //                    _instance = go.AddComponent<TriggerSystem>();
        //                }
        //                else if (objects.Length == 1)
        //                {
        //                    _instance = objects[0];
        //                }
        //                else// if (objects.Length > 1)
        //                {
        //                    Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}.", typeof(DevCommandSingleton).Name, objects.Length);
        //                }
        //            }
        //            return _instance;
        //        }
        //    }
        //}
        //private void Awake()
        //{
        //    this.LocalAwake();
        //}

        protected override void Awake()
        {
            base.Awake();
            if (this.TriggerListener == null)
                this.TriggerListener = new TriggerListener<TriggerEvent>();
        }

        protected override void Start() {}

        public static int Length
        {
            get { return Instance.TriggerListener.TriggerLength; }
        }

        public void Register<T>(T cmd, Action<TriggerEvent> callback) where T : struct
        {
            var command = this.enumToInt<T>(cmd);
            if (command.result)
                this.TriggerListener.AddListener(command.id, callback);
        }

        public void Register(int cmd, Action<TriggerEvent> callback)
        {
            this.TriggerListener.AddListener(cmd, callback);
        }

        public void UnRegister<T>(T cmd, Action<TriggerEvent> callback) where T : struct
        {
            var command = this.enumToInt<T>(cmd);
            if (command.result)
                this.TriggerListener.RemoveListener(command.id, callback);
        }

        public void UnRegister(int cmd, Action<TriggerEvent> callback)
        {
            this.TriggerListener.RemoveListener(cmd, callback);
        }

        public void UnRegisterSeries(int cmd)
        {
            this.TriggerListener.RemoveSeriesListener(cmd);
        }

        public void ClearAll()
        {
            this.TriggerListener.ClearListeners();
        }

        public bool Dispatch(int cmd, params object[] args)
        {
            return this.TriggerListener.Dispatch(cmd, args);
        }

        public bool Dispatch(TriggerCmd cmd, params object[] args)
        {
            return this.TriggerListener.Dispatch((int)cmd, args);
        }

        private (bool result, int id) enumToInt<T>(T cmd)
        {
            int id = -1;
            bool result = false;

            if (typeof(T).IsEnum)
            {
                Type enumType = typeof(T);
                string name = Enum.GetName(enumType, cmd);
                string enumValue = Enum.Format(enumType, Enum.Parse(enumType, name), "d");
                id = int.Parse(enumValue);
                result = true;
                // Debug.Log($"{enumType} : {name} = {id}");
            }
            else
            {
                Debug.LogError($"{cmd} is NOT enum type !!!");
            }

            return (result, id);
        }
        public T ToEnum<T>(string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("EventTag is null !!!");
            }

            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }
    }
}
