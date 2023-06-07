using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Capabilities;
using TankGame.Events;
using TankGame.Units.Ai;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Units.Interactions {

	public class Source : MonoBehaviour {

		private Dictionary<string, Interaction> registered;

		private Dictionary<string, UnityEventBase> listenerMap;

		private Dictionary<string, PreRequisite> addedPreRequisites = new Dictionary<string, PreRequisite>();

		private Character character;

		protected virtual void Awake () {
			IInteractable[] interactables = GetComponents<IInteractable>();
			character = GetComponent<Character>();
			Collector interactionScraper = new Collector(gameObject, ISide.Source);

			foreach (IInteractable interactable in interactables) {
				interactable.OnCollection(interactionScraper);
			}

			registered = interactionScraper.Interactions;
			listenerMap = interactionScraper.Listeners;
		}

		public void Request (Actor.Request request) {
			if (registered.TryGetValue(request.Key, out Interaction found)) {
				Interactionlet packet = new Interactionlet(found, request.Data);

				packet.Phase = IPhase.Pre;
				packet.Result = IResult.Continue;

				InteractionEvent _event = Post(packet);

				if (!_event.Result.Equals(IResult.Cancel)) {
					packet.Result = found.Construct(request.Actor, packet);
				}
				else packet.Result = IResult.Cancel;

				
				request.Callback(packet);
				Post(packet);
			}
		}

		public void Act (Interactionlet packet, Actor actor, Action<Interactionlet> callback) {
			packet.Step = IStep.Act;
			packet.Phase = IPhase.Pre;
			packet.Result = packet.Interaction.Act(actor, packet);
		}

		private InteractionEvent Post (Interactionlet result) {
			InteractionEvent _event = new InteractionEvent(result, character, ISide.Actor);
			EventBus.Post(_event);
			return _event;
		}

		public List<PreRequisite> GetPreRequisites () {
			List<PreRequisite> output = new List<PreRequisite>();

			output.AddRange(addedPreRequisites.Values);

			return output;
		}

		public void SubmitPreRequisite (string key, IEvaluator condition, State solution){
			PreRequisite output = new PreRequisite {
				condition = condition,
				solution = solution
			};

			addedPreRequisites.TryAdd(key, output);
		}

		public void ExpirePreRequisite (string key) {
			if (addedPreRequisites.ContainsKey(key)) {
				addedPreRequisites.Remove(key);
			}
		}
	}

	public class Interactionlet {

		public IPhase Phase { get; set; }
		public IStep Step { get; set; }
		public IResult Result { get; set; }
		public string Name { get { return Interaction.Name; } }

		internal Interaction Interaction;

		public Dictionary<string, ICapability> Data { get; private set; }

		public Source Source { get { return Interaction.Parent.GetManager(); } }
		public IInteractable Component { get { return Interaction.Parent; } }

		internal Interactionlet (Interaction _interaction, params ICapability[] data) {
			Interaction = _interaction;
			Data = new Dictionary<string, ICapability>();

			foreach (ICapability capability in data) {
				Data.TryAdd(capability.Name, capability);
			}
		}

		public void Cancel () {
			Result = IResult.Cancel;
		}
	}
}