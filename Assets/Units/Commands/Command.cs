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

		protected Character Character { get; private set; }

		public delegate void CommandCallback(CommandContext context);

		public event CommandCallback OnStart;
		public event CommandCallback OnPerform;
		public event CommandCallback OnCancel;
		public event CommandCallback OnComplete;

		public virtual void Start(Character character) {
			Character = character;
			Phase = CommandPhase.Started;

			CommandContext context = Context();

			//PostCommandEvent(context);
			OnStart.Invoke(context);
		}

		public virtual void Perform () {
			Phase = CommandPhase.Performed;

			CommandContext context = Context();

			//PostCommandEvent(context);
			OnPerform.Invoke(context);
		}

		public virtual void Cancel () {
			Phase = CommandPhase.Cancelled;

			CommandContext context = Context();

			//PostCommandEvent(context);
			OnCancel.Invoke(context);
		}

		protected virtual void Complete () {
			Phase = CommandPhase.Complete;

			CommandContext context = Context();

			//PostCommandEvent(context);
			OnComplete.Invoke(context);
		}

		private CommandContext Context () {
			return new CommandContext(Character, this, Phase);
		}

		public virtual void OnTriggerEnter(Collider2D collision) { }

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