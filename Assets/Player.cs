using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

	private List<ISelectable> selectables = new List<ISelectable>();

	public void Start() {
		
	}

	public void Select (InputAction.CallbackContext context) {
		Debug.Log("cum");
	}
}