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

//Dummy class for array and list storage, since you can't create a list of generics that you don't know the types of at run time
//call GetAsType where T = Command Type (for example MoveCommand) to get the Command as it's proper type
[Serializable]
public abstract class Command {

	public string name;
	public Type type;

	internal Command(Type _type) {
		type = _type;
	}

	public T GetAsType<T>() where T : Command {
		if (typeof(T) == this.GetType()) {
			return (T)this;
		}

		else return null;
	}

	public Type TargetType() {
		return type;
	}
}