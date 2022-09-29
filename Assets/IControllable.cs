using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IControllable {
	//PlayerInput GetInput ();
	void Input(InputAction.CallbackContext context);
	GameObject GetObject ();
}