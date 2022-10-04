using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretStation : TankStation {

	//private Vector2 mousePos;

	//In degrees per second
	[SerializeField]
	private float rotateSpeed;

	private float rotateStep {
		get {
			return rotateSpeed * Time.fixedDeltaTime;
		}
	}
	
	private float currentAngle {
		get {
			return turretRigidBody.rotation;
		}
	}

	[SerializeField]
	private Rigidbody2D turretRigidBody;

	private float lookAngle;

	float totalMinDiff;
	float totalMaxDiff;
	int posOrNeg;
	Vector3 lookDirection;

	protected override void Awake () {
		base.Awake();


	}

	private void FixedUpdate () {
		if (!Occupied) return;

		

		//Debug.Log("currentAngle: " + currentAngle);

		//Quaternion newRot = Quaternion.LookRotation(lookDirection);

		float distToMin = Mathf.Abs(lookAngle);
		float distToMax = 180f - distToMin;

		float currentDistToMin = Mathf.Abs(currentAngle);
		float currentDistToMax = 180f - currentDistToMin;

		totalMinDiff = distToMin + currentDistToMin;
		totalMaxDiff = distToMax + currentDistToMax;

		int sameSide = (int) (Mathf.Sign(lookAngle) * Mathf.Sign(currentAngle));



		//Quaternion.Slerp(turretRigidBody.transform.rotation, newRot, rotateSpeed * Time.fixedDeltaTime);

		posOrNeg = 1;

		float finalDist = totalMinDiff;

		if (totalMaxDiff < totalMinDiff) {
			posOrNeg *= -1;
			finalDist = totalMaxDiff;
		}

		float gigaAngle = Mathf.MoveTowardsAngle(currentAngle, lookAngle, rotateSpeed * Time.fixedDeltaTime);

		if (currentAngle > 0) posOrNeg *= -1;

		//float finalAngle = finalDist > rotateStep ? currentAngle + (posOrNeg * rotateSpeed * Time.fixedDeltaTime) : lookAngle;

		//Debug.Log("finalAngle: " + finalAngle);

		//turretRigidBody.SetRotation(finalAngle);

		turretRigidBody.MoveRotation(gigaAngle);
	}

	public void Look (InputAction.CallbackContext context) {
		Vector3 mousePos = context.ReadValue<Vector2>();

		lookDirection = (Camera.main.ScreenToWorldPoint(mousePos) - turretRigidBody.gameObject.transform.position);

		lookAngle = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90;
	}

	public void ReadAlgorithm (InputAction.CallbackContext context) {
		if (context.started) {
			/*Debug.Log("lookDirection: " + lookDirection);
			Debug.Log("currentAngle: " + currentAngle);
			Debug.Log("lookAngle: " + lookAngle);
			Debug.Log("totalMinDiff: " + totalMinDiff);
			Debug.Log("totalMaxDiff: " + totalMaxDiff);
			Debug.Log("Direction: " + posOrNeg);*/

			Debug.Log("Firing...");
		}
	}

	//Both of these are used to move back and forth between border values on the angle so turret smoothing will always take the shortest path
	//Unity measures its rotational values as two 180 degree sides (positive and negative).
	//In order to cross over the boundaries between both sides we need to measure the difference between the value and the relative border.
}