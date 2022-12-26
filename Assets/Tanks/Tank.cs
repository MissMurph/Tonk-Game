using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankGame.Tanks.Stations;
using TankGame.Units;
using TankGame.Units.Interactions;
using TankGame.Tanks.Systems.Stations;

namespace TankGame.Tanks {

    public class Tank : MonoBehaviour, IInteractable {

        private Dictionary<Stations, Station> stations = new Dictionary<Stations, Station>();

        public PlayerInput input;

        //private List<Character> embarkedCharacters = new List<Character>();

        public InputAction moveAction;

        public Vector2 yeetDirection;

		[SerializeField]
        private Port[] embarkPorts;

        private void Awake() {
            input = GetComponent<PlayerInput>();

            foreach (Station station in GetComponentsInChildren<Station>()) {
                stations.Add((Stations)Enum.Parse(typeof(Stations), station.gameObject.name), station);
            }
        }

        /*public void Disembark(Character character) {
            if (embarkedCharacters.Contains(character)) {
                embarkedCharacters.Remove(character);
            }
        }*/

        public List<Station> GetStations() {
            return new List<Station>(stations.Values);
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
            return new List<AbstractInteractionFactory>() {
                new GenericInteractionFactory("Embark", TryEmbark),
                new GenericInteractionFactory("Disembark", TryDisembark)
            };
        }

        private GenericInteraction TryEmbark (Character character, string name) {
            if (!ReferenceEquals(character.EmbarkedVehicle, this)) return null;

            float lowestDist = 100f;
            Port closestPort = null;

            foreach (Port port in embarkPorts) {
                float newDist = (character.transform.position - port.transform.position).magnitude;

                if (newDist < lowestDist) {
                    lowestDist = newDist;
                    closestPort = port;
                }
			}

            return closestPort.TryUsePort(character, name);
		}

        private GenericInteraction TryDisembark(Character character, string name) {
            if (ReferenceEquals(character.EmbarkedVehicle, this)) return null;

            float lowestDist = 100f;
            Port closestPort = null;

            foreach (Port port in embarkPorts) {
                float newDist = (character.transform.position - port.transform.position).magnitude;

                if (newDist < lowestDist) {
                    lowestDist = newDist;
                    closestPort = port;
                }
            }

            return closestPort.TryUsePort(character, name);
        }

        /*private InteractionContext<Embark> IEmbark (Embark interaction) {
            if (!interaction.Seat.Occupied) {
                interaction.ActingCharacter.Embark(interaction.Seat.GetController());
                return new InteractionContext<Embark>(interaction, IPhase.Post, IResult.Success);
            }

            Debug.LogWarning(name + " could not embark seat " + interaction.Seat.name);
            return new InteractionContext<Embark>(interaction, IPhase.Post, IResult.Fail);
        }

        private AbstractInteraction TryEmbark (Station seat, Character character, string name) {
            if (stations.TryGetValue(seat, out Station station) && !station.Occupied) {
                return new Embark(station, character, name, this, IEmbark);
            }

            Debug.LogWarning("No free station available! Null value provided");
            return null;
        }

        private Station EvaluateSeat (Character character) {
            if (character.GetType().Equals(typeof(PlayerCharacter))) return Station.Commander;

            foreach (KeyValuePair<Station, Station> entry in stations) {
                if (!entry.Key.Equals(Station.Commander) && !entry.Value.Occupied) {
                    return entry.Key;
                }
            }

            Debug.LogWarning("No free station available! Null value provided");
            return Station.Driver;
        }*/

        protected class Embark : AbstractInteraction<Embark> {

            internal Station Seat { get; private set; }

            internal Embark (Station seat, Character character, string name, IInteractable parent, InteractionFunction destination) : base(destination, character, name, parent) {
                Seat = seat;
            }
        }

        /*public void Interact(Character character) {
            Debug.Log("Interact");
            if (character.GetType().Equals(typeof(PlayerCharacter)) && stations.TryGetValue("CommandStation", out Station station)) {
                character.Embark(station.GetController());
                return;
            }

            Debug.Log("Interact Not Player");
            foreach (Station seat in stations.Values) {
                if (seat.Occupied || seat.name == "CommandStation") continue;

                Debug.Log("Seat " + seat.name + " is not occupied");
                character.Embark(seat.GetController());
                embarkedCharacters.Add(character);
                break;
            }
        }*/

        public enum Stations {
            Driver,
            Gunner,
            Loader,
            Commander,
            Coax,
            Auxillary
        }
    }
}