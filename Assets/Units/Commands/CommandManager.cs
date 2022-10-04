using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandManager : MonoBehaviour {

	const float commandUpdateTime = 0.5f;

	public CommandBinding[] commands;
	private Dictionary<string, CommandUnityEvent> boundCommands = new Dictionary<string, CommandUnityEvent> ();

	private Queue<Command> commandQueue = new Queue<Command>();

	public bool executingCommand = false;

	private Coroutine currentCoroutine;

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
		if (boundCommands.TryGetValue(command.Name, out CommandUnityEvent boundFunction)) {
			executingCommand = true;
			boundFunction.Invoke(command, CommandComplete);
		}
	}

	//Successful if fully completed, if not then it's been interrupted
	private void CommandComplete (bool successful) {
		executingCommand = false;
	}

	public virtual void EnqueueCommand(Command command) {
		if (!commandQueue.Contains(command) && !character.Embarked) commandQueue.Enqueue(command);
	}

	public virtual void ExecuteCommand(Command command) {
		commandQueue.Clear();

		if (character.Embarked) character.Disembark();

		PerformCommand(command);
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