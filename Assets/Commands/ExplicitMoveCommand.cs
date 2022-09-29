using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExplicitMoveCommand : ExplicitCommand {

	public ExplicitMoveCommand(InputAction.CallbackContext context) : base(context, "explicit_move") { }
}