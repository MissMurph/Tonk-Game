using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IControllable {

	void Input(InputAction.CallbackContext context);
	GameObject GetObject();
}