using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ExplicitCommand : Command<InputAction.CallbackContext> {
	protected ExplicitCommand(InputAction.CallbackContext context, string _name) : base(context, _name) {}
}