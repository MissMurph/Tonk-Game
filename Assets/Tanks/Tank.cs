using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankGame.Tanks.Stations;
using TankGame.Units;
using TankGame.Units.Interactions;

namespace TankGame.Tanks {

    public class Tank : MonoBehaviour, IInteractable {

        private Dictionary<Station, TankStation> stations = new Dictionary<Station, TankStation>();

        public PlayerInput input;

        private List<Character> embarkedCharacters = new List<Character>();

        public InputAction moveAction;

        public Vector2 yeetDirection;

        private void Awake() {
            input = GetComponent<PlayerInput>();

            foreach (TankStation station in GetComponentsInChildren<TankStation>()) {
                stations.Add((Station)Enum.Parse(typeof(Station), station.gameObject.name), station);
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
            return gameObject;
        }

        public List<AbstractInteractionFactory> GetInteractions () {
            List<AbstractInteractionFactory> output = new List<AbstractInteractionFactory>();

            output.Add(new InteractionFactory<Station>("embark", TryEmbark, () => { return new List<Station>(stations.Keys); }, EvaluateSeat));

            return output;
        }

        private InteractionContext<Embark> IEmbark (Embark interaction) {
            if (!interaction.Seat.Occupied) {
                interaction.ActingCharacter.Embark(interaction.Seat.GetController());
                return new InteractionContext<Embark>(interaction, IPhase.Post, IResult.Success);
            }

            Debug.LogWarning(name + " could not embark seat " + interaction.Seat.name);
            return new InteractionContext<Embark>(interaction, IPhase.Post, IResult.Fail);
        }

        private AbstractInteraction TryEmbark (Station seat, Character character, string name) {
            if (stations.TryGetValue(seat, out TankStation station) && !station.Occupied) {
                return new Embark(station, character, name, this, IEmbark);
            }

            Debug.LogWarning("No free station available! Null value provided");
            return null;
        }

        private Station EvaluateSeat (Character character) {
            if (character.GetType().Equals(typeof(PlayerCharacter))) return Station.Commander;

            foreach (KeyValuePair<Station, TankStation> entry in stations) {
                if (!entry.Key.Equals(Station.Commander) && !entry.Value.Occupied) {
                    return entry.Key;
                }
            }

            Debug.LogWarning("No free station available! Null value provided");
            return Station.Driver;
        }

        protected class Embark : AbstractInteraction<Embark> {

            internal TankStation Seat { get; private set; }

            internal Embark (TankStation seat, Character character, string name, IInteractable parent, InteractionFunction destination) : base(destination, character, name, parent) {
                Seat = seat;
            }
        }

        /*public void Interact(Character character) {
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
        }*/

        public enum Station {
            Driver,
            Gunner,
            Loader,
            Commander,
            Coax,
            Auxillary
        }
    }
}