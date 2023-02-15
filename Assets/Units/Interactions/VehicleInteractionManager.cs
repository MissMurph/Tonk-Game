using System.Collections;
using System.Collections.Generic;
using TankGame.Tanks;
using TankGame.Units.Commands;
using TankGame.Units.Navigation;
using UnityEngine;

namespace TankGame.Units.Interactions {

    public class VehicleInteractionManager : InteractionManager {

        [SerializeField]
        private Tank parent;

        private PathRequestManager parentTraversable;
        private InteractionManager parentIntManager;

        protected override void Awake () {
            parentTraversable = parent.GetComponent<PathRequestManager>();
            parentIntManager = parent.GetComponent<InteractionManager>();
        }

        //This is a cheeky hack method. Character will immediately execute whatever interaction it collects, so if we need to enqueue two commands to cross traversable
        //we can return null so the controller wont execute the base one, then enqueue both needed commands
        public override AbstractInteraction RequestInteraction<T> (string name, T target, Character character) {
            AbstractInteraction baseInt = base.RequestInteraction(name, target, character);

            if (ReferenceEquals(character.Traversable, parentTraversable)) 
                return baseInt;
            else {
                AbstractInteraction embarkInt = parentIntManager.RequestInteraction("Embark", character);
                character.ExecuteCommand(new Interact(embarkInt));
                character.EnqueueCommand(new Interact(baseInt));
                return null;
            }
        }
    }
}