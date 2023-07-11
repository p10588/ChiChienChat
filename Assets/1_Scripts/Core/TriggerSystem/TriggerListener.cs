using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChiChien.Core
{
	public static class Extensions
	{
		public static bool Exists(this Delegate @delegate, Delegate target)
		{
			bool result = false;
			Delegate[] delegates = @delegate.GetInvocationList();
			result = Array.Find<Delegate>(delegates, d => d == target) != null;
			return result;
		}
	}

	public abstract class EventBase
	{
		public int EventType
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format($"{this.GetType().FullName}, {this.EventType}");
		}
	}

	public class TriggerListener<T> where T : EventBase
	{
		private Dictionary<int, Delegate> _listenerLookup = new Dictionary<int, Delegate>();

		private Delegate this[int type]
		{
			get
			{
				if (!this._listenerLookup.ContainsKey(type))
				{
					this._listenerLookup.Add(type, null);
				}
				return this._listenerLookup[type];
			}
			set { this._listenerLookup[type] = value; }
		}

		public int TriggerLength
		{
			get { return this._listenerLookup.Count; }
		}

		/// <summary>
		/// Dispatch the specified event.
		/// </summary>
		/// <param name="type">Event type.</param>
		/// <param name="args">Event arguments.</param>
		public bool Dispatch(int type, params object[] args)
		{
			return this.Dispatch(type, (T)Activator.CreateInstance(typeof(T), args));
		}

		public bool Dispatch(int type, T evt)
		{
			if (evt == null)
			{
				return false;
			}

			Action<T> listeners = this[type] as Action<T>;
			if (listeners == null)
			{
				Debug.LogError($"Type={type} {(TriggerCmd)type} : WITHOUT ANY register dispatch actions");
				return false;
			}

            try
            {
				evt.EventType = type;
				listeners.Invoke(evt);
				return true;
			}catch(Exception e)
            {
				Debug.LogError($"Type={type} {(TriggerCmd)type}: {e.Message}");
				return false;
            }
			
		}

		public bool HasListener(int type, Action<T> listener)
		{
			Delegate listeners = this[type];
			return (listeners != null) ? listeners.Exists(listener) : false;
		}

		public void AddListener(int type, Action<T> listener)
		{
			Delegate listeners = Delegate.Combine(this[type], listener);
			this[type] = listeners;
			//EventDispatcherProfiler.AddListener(typeof(T).Name, type, listener);
		}

		public void RemoveListener(int type, Action<T> listener)
		{
			if (this.HasListener(type, listener))
			{
				Delegate listeners = Delegate.Remove(this[type], listener);
				this[type] = listeners;
				//EventDispatcherProfiler.RemoveListener(typeof(T).Name, type, listener);
			}
		}

		public void RemoveSeriesListener(int type)
		{
			Delegate listener = this[type];
			if (listener == null)
				return;

			Delegate[] listeners = this[type].GetInvocationList();
			for (int j = 0; j < listeners.Length; j++)
			{
				this.RemoveListener(type, (Action<T>)listeners[j]);
			}
		}

		public void ClearListeners()
		{
			int[] types = this._listenerLookup.Keys.ToArray();
			for (int i = 0; i < types.Length; i++)
			{
				int type = types[i];
				if (this[type] == null)
				{
					continue;
				}
				Delegate[] listeners = this[type].GetInvocationList();
				for (int j = 0; j < listeners.Length; j++)
				{
					this.RemoveListener(type, (Action<T>)listeners[j]);
				}
			}
			this._listenerLookup.Clear();
		}
	}

	[System.Serializable]
	public class TriggerEvent : EventBase
	{
		[SerializeField] private bool _autoRegister = true;
		[SerializeField] private int _cmdID = 0;

		private object[] _args;

		public TriggerEvent() { }

		public TriggerEvent(params object[] args)
		{
			this._args = args;
		}

		public bool IsAuto
		{
			get { return this._autoRegister; }
		}

		public int CMD_ID
		{
			set { this._cmdID = value; }
			get { return this._cmdID; }
		}

		public object[] Args
		{
			set { this._args = value; }
			get { return this._args; }
		}
	}
}
