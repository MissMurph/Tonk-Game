using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public abstract class Command<T> : Command {

	private T target;
	
	protected Command (T _target, string _name) : base(typeof(T), _name) {
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

	public string Name { get; private set; }
	public Type TargetType { get; private set; }

	internal Command(Type _type, string _name) {
		TargetType = _type;
		Name = _name;
	}

	public T GetAsType<T>() where T : Command {
		if (typeof(T) == this.GetType()) {
			return (T)this;
		}

		else return null;
	}
}