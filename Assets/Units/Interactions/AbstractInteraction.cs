using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;

namespace TankGame.Units.Interactions {

	public abstract class AbstractInteraction<T> : AbstractInteraction where T : AbstractInteraction<T> {

		public delegate InteractionContext<T> InteractionFunction (T interaction);

		private InteractionFunction func;

		private IResult currentResult = IResult.Start;

		protected AbstractInteraction (InteractionFunction _destination, Character _character, string _name, IInteractable _parent) : base(typeof(T), _name, _parent, _character) {
			func = _destination;
		}

		public override InteractionContext Act (Source actor) {
			//Fire Pre Events
			InteractionContext<T> context = PostEvent(new InteractionContext<T>((T)this, IPhase.Pre, currentResult), actor);

			//Check if the interaction is being cancelled
			if (context.Result.Equals(IResult.Cancel)) {
				context.Phase = IPhase.Post;
				PostEvent(context, actor);
				return context;
			}

			context = func.Invoke((T)this);

			currentResult = context.Result;

			PostEvent(context, actor);

			return context;
		}

		//Will fire local manager event, then local actor, then global event bus event
		private InteractionContext<T> PostEvent (InteractionContext<T> context, Source actor) {
			Parent.GetManager().Post(context);
			actor.Post(context);
			EventBus.Post(new InteractionEvent<T>(context));
			return context;
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

		public abstract InteractionContext Act (Source actor);
	}

	/*	CONTEXT	*/

	public class InteractionContext<T> : InteractionContext where T : AbstractInteraction<T> {
		public T Interaction { get; private set; }

		public InteractionContext (T _interaction, IPhase _phase, IResult _result) : base(_interaction.Name, _phase, _result) {
			Interaction = _interaction;
		}
	}

	public class InteractionContext {
		public IPhase Phase { get; internal set; }
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