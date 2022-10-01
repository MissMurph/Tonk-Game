using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank : MonoBehaviour, IInteractable {
    
    private Dictionary<string, TankStation> stations = new Dictionary<string, TankStation>();

    public PlayerInput input;

    private List<Character> embarkedCharacters = new List<Character>();

    public InputAction moveAction;

    public Vector2 yeetDirection;

    private void Awake () {
        input = GetComponent<PlayerInput>();
        
        foreach (TankStation station in GetComponentsInChildren<TankStation>()) {
            stations.Add(station.gameObject.name, station);
		}
    }

    public void Disembark (Character character) {
        if (character.gameObject.transform.parent == this.transform) {
            character.transform.SetParent(null);
            character.transform.position = transform.position + (Vector3.left * 2f);
            embarkedCharacters.Remove(character);
        }
    }

    public List<TankStation> GetStations () {
        return new List<TankStation>(stations.Values);
	}

    /*  INPUT FUNCTIONS */

    public void Look (InputAction.CallbackContext context) {
        Debug.Log("Tank Look");
    }

    public void Fire (InputAction.CallbackContext context) {
        Debug.Log("Tank Fire");
    }

    public void Zoom (InputAction.CallbackContext context) {
        Debug.Log("Tank Zoom ");
    }

    /*  INTERFACE FUNCTIONS */

    public GameObject GetObject () {
        return this.gameObject;
    }

    public void Interact (Character character) {
        if (character.GetType().Equals(typeof(PlayerCharacter)) && stations.TryGetValue("CommandStation", out TankStation station)) {
            character.Embark(station.GetController());
            return;
		}

        foreach (TankStation seat in stations.Values) {
            if (seat.Occupied && seat.name != "CommandStation") continue;

            character.Embark(seat.GetController());
            embarkedCharacters.Add(character);
        }
    }
}