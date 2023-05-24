using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankGame.Units;
using TankGame.Units.Interactions;
using TankGame.Tanks.Systems.Stations;
using TankGame.Units.Navigation;
using TankGame.Units.Ai;

namespace TankGame.Tanks {

    public class Tank : MonoBehaviour, IInteractable, ITraversable {

        private Dictionary<Stations, Station> stations = new Dictionary<Stations, Station>();

        public PlayerInput input;

        //private List<Character> embarkedCharacters = new List<Character>();

        public InputAction moveAction;

        public Vector2 yeetDirection;

		[SerializeField]
        private Port[] embarkPorts;

        private Source manager;

        private void Awake() {
            input = GetComponent<PlayerInput>();
            manager = GetComponent<Source>();

            foreach (Station station in GetComponentsInChildren<Station>()) {
                stations.Add((Stations)Enum.Parse(typeof(Stations), station.gameObject.name), station);
            }

            foreach (Character character in GetComponentsInChildren<Character>()) {
                character.StateMachine.SubmitPreRequisite("embarked", new NeedsToDisembark(this), new Interacting(TryEmbark, "Disembark"));
                character.IntManager.SubmitPreRequisite("embarked", new NeedsToEmbark(this), new Interacting(TryEmbark, "Embark"));
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

        public GenericInteraction TryEmbark (Character character, string name) {
            if (name == "Embark") {
                if (ReferenceEquals(character.Traversable, this)) return null;

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
            else if (name == "Disembark") {
                if (!ReferenceEquals(character.Traversable, this)) return null;

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
            else return null;
		}

        private GenericInteraction TryDisembark(Character character, string name) {
            if (ReferenceEquals(character.Traversable, this)) return null;

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

        public void FindPath (PathRequest request, Action<PathResult> callback) {
            Vector3[] outPut = new Vector3[1] { transform.InverseTransformPoint(request.pathEnd.position) };
            callback(new PathResult(outPut, true, request.callback));
        }

		public Source GetManager() {
            return manager;
		}

		protected class Embark : AbstractInteraction<Embark> {

            internal Station Seat { get; private set; }

            internal Embark (Station seat, Character character, string name, IInteractable parent, InteractionFunction destination) : base(destination, character, name, parent) {
                Seat = seat;
            }
        }

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