using System.Collections;
using System.Collections.Generic;
using TankGame.Players;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Tanks.Systems.Stations {

	public class Turret : Station {

		//private Vector2 mousePos;

		//In degrees per second
		[SerializeField]
		private float rotateSpeed;

		private float CurrentAngle {
			get {
				return turretRigidBody.rotation;
			}
		}

		private Seat localSeat;

		public override bool Manned {
			get
			{
				return manningCharacter is not null;
			}
		}

		protected override Character manningCharacter {
			get
			{
				return localSeat.Occupant;
			}
		}

		[SerializeField]
		private Rigidbody2D turretRigidBody;

		private float lookAngle;

		float totalMinDiff;
		float totalMaxDiff;
		int posOrNeg;
		Vector3 lookDirection;

		protected override void Awake() {
			base.Awake();

			localSeat = GetComponent<Seat>();
		}

		protected override void Start () {
			base.Start();

			manager.AddListener<GenericInteraction>("Sit", OnSit);
			manager.AddListener<GenericInteraction>("Unsit", OnUnsit);
		}

		private void FixedUpdate() {
			if (!Manned) return;

			float distToMin = Mathf.Abs(lookAngle);
			float distToMax = 180f - distToMin;

			float currentDistToMin = Mathf.Abs(CurrentAngle);
			float currentDistToMax = 180f - currentDistToMin;

			totalMinDiff = distToMin + currentDistToMin;
			totalMaxDiff = distToMax + currentDistToMax;

			int sameSide = (int)(Mathf.Sign(lookAngle) * Mathf.Sign(CurrentAngle));

			posOrNeg = 1;

			float finalDist = totalMinDiff;

			if (totalMaxDiff < totalMinDiff) {
				posOrNeg *= -1;
				finalDist = totalMaxDiff;
			}

			float gigaAngle = Mathf.MoveTowardsAngle(CurrentAngle, lookAngle, rotateSpeed * Time.fixedDeltaTime);

			if (CurrentAngle > 0) posOrNeg *= -1;

			turretRigidBody.MoveRotation(gigaAngle);
		}

		public void Look(InputAction.CallbackContext context) {
			if (!Manned) return;

			Vector3 mousePos = context.ReadValue<Vector2>();

			lookDirection = (Camera.main.ScreenToWorldPoint(mousePos) - turretRigidBody.gameObject.transform.position);

			lookAngle = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90;
		}

		public void Fire(InputAction.CallbackContext context) {
			if (!Manned) return;

			if (context.started) {
				Debug.Log("Firing...");
			}
		}

		private void OnSit (InteractionContext<GenericInteraction> context) {
			if (ReferenceEquals(context.Interaction.ActingCharacter, Player.PlayerCharacter)) {
				Player.SwitchControl(InputReceiver);
			}
		}

		private void OnUnsit (InteractionContext<GenericInteraction> context) {
			if (ReferenceEquals(context.Interaction.ActingCharacter, Player.PlayerCharacter)) {
				Player.ResetControl();
			}
		}

		public override GenericInteraction TryMan(Character character, string name) {
			return localSeat.TrySit(character, "Sit");
		}
	}
}