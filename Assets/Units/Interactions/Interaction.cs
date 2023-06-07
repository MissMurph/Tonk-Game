using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Capabilities;
using TankGame.Events;
using UnityEngine;

namespace TankGame.Units.Interactions {

	public class Interaction {

		//Both Construction and Acting will require a result & an Interactionlet thats dealt with, because of these we use the same delegate for both
		public delegate IResult InteractionDel (Actor actor, Interactionlet data);

		private InteractionDel Constructor { get; set; }
		private InteractionDel Destination { get; set; }

		public string Name { get; private set; }

		public int Priority { get; private set; }

		public IInteractable Parent { get; private set; }

		public Source Manager { get { return Parent.GetManager(); } }

		private Dictionary<string, ICapability> capabilities;

		public Interaction (InteractionDel _destination, InteractionDel _constructor, string _name, IInteractable _parent) {
			Destination = _destination;
			Constructor = _constructor;
			Name = _name;
			Parent = _parent;
			capabilities = new Dictionary<string, ICapability>();
		}

		public void RegisterCapability (string key, ICapability data) {
			if (!capabilities.TryAdd(key, data)) {
				Debug.LogError(key + " capability already submitted to " + Name + " interaction on " + Parent.GetObject().name + "!");
			}
		}

		public IResult Construct (Actor actor, Interactionlet data) => Constructor(actor, data);

		public IResult Act (Actor actor, Interactionlet data) => Destination(actor, data);

		//Priorities are between 0 -> 9
		//9 is highest priority, 0 is set to be ignored by default command construction
		//Set to 0 if you don't want this to be automatically ordered on right click
		public void SetPriority (int value) {
			Priority = Mathf.Clamp(value, 0, 9);
		}
	}

	

	public enum IPhase {
		Pre,
		Post
	}

	public enum IStep {
		Request,
		Start,
		Act,
		Handshake,
		Complete
	}

	public enum IResult {
		Cancel,
		Continue,
		Fail,
		Success
	}

	public enum ISide {
		Source,
		Actor
	}
}