using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	[Serializable]
	public abstract class Command<T> : Command {

		private T target;

		protected Command(T _target, string _name) : base(typeof(T), _name) {
			target = _target;
		}

		public T Target() {
			return target;
		}
	}

	[Serializable]
	public abstract class Command {
		public string Name { get; private set; }
		public Type TargetType { get; private set; }
		public CommandPhase Phase { get; private set; }
		public Transform TargetTransform { get; protected set; }

		private Action<CommandContext> completeCallback;

		protected Character Character { get; private set; }

		public virtual void Start(Character character, Action<CommandContext> callback) {
			Character = character;
			Phase = CommandPhase.Started;
			PostCommandEvent(character);
			completeCallback = callback;
		}

		public virtual void Perform() {
			Phase = CommandPhase.Performed;
			PostCommandEvent(Character);
		}

		public virtual void Cancel() {
			Phase = CommandPhase.Cancelled;

			CommandContext context = new CommandContext(Character, this, Phase);

			PostCommandEvent(context);
			completeCallback(context);
		}

		protected virtual void Complete () {
			Phase = CommandPhase.Complete;

			CommandContext context = new CommandContext(Character, this, Phase);
			PostCommandEvent(context);
			completeCallback(context);
		}

		public virtual void OnTriggerEnter(Collider2D collision) { }

		private CharacterEvent.CommandEvent PostCommandEvent(Character character) {
			return EventBus.Post(new CharacterEvent.CommandEvent(new CommandContext(character, this, Phase)));
		}

		private CharacterEvent.CommandEvent PostCommandEvent(CommandContext context) {
			return EventBus.Post(new CharacterEvent.CommandEvent(context));
		}

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

		public enum CommandPhase {
			Started,
			Performed,
			Cancelled,
			Complete
		}

		public struct CommandContext {

			public Character Character { get; private set; }
			public Command Command { get; private set; }
			public CommandPhase Phase { get; private set; }

			internal CommandContext(Character _character, Command _command, CommandPhase _phase) {
				Character = _character;
				Command = _command;
				Phase = _phase;
			}
		}
	}
}