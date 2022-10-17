using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankGame.Tanks.Stations;
using TankGame.Units;

namespace TankGame.Tanks {

    public class Tank : MonoBehaviour, IInteractable {

        private Dictionary<string, TankStation> stations = new Dictionary<string, TankStation>();

        public PlayerInput input;

        private List<Character> embarkedCharacters = new List<Character>();

        public InputAction moveAction;

        public Vector2 yeetDirection;

        private void Awake() {
            input = GetComponent<PlayerInput>();

            foreach (TankStation station in GetComponentsInChildren<TankStation>()) {
                stations.Add(station.gameObject.name, station);
            }
        }

        public void Disembark(Character character) {
            if (embarkedCharacters.Contains(character)) {
                embarkedCharacters.Remove(character);
            }
        }

        public List<TankStation> GetStations() {
            return new List<TankStation>(stations.Values);
        }

        /*  INPUT FUNCTIONS */

        public void Look(InputAction.CallbackContext context) {
            Debug.Log("Tank Look");
        }

        public void Fire(InputAction.CallbackContext context) {
            Debug.Log("Tank Fire");
        }

        public void Zoom(InputAction.CallbackContext context) {
            Debug.Log("Tank Zoom ");
        }

        /*  INTERFACE FUNCTIONS */

        public GameObject GetObject() {
            return this.gameObject;
        }

        public void Interact(Character character) {
            Debug.Log("Interact");
            if (character.GetType().Equals(typeof(PlayerCharacter)) && stations.TryGetValue("CommandStation", out TankStation station)) {
                character.Embark(station.GetController());
                return;
            }

            Debug.Log("Interact Not Player");
            foreach (TankStation seat in stations.Values) {
                if (seat.Occupied || seat.name == "CommandStation") continue;

                Debug.Log("Seat " + seat.name + " is not occupied");
                character.Embark(seat.GetController());
                embarkedCharacters.Add(character);
                break;
            }


        }
    }
}