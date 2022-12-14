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

		public T Target { get; private set; }

		protected Command(T _target, string _name) : base(typeof(T), _name) {
			Target = _target;
			if (cntxt == null) cntxt = () => new CommandContext(Character, this, Phase);
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
		public event CommandCallback OnComplete;

		public delegate CommandContext ContextConstructor ();

		protected ContextConstructor cntxt;

		public virtual void Start(Character character) {
			//These IF checks exist in case timing causes either Start or Perform to be called as the completion is triggering, potentially reversing the completion and soft-locking the manager
			if (Phase.Equals(CommandPhase.Cancelled) || Phase.Equals(CommandPhase.Complete)) return;

			Character = character;
			Phase = CommandPhase.Started;

			CommandContext context = cntxt.Invoke();

			//PostCommandEvent(context);
			if (OnStart != null) OnStart.Invoke(context);
		}

		public virtual void Perform () {
			if (Phase.Equals(CommandPhase.Cancelled) || Phase.Equals(CommandPhase.Complete) || Phase.Equals(CommandPhase.Performed)) return;

			Phase = CommandPhase.Performed;

			CommandContext context = cntxt.Invoke();

			//PostCommandEvent(context);
			if (OnPerform != null) OnPerform.Invoke(context);
		}

		public virtual void Cancel () {
			Phase = CommandPhase.Cancelled;

			CommandContext context = cntxt.Invoke();

			//PostCommandEvent(context);
			if (OnComplete != null) OnComplete.Invoke(context);
		}

		protected virtual void Complete () {
			Phase = CommandPhase.Complete;

			CommandContext context = cntxt.Invoke();

			//PostCommandEvent(context);
			if (OnComplete != null) OnComplete.Invoke(context);
		}

		public virtual void OnTriggerEnter(Collider2D collision) { }

		//Temporarily discontinuing this. Provided alternatives so only internal systems with access to the Commands can influence their behaviour using public events above
		//Not deleting as Observer could still be adopted effectively for CommandEvents
		private CharacterEvent.CommandEvent PostCommandEvent(CommandContext context) {
			return EventBus.Post(new CharacterEvent.CommandEvent(context));
		}

		internal Command(Type _type, string _name) {
			TargetType = _type;
			Name = _name;
			Phase = CommandPhase.Waiting;
		}

		public T GetAsType<T>() where T : Command {
			if (typeof(T) == this.GetType()) {
				return (T)this;
			}

			else return null;
		}

		public enum CommandPhase {
			Waiting,
			Started,
			Performed,
			Cancelled,
			Complete
		}

		public class CommandContext {

			public Character Character { get; private set; }
			public Command Command { get; private set; }
			public CommandPhase Phase { get; private set; }

			protected CommandContext(Command _command) : this(_command.Character, _command, _command.Phase) {}

			internal CommandContext (Character _character, Command _command, CommandPhase _phase){
				Character = _character;
				Command = _command;
				Phase = _phase;
			}
		}
	}
}