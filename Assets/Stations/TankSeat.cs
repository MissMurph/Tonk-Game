using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSeat : MonoBehaviour {
	public bool Occupied { get; private set; }

	private IControllable inputReceiver;

	private void Awake() {
		Occupied = false;
		inputReceiver = GetComponent<IControllable>();
	}

	public IControllable GetController () {
		Occupied = true;
		return inputReceiver;
	}

	public void Disembark () {
		Occupied = false;
	}
}