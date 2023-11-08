using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LinkedElement<T> : LinkedElement where T : MonoBehaviour {

	public abstract T GetLinked();

	protected override void Awake () {
		LinkedType = typeof(T);
		//LinkedName = GetLinked().name;
		base.Awake();
	}
}

public abstract class LinkedElement : MonoBehaviour {

	public Type LinkedType { get; internal set; }
	public string LinkedName { get; internal set; }
	public RectTransform UITransform { 
		get {
			return (RectTransform)transform;
		}
	}

	public LinkedElement<T> GetAsType<T> () where T : MonoBehaviour {
		if (typeof(T).Equals(LinkedType)) return (LinkedElement<T>) this;

		throw new ArgumentException("Type Entered Doesn't Match Linked Type!");
	}

	protected virtual void Awake () {
		LinkedCache.OnElementInstantiate(this);
	}

	private void OnDestroy () {
		LinkedCache.OnElementDestroy(this);
	}
}