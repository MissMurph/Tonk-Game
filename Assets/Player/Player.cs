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
using TankGame.Units.Navigation;

namespace TankGame.Players {

	public class Player : MonoBehaviour {

		private static Player Instance;

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

		public static List<Character> Characters {
			get {
				return new List<Character>(Instance.characters);
			}
		}

		public static Character PlayerCharacter {
			get {
				return Instance.playerCharacter;
			}
		}

		public static IControllable CurrentHost {
			get {
				return Instance.currentControl;
			}
		}

		public static ControlType ControlMode {
			get {
				return Instance.mode;
			}
		}

		[SerializeField]
		private Transform canvasTransform;

		[SerializeField]
		private PlayerUIController uiController;

		private PlayerController playerController;

		private IControllable currentControl;

		private IControllable explicitController;

		[SerializeField]
		private List<Character> characters = new List<Character>();

		[SerializeField]
		private Character playerCharacter;

		private PlayerInput input;

		private ControlType mode;

		private void Awake () {
			playerController = GetComponent<PlayerController>();
			currentControl = playerController;
			input = GetComponent<PlayerInput>();
			mode = ControlType.Implicit;

			Instance = this;
		}

		private void Update () {
			if (currentControl != null && currentControl.GetObject() != this.gameObject) {
				GameObject target = currentControl.GetObject();
				transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
			}
		}

		public void Input (InputAction.CallbackContext context) {
			currentControl.Input(context);
		}

		public static void SwitchControl (IControllable controllable) {
			//Debug.Log("Old: " + currentControl.GetObject().name + "   New: " + controllable.GetObject().name);

			if (Instance.explicitController == null || Instance.explicitController.GetObject().name != controllable.GetObject().name) {
				IControllable oldHost = Instance.explicitController;
				Instance.explicitController = controllable;
				Instance.currentControl = Instance.explicitController;
				Instance.input.SwitchCurrentActionMap("Explicit");
				Instance.mode = ControlType.Explicit;

				EventBus.Post(new PlayerEvent.HostChange(oldHost, Instance.explicitController));

				//currentControl.GetInput().enabled = false;

				//currentControl = controllable;

				//controllable.GetInput().enabled = true;
				//controllable.GetInput().ActivateInput();
			}
		}

		public void SwitchImplicitExplicit (InputAction.CallbackContext context) {
			if (context.started) {
				SwitchControl();
			}
		}

		private void SwitchControl (){
			if (currentControl.Equals(playerController) && explicitController != null) {
				currentControl = explicitController;
				input.SwitchCurrentActionMap("Explicit");
				Instance.mode = ControlType.Explicit;
				EventBus.Post(new PlayerEvent.ControlSwitch(PlayerEvent.ControlSwitch.ControlType.Explicit));
			}
			else {
				currentControl = playerController;
				input.SwitchCurrentActionMap("Implicit");
				Instance.mode = ControlType.Explicit;
				EventBus.Post(new PlayerEvent.ControlSwitch(PlayerEvent.ControlSwitch.ControlType.Implicit));
			}
		}

		public static void ResetControl() {
			if (Instance.mode.Equals(ControlType.Explicit)) {
				Instance.SwitchControl();
				Instance.explicitController = null;
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

	public enum ControlType {
		Explicit,
		Implicit
	}
}