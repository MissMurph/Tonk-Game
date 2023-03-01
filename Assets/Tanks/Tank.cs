using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankGame.Tanks.Stations;
using TankGame.Units;
using TankGame.Units.Interactions;
using TankGame.Tanks.Systems.Stations;
using TankGame.Units.Navigation;

namespace TankGame.Tanks {

    public class Tank : MonoBehaviour, IInteractable, ITraversable {

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

        public GenericInteraction TryEmbark (Character character, string name) {
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
            Vector3[] outPut = new Vector3[1] { request.pathEnd.localPosition };
            callback(new PathResult(outPut, true, request.callback));

            if (!ReferenceEquals(request.originTraversable, request.targetTraversable) && ReferenceEquals(this, request.targetTraversable)) {

            }
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