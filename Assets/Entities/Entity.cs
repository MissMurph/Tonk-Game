using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Entities {

	public class Entity : MonoBehaviour {

		private Dictionary<Type, ITaggable> registered;

		private void Awake () {
			registered = new Dictionary<Type, ITaggable>();

			ITaggable[] found = GetComponents<ITaggable>();

			foreach (ITaggable component in found) {
				component.Register(this);
			}
		}

		public T Get<T> () where T : ITaggable {
			if (registered.TryGetValue(typeof(T), out ITaggable found)) {
				return (found as ITaggable<T>).Get();
			}

			Debug.LogError("No component found of type " + typeof(T) + " attached to Entity " + name);
			//Visual Studio recommended the below and I don't know what its going to do but hey it compiles
			return default;
		}

		public void Register<T> (T component) where T : ITaggable {
			if (!registered.TryAdd(typeof(T), component)) Debug.LogError("Component of type " );
		}
	}
}