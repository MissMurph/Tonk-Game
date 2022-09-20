using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class Command<T> : Command {

	private T target;

	protected Command (T _target) : base(typeof(T)) {
		target = _target;
	}

	public T Target() {
		return target;
	}
}

[Serializable]
public abstract class Command {

	public string name;
	public Type type;

	internal Command(Type _type) {
		type = _type;
	}

	public Command<T> GetAsType<T>() {
		if (typeof(T) == type) {
			return (Command<T>)this;
		}

		else return null;
	}

	public Type TargetType() {
		return type;
	}
}