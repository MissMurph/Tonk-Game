using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputProcessor : MonoBehaviour, IControllable {

	[SerializeField]
	private InputEntry[] inputEntries;

	private Dictionary<string, UnityEvent<InputAction.CallbackContext>> inputDictionary = new Dictionary<string, UnityEvent<InputAction.CallbackContext>>();

	private void Awake() {
		//commandManager = GetComponent<CommandManager>();

		if (inputEntries.Length == 0) return;

		//Debug.Log("input entries:  " + inputEntries.Length);

		foreach (InputEntry entry in inputEntries) {
			//Debug.Log("Converting InputEntry to input: " + entry.inputAction.action.name);
			inputDictionary.Add(entry.inputAction.action.name, entry.function);
		}
	}

	public void Input (InputAction.CallbackContext context) {
		Debug.Log(context.action.name);
		if (inputDictionary.TryGetValue(context.action.name, out UnityEvent<InputAction.CallbackContext> function)) {
			//Debug.Log("Dictionary contained");
			function.Invoke(context);
		}
	}

	public GameObject GetObject() {
		return gameObject;
	}

	public void AddInput (InputEntry entry) {
		//Debug.Log(inputDictionary.Count);

		Debug.Log("Trying to add Input: " + entry.inputAction.action.name + "   " + inputDictionary.TryAdd(entry.inputAction.action.name, entry.function));
		//inputDictionary.Add(entry.inputAction.action.name, entry.function);
	}

	public List<InputEntry> GetInputs () {
		return new List<InputEntry>(inputEntries);
	}
}

[System.Serializable]
public class InputEntry {

	public InputActionReference inputAction;
	public UnityEvent<InputAction.CallbackContext> function;
}