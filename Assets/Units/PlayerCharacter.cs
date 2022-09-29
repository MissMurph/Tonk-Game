using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character {

	public PlayerInput input;
	public Player player;

	public PlayerInput GetInput () {
		return input;
	}

	public override void Embark (IControllable seat) {
		base.Embark(seat);

		player.SwitchControl(seat);
	}

	public override void Disembark () {
		base.Disembark();

		player.ResetControl();
	}
}