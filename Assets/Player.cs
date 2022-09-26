using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour {

	[SerializeField]
	private PlayerController playerController;

	public World world;

	public IControllable currentControl;

	public void SwitchControl (IControllable controllable) {
		//Debug.Log("Old: " + currentControl.GetObject().name + "   New: " + controllable.GetObject().name);
		if (currentControl.GetObject().name != controllable.GetObject().name) {
			currentControl.GetInput().enabled = false;
			
			currentControl = controllable;

			controllable.GetInput().enabled = true;
			controllable.GetInput().ActivateInput();
		}
	}

	public void ResetControl () {
		if (currentControl.GetType().Equals(playerController.GetType())) {
			SwitchControl(playerController);
		}
	}

	private void Awake () {
		playerController = GetComponent<PlayerController>();
		currentControl = playerController;
	}

	private void Update () {
		if (currentControl != null && currentControl.GetObject() != this.gameObject) {
			GameObject target = currentControl.GetObject();
			transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
		}
	}
}