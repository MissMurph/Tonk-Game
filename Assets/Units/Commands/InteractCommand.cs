using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractCommand : Command<IInteractable> {

	public InteractCommand (IInteractable target) : base(target, "interact") {}
}