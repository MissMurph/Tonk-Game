using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Items;
using TankGame.Units.Interactions;
using TankGame.Units;

namespace TankGame.Events {

    public class InteractionEvent : AbstractEvent {

        public Interactionlet Interaction { get; private set; }
        public IPhase Phase { get; private set; }
        public IResult Result { get; private set; }
        public IStep Step { get; private set; }
        public ISide Side { get; private set; }

        public Character Actor;

        public InteractionEvent (Interactionlet _packet, Character _actor, ISide _side) : base("interaction:" + _packet.) {
            Interaction = _packet;
            Actor = _actor;
            Phase = _packet.Phase;
            Result = _packet.Result;
            Step = _packet.Step;
            Side = _side;
        }

        public void Cancel () {
            Result = IResult.Cancel;
        }
    }
}