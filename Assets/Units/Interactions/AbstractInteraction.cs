using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Interactions {

	public abstract class AbstractInteraction<T> : AbstractInteraction where T : AbstractInteraction<T> {

		public delegate InteractionContext<T> InteractionFunction (T interaction);

		private InteractionFunction func;

		protected AbstractInteraction (InteractionFunction _destination, Character _character, string _name, IInteractable _parent) : base(typeof(T), _name, _parent, _character) {
			func = _destination;
		}

		public override InteractionContext Act (InteractionManager actor) {
			InteractionContext<T> context = actor.Post(new InteractionContext<T>((T)this, IPhase.Pre, IResult.Start));

			if (context.Result.Equals(IResult.Cancel)) return context;

			context = func.Invoke((T)this);

			return actor.Post(context);
		}
	}

	public abstract class AbstractInteraction {

		public Type TargetType { get; private set; }
		public string Name { get; private set; }
		public IInteractable Parent { get; private set; }
		public Character ActingCharacter { get; private set; }

		internal AbstractInteraction (Type _type, string _name, IInteractable _parent, Character _character) {
			TargetType = _type;
			Name = _name;
			Parent = _parent;
			ActingCharacter = _character;
		}

		public abstract InteractionContext Act (InteractionManager actor);
	}

	/*	CONTEXT	*/

	public class InteractionContext<T> : InteractionContext where T : AbstractInteraction<T> {
		public T Interaction { get; private set; }

		public InteractionContext (T _interaction, IPhase _phase, IResult _result) : base(_interaction.Name, _phase, _result) {
			Interaction = _interaction;
		}
	}

	public class InteractionContext {
		public IPhase Phase { get; private set; }
		public IResult Result { get; private set; }
		public string Name { get; private set; }

		internal InteractionContext (string _name, IPhase _phase, IResult _result) {
			Name = _name;
			Phase = _phase;
			Result = _result;
		}

		public void Cancel () {
			Result = IResult.Cancel;
		}
	}

	public enum IPhase {
		Pre,
		Post
	}

	public enum IResult {
		Start,
		Cancel,
		Fail,
		Continue,
		Success
	}
}