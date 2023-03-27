using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Units.Ai {

    //public delegate AbstractInteraction Yeet<T> (string name, T target, Character character);

    public class Embarking : Interacting {

        private InteractionManager source;
        private string interactionName;

        public override void Enter (Character actor) {
            base.Enter(actor);


        }

        public override void Act (Character actor) {
            throw new System.NotImplementedException();
        }
    }
}