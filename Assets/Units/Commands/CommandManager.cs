using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Units.Commands {

	public class CommandManager : MonoBehaviour {

		const float commandUpdateTime = 0.5f;

		public CommandBinding[] commands;
		private Dictionary<string, CommandUnityEvent> boundCommands = new Dictionary<string, CommandUnityEvent>();

		private Queue<Command> commandQueue = new Queue<Command>();

		public bool executingCommand = false;

		private Coroutine currentCoroutine;

		public Command ActiveCommand { get; private set; }

		private List<Transform> inRangeTransforms = new List<Transform>();

		[SerializeField]
		Character character;

		private void Awake() {
			character = GetComponent<Character>();

			for (int i = 0; i < commands.Length; i++) {
				boundCommands.Add(commands[i].name, commands[i].action);
			}
		}

		protected virtual void Start() {
			StartCoroutine(UpdateCommands());
		}

		IEnumerator UpdateCommands() {
			if (Time.timeSinceLevelLoad < .3f) {
				yield return new WaitForSeconds(.3f);
			}

			while (true) {
				yield return new WaitForSeconds(commandUpdateTime);

				if (!executingCommand && commandQueue.TryDequeue(out Command command)) {
					StopCoroutine(currentCoroutine);
					PerformCommand(command);
				}
			}
		}

		protected void PerformCommand(Command command) {
			/*if (boundCommands.TryGetValue(command.Name, out CommandUnityEvent boundFunction)) {
				executingCommand = true;
				boundFunction.Invoke(command, CommandComplete);
			}*/

			if (character.Embarked) character.Disembark();

			ActiveCommand = command;
			ActiveCommand.Start(character);
			ActiveCommand.OnComplete += CommandComplete;
		}

		//Callback function for commands to let the manager know when they're done.
		private void CommandComplete(Command.CommandContext context) {
			executingCommand = false;
			ActiveCommand = null;
			character.target = null;
		}

		public virtual void EnqueueCommand(Command command) {
			if (!commandQueue.Contains(command) && !character.Embarked) commandQueue.Enqueue(command);
		}

		public virtual void ExecuteCommand(Command command) {
			if (ActiveCommand != null) ActiveCommand.Cancel();

			commandQueue.Clear();

			//if (character.Embarked) character.Disembark();

			PerformCommand(command);
		}


		//interaction trigger
		private void OnTriggerEnter2D(Collider2D collision) {
			Transform parentTransform = collision.transform.root;

			if (ActiveCommand != null && ActiveCommand.TargetTransform != null && ActiveCommand.TargetTransform == parentTransform) {
				ActiveCommand.OnTriggerEnter(collision);
			}

			inRangeTransforms.Add(collision.transform);
		}

		private void OnTriggerExit2D(Collider2D collision) {
			if (inRangeTransforms.Contains(collision.transform)) {
				inRangeTransforms.Remove(collision.transform);
			}
		}

		public bool IsInRange (Transform transform) {
			return inRangeTransforms.Contains(transform);
		}

		public List<Transform> TransformsInRange () {
			return new List<Transform>(inRangeTransforms);
		}
	}

	[Serializable]
	public class CommandBinding {
		public string name;
		public CommandUnityEvent action;
	}

	//This is a dummy because again, can't have generics in inspector, but can construct UnityEvents with a Generic type
	[Serializable]
	public class CommandUnityEvent : UnityEvent<Command, Action<bool>> {

	}
}