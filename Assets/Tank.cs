using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank : MonoBehaviour, IInteractable {
    
    private Dictionary<string, TankSeat> stations = new Dictionary<string, TankSeat>();

    public PlayerInput input;

    private List<Character> embarkedCharacters = new List<Character>();

    public float moveSpeed;
    public float rotationSpeed;

    public Vector2 movingDirection;

    private Rigidbody2D rigidBody;

    public InputAction moveAction;

    public Vector2 yeetDirection;

    //in seconds
    public float windUpTime;

    private float currentWindUp = 0f;

    private void Awake () {
        input = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();

        
        
        foreach (TankSeat station in GetComponentsInChildren<TankSeat>()) {
            stations.Add(station.gameObject.name, station);
		}
    }

    private void FixedUpdate () {
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



    public void Disembark (Character character) {
        if (character.gameObject.transform.parent == this.transform) {
            character.transform.SetParent(null);
            character.transform.position = transform.position + (Vector3.left * 2f);
            embarkedCharacters.Remove(character);
        }
    }

    /*  INPUT FUNCTIONS */

    public void Look (InputAction.CallbackContext context) {
        Debug.Log("Tank Look");
    }

    public void Fire (InputAction.CallbackContext context) {
        Debug.Log("Tank Fire");
    }

    public void Move (Command command, Action<bool> callback) {
        InputAction.CallbackContext context = command.GetAsType<ExplicitMoveCommand>().Target();

        if (context.performed) {
            Vector2 result = context.ReadValue<Vector2>();

            movingDirection = result;
        }
        else if (context.canceled) {
            movingDirection = Vector2.zero;
		}
    }

    public void Zoom (InputAction.CallbackContext context) {
        Debug.Log("Tank Zoom ");
    }

    /*  INTERFACE FUNCTIONS */

    public GameObject GetObject () {
        return this.gameObject;
    }

    public void Interact (Character character) {
        if (character.GetType().Equals(typeof(PlayerCharacter)) && stations.TryGetValue("CommandStation", out TankSeat station)) {
            character.Embark(station.GetController());
            return;
		}

        foreach (TankSeat seat in stations.Values) {
            if (seat.Occupied && seat.name != "CommandStation") continue;

            character.Embark(seat.GetController());
            embarkedCharacters.Add(character);
        }
    }
}