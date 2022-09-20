using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands : MonoBehaviour {

	private static Dictionary<string, CommandFactory> commands = new Dictionary<string, CommandFactory>();

	public static readonly CommandFactory MoveCommand = RegisterCommand<MoveCommand, Vector2>("move", (target) => new MoveCommand(target));

	private void Start() {
		
	}

	public static C Construct<C, T>(CommandFactory factory, T value) where C : Command<T> {
		if (commands.ContainsValue(factory)) {
			CommandFactory<C, T> newFactory = factory.GetAsType<C, T>();
			return newFactory.Construct(value);
		}

		else return null;
	}

	public static C Construct<C, T>(string name, T value ) where C : Command<T> {
		if (commands.TryGetValue(name, out CommandFactory factory)) {
			return Construct<C, T>(factory, value);
		}

		else return null;
	}

	//T is the Command type, which will be used to make the relevant CommandFactory that will output the Command as the specified class T.
	private static CommandFactory RegisterCommand<C, T> (string name, CommandFactory<C, T>.Del constructor) where C : Command<T> {
		CommandFactory factory = new CommandFactory<C, T>(name, constructor);
		commands.Add(name, factory);
		return factory;
	}

	
	public class CommandFactory<C, T> : CommandFactory where C : Command<T> {
		
		public delegate C Del(T target);
		public Del del;

		public CommandFactory(string name, Del constructor) : base(name, typeof(C)) {
			del = constructor;
		}

		public C Construct (T _target) {
			return del.Invoke(_target);
		}
	}

	public class CommandFactory {

		public string Name { get; private set; }
		public Type CommandType { get; private set; }

		internal CommandFactory<C, T> GetAsType<C, T>() where C : Command<T> {
			if (typeof(C) == CommandType) {
				return (CommandFactory<C, T>)this;
			}

			else return null;
		}

		internal CommandFactory (string _name, Type _commandType) {
			CommandType = _commandType;
		}
	}
}