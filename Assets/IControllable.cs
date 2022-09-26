using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IControllable {
	PlayerInput GetInput ();
	GameObject GetObject ();
}