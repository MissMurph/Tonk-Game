using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretStation : TankStation {

	//private Vector2 mousePos;

	//In degrees per second
	[SerializeField]
	private float rotateSpeed;

	[SerializeField]
	private Rigidbody2D turretRigidBody;

	private Vector3 lookDirection;

	private float lookAngle;

	Vector3 mousePos;

	private float currentAngle {
		get {
			return turretRigidBody.gameObject.transform.up.z;
		}
	}

	protected override void Awake () {
		base.Awake();


	}

	private void FixedUpdate () {
		int posOrNeg = currentAngle - lookAngle > 0 ? 1 : -1;

		Quaternion newRot = Quaternion.LookRotation(lookDirection);

		turretRigidBody.MoveRotation(lookAngle);
	}

	public void Look (InputAction.CallbackContext context) {
		mousePos = context.ReadValue<Vector2>();

		Debug.Log(mousePos);

		lookDirection = (Camera.main.ScreenToWorldPoint(mousePos) - turretRigidBody.gameObject.transform.position);

		//lookAngle = Mathf.Atan(lookDirection.x / lookDirection.y) * Mathf.Rad2Deg;

		//Debug.Log(lookAngle);
	}

	private void OnDrawGizmos () {
		Gizmos.color = Color.cyan;

		Gizmos.DrawLine(transform.position, mousePos);
	}
}