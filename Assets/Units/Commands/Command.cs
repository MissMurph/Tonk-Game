using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using TankGame.Units.Ai;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	[Serializable]
	public abstract class Command : Goal {

		[SerializeField] public string Name { get; private set; }

		public CommandPhase Phase { get; protected set; } = CommandPhase.Waiting;

		public delegate void Callback ();

		public event Callback OnComplete;

		public Command () {

		}

		protected Command (Command command) : base(command) {
			Name = command.Name;
		}

		protected virtual void End () {
			if (OnComplete != null) OnComplete.Invoke();
		}
	}

	public abstract class TargetedCommand<T> : Command {

		public T Target { get; protected set; }

		[SerializeField] private int endNode;

		public TargetedCommand () {
			
		}

		protected TargetedCommand (Command command, T _target) : base(command) {
			if (command is TargetedCommand<T>) {
				TargetedCommand<T> tComm = (TargetedCommand<T>) command;

				endNode = tComm.endNode;
			}

			Target = _target;
		}

		public override void Initialize (Character character) {
			base.Initialize(character);

			Nodes[endNode].OnComplete += End;

			foreach (Decision node in Nodes) {
				if (node.State is TargetedState<T>) {
					TargetedState<T> state = (TargetedState<T>)node.State;
					state.SetTarget(Target);
				}
			}
		}
	}

	public enum CommandPhase {
		Waiting,
		Acting,
		Completed,
		Failed
	}
}