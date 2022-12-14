using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Interactions {
	
	public class InteractionFactory<T> : AbstractInteractionFactory {

		public delegate AbstractInteraction ConstructionDel(T target, Character character, string name);
		public delegate List<T> TargetDel ();
		public delegate T EvaluateDel (Character character);

		protected ConstructionDel Constructor { get; private set; }
		protected TargetDel TargetQuery { get; private set; }
		protected EvaluateDel Evaluation { get; private set; }

		public AbstractInteraction Construct (T target, Character character) {
			return Constructor.Invoke(target, character, Name);
		}

		public override AbstractInteraction Construct (Character character) {
			if (Evaluation != null) return Construct(Evaluation.Invoke(character), character);
			return base.Construct(character);
		}

		public List<T> GetTargets () {
			return TargetQuery.Invoke();
		}

		public InteractionFactory (string name, ConstructionDel _constructor, TargetDel _targetQuery) : base(typeof(T), typeof(T), name) {
			Constructor = _constructor;
			TargetQuery = _targetQuery;
		}

		public InteractionFactory (string name, ConstructionDel _constructor, TargetDel _targetQuery, EvaluateDel _evaluation) : base(typeof(T), typeof(T), name) {
			Constructor = _constructor;
			TargetQuery = _targetQuery;
			Evaluation = _evaluation;
		}
	}

	public abstract class AbstractInteractionFactory {

		public string Name { get; private set; }
		public Type TargetType { get; private set; }
		public Type InteractionType { get; private set; }

		internal AbstractInteractionFactory (Type _targetType, Type _intType, string _name) {
			TargetType = _targetType;
			InteractionType = _intType;
			Name = _name;
		}

		public virtual AbstractInteraction Construct (Character character) {
			Debug.LogWarning("No default option available! Choose your target manually");
			return null;
		}

		public InteractionFactory<T> GetAsType<T> () {
			if (typeof(T).Equals(TargetType)) {
				return (InteractionFactory<T>)this;
			}

			Debug.LogException(new ArgumentException("Object not Requested type, cannot be cast to! Returned null"));
			return null;
		}
	}
}