using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProcessor : MonoBehaviour, IControllable {

	public InputEntry[] inputEntries;

	private Dictionary<string, string> inputDictionary = new Dictionary<string, string>();

	private CommandManager commandManager;

	private void Awake() {
		commandManager = GetComponent<CommandManager>();

		foreach (InputEntry entry in inputEntries) {
			inputDictionary.Add(entry.inputAction.action.name, entry.commandName);
		}
	}

	public void Input (InputAction.CallbackContext context) {
		if (inputDictionary.TryGetValue(context.action.name, out string commandName)) {

			commandManager.ExecuteCommand(Commands.ConstructFromInput(commandName, context));
		}
	}

	public GameObject GetObject() {
		return gameObject;
	}
}

[System.Serializable]
public class InputEntry {

	public InputActionReference inputAction;
	public string commandName;
}