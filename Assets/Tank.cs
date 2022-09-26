using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank : MonoBehaviour, IInteractable, IControllable {

    private List<TankStation> stations = new List<TankStation>();

    public PlayerInput input;

    private List<Character> embarkedCharacters = new List<Character>();

    public float moveSpeed;
    public float rotationSpeed;

    private Vector2 movingDirection;

    private Rigidbody2D rigidBody;

    private void Awake () {
        input = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update () {
        //transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z + (rotationSpeed * movingDirection.x * Time.deltaTime));
        //transform.position += transform.up * movingDirection.y * moveSpeed * Time.deltaTime;
        

    }



    public void Disembark (Character character) {
        if (character.gameObject.transform.parent == this.transform) {
            character.transform.SetParent(null);
            character.transform.position = transform.position + (Vector3.left * 2f);
            embarkedCharacters.Remove(character);
        }
    }

    public List<TankStation> Stations () {
        return stations;
	}

    /*  INPUT FUNCTIONS */

    public void Look (InputAction.CallbackContext context) {
        Debug.Log("Tank Look");
    }

    public void Fire (InputAction.CallbackContext context) {
        Debug.Log("Tank Fire");
    }

    public void Move (InputAction.CallbackContext context) {
        if (!context.started) return;
        
        Vector2 result = context.ReadValue<Vector2>();

        movingDirection = result;

        
    }

    public void Zoom (InputAction.CallbackContext context) {
        Debug.Log("Tank Zoom ");
    }

    /*  INTERFACE FUNCTIONS */

    public PlayerInput GetInput () {
        return input;
    }

    public GameObject GetObject () {
        return this.gameObject;
    }

    public void Interact (Character character) {
        character.Embark(this);
        embarkedCharacters.Add(character);
    }
}