using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCommand : Command<IInteractable> {

	public InteractCommand (IInteractable target) : base(target, "interact") {

	}
}