using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Tanks.Systems.Stations {

    public class Driver : Station {

        public Vector2 movingDirection;

        private Rigidbody2D rigidBody;

        private float currentWindUp = 0f;

        public float moveSpeed;
        public float rotationSpeed;
        public float windUpTime;    //in seconds

        private Seat localSeat;

		public override bool Manned {
            get {
                return manningCharacter is not null;
            }
        }

		protected override Character manningCharacter {
            get {
                return localSeat.Occupant;
            }
        }

		protected override void Awake() {
            base.Awake();

            rigidBody = GetComponentInParent<Rigidbody2D>();
            localSeat = GetComponent<Seat>();
        }

        private void FixedUpdate() {
            if (!Manned) return;

            //transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z + (rotationSpeed * movingDirection.x * Time.deltaTime));
            //transform.position += transform.up * movingDirection.y * moveSpeed * Time.deltaTime;
            if (movingDirection != Vector2.zero) {
                currentWindUp += Time.deltaTime;
                currentWindUp = Mathf.Clamp(currentWindUp, 0, windUpTime);
            }

            else currentWindUp = 0f;

            rigidBody.AddForce(transform.up * (movingDirection.y * moveSpeed * (currentWindUp / windUpTime)));

            float impulse = (movingDirection.x * rotationSpeed * Mathf.Deg2Rad) * rigidBody.inertia;

            rigidBody.AddTorque(-impulse * (currentWindUp / windUpTime), ForceMode2D.Impulse);
        }

        public void Move(InputAction.CallbackContext context) {
            if (context.performed) {
                Vector2 result = context.ReadValue<Vector2>();

                movingDirection = result;
            }
            else if (context.canceled) {
                movingDirection = Vector2.zero;
            }
        }

		public override GenericInteraction TryMan(Character character, string name) {
            return localSeat.TrySit(character, "Sit");
		}
	}
}