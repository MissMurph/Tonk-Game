using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Events {

	public class EventBus : MonoBehaviour {

		private static EventBus instance;

		private Dictionary<Type, UnityEventBase> map = new Dictionary<Type, UnityEventBase>();

		private void Awake() {
			instance = this;
		}

		public static T Post<T>(T postedEvent) {
			Type type = typeof(T);

			if (instance.map.TryGetValue(type, out UnityEventBase value)) {
				UnityEvent<T> superType = (UnityEvent<T>)value;
				superType.Invoke(postedEvent);
			}

			return postedEvent;
		}

		public static void Subscribe<T>(UnityAction<T> func) where T : AbstractEvent {
			Type type = typeof(T);

			if (instance.map.TryGetValue(type, out UnityEventBase value)) {
				UnityEvent<T> superType = (UnityEvent<T>)value;
				superType.AddListener(func);
			}
			else {
				UnityEvent<T> unityEvent = new UnityEvent<T>();
				unityEvent.AddListener(func);
				instance.map.Add(type, unityEvent);
			}
		}

		public static void Unsubscribe<T> (UnityAction<T> func) where T : AbstractEvent {
			Type type = typeof(T);

			if (instance.map.TryGetValue(type, out UnityEventBase value)) {
				UnityEvent<T> superType = (UnityEvent<T>)value;
				superType.RemoveListener(func);
			}
		}
	}
}