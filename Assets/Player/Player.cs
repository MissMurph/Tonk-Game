using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour {

	public static Player Instance { get; private set; }

	[SerializeField]
	private PlayerController playerController;

	public World world;

	public IControllable currentControl;

	private IControllable explicitController;

	public Character[] sceneCharacters;

	public List<Character> characters;

	private PlayerCharacter playerCharacter;

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

		Instance = this;
	}

	private void Update() {
		if (currentControl != null && currentControl.GetObject() != this.gameObject) {
			GameObject target = currentControl.GetObject();
			transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
		}
	}

	public void Input (InputAction.CallbackContext context) {
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

	public static List<Character> GetCharacters () {
		return Instance.characters;
	}

	public static List<ISelectable> GetSelected() {
		return Instance.playerController.GetSelected();
	}

	//False represents deselection, true represents selections
	public static bool Select (ISelectable selectable) {
		return Instance.playerController.Select(selectable);
	}
}