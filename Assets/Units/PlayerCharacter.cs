using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character, IControllable {

	public PlayerInput input;
	public Player player;

	public PlayerInput GetInput () {
		return input;
	}

	public override void Embark (Tank tank) {
		base.Embark(tank);

		player.SwitchControl(tank);
	}

	public override void Disembark () {
		base.Disembark();

		player.ResetControl();
	}
}