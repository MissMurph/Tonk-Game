using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TankGame.Units;
using TankGame.Units.Commands;
using TankGame.Players.Input;
using TankGame.Events;
using TankGame.GameWorld;

namespace TankGame.Players {

	public class Player : MonoBehaviour {

		public static Player Instance { get; private set; }

		public static PlayerController Controller {
			get {
				return Instance.playerController;
			}
		}

		public static PlayerUIController UI {
			get {
				return Instance.uiController;
			}
		}

		public static Transform Canvas {
			get {
				return Instance.canvasTransform;
			}
		}

		[SerializeField]
		private Transform canvasTransform;

		[SerializeField]
		private PlayerUIController uiController;

		private PlayerController playerController;

		public WorldGrid world;

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

		public void Input(InputAction.CallbackContext context) {
			currentControl.Input(context);
		}

		public void SwitchControl(IControllable controllable) {
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

		public void SwitchImplicitExplicit(InputAction.CallbackContext context) {
			if (context.started) {
				if (currentControl.Equals(playerController) && explicitController != null) {
					currentControl = explicitController;
					input.SwitchCurrentActionMap("Explicit");
					isImplicit = false;
					EventBus.Post(new PlayerEvent.ControlSwitch(PlayerEvent.ControlSwitch.ControlType.Explicit));
				}
				else {
					currentControl = playerController;
					input.SwitchCurrentActionMap("Implicit");
					isImplicit = true;
					EventBus.Post(new PlayerEvent.ControlSwitch(PlayerEvent.ControlSwitch.ControlType.Implicit));
				}
			}
		}

		public void ResetControl() {
			if (!currentControl.GetType().Equals(playerController.GetType())) {
				SwitchControl(playerController);
			}
		}

		public static List<Character> GetCharacters() {
			return Instance.characters;
		}

		public static List<ISelectable> GetSelected() {
			return Instance.playerController.GetSelected();
		}

		private void OnDestroy () {
			Instance = null;
		}
	}
}