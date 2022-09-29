using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour {

	[SerializeField]
	private PlayerController playerController;

	public World world;

	public IControllable currentControl;

	private IControllable explicitController;

	public Character[] sceneCharacters;

	private List<Character> characters;
	private PlayerCharacter playerCharacter;

	public InputActionReference action;

	public delegate void InputDelegate(InputAction.CallbackContext context);

	private Dictionary<string, InputDelegate> InputMap = new Dictionary<string, InputDelegate>();

	private PlayerInput input;

	public bool isImplicit;

	private void Awake() {
		playerController = GetComponent<PlayerController>();
		currentControl = playerController;
		characters = new List<Character>(sceneCharacters);
		input = GetComponent<PlayerInput>();
		isImplicit = true;

		//action.ToInputAction().
		
		foreach (Character c in characters) {
			if (c.GetType() == typeof(PlayerCharacter)) {
				playerCharacter = (PlayerCharacter)c;
				break;
			}
		}
	}

	private void Update() {
		if (currentControl != null && currentControl.GetObject() != this.gameObject) {
			GameObject target = currentControl.GetObject();
			transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
		}
	}

	public void Input (InputAction.CallbackContext context) {
		Debug.Log(context.action.name);
		currentControl.Input(context);
	}

	public void SwitchControl (IControllable controllable) {
		//Debug.Log("Old: " + currentControl.GetObject().name + "   New: " + controllable.GetObject().name);

		if (explicitController == null || explicitController.GetObject().name != controllable.GetObject().name) {
			explicitController = controllable;
			currentControl = explicitController;
			input.SwitchCurrentActionMap("Explicit");
			isImplicit = false;

			//currentControl.GetInput().enabled = false;

			//currentControl = controllable;

			//controllable.GetInput().enabled = true;
			//controllable.GetInput().ActivateInput();
		}
	}

	public void SwitchImplicitExplicit (InputAction.CallbackContext context) {
		if (context.started) {
			if (currentControl.Equals(playerController) && explicitController != null) {
				currentControl = explicitController;
				input.SwitchCurrentActionMap("Explicit");
				isImplicit = false;
			}
			else {
				currentControl = playerController;
				input.SwitchCurrentActionMap("Implicit");
				isImplicit = true;
			}
		}
	}

	public void ResetControl () {
		if (currentControl.GetType().Equals(playerController.GetType())) {
			SwitchControl(playerController);
		}
	}
}