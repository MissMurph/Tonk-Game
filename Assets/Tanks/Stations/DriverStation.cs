using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Tanks.Stations {

    public class DriverStation : TankStation {

        public Vector2 movingDirection;

        private Rigidbody2D rigidBody;

        private float currentWindUp = 0f;

        public float moveSpeed;
        public float rotationSpeed;
        public float windUpTime;    //in seconds

        protected override void Awake() {
            base.Awake();

            rigidBody = GetComponentInParent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            if (!Occupied) return;

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
    }
}