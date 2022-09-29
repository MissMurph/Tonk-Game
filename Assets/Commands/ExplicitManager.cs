using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplicitManager : CommandManager {

	protected override void Start () {}

	public override void EnqueueCommand(Command command) {
		ExecuteCommand(command);
	}

	public override void ExecuteCommand(Command command) {
		PerformCommand(command);
	}
}