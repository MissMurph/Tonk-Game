using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

	private List<ISelectable> selectables = new List<ISelectable>();

	public Grid grid;

	public void Fire (InputAction.CallbackContext context) {

	}

	public void RightClick (InputAction.CallbackContext context) {
		
	}

	public void Move (InputAction.CallbackContext context) {
		
	}
}