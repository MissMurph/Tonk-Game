using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Items;
using TankGame.Units.Interactions;
using TankGame.Units;

namespace TankGame.Events {

    public class InteractionEvent<T> : AbstractEvent where T : AbstractInteraction<T> {

        public IPhase Phase { get; private set; }
        public IResult Result { get; private set; }
        public Character ActingCharacter { get; private set; }
        public T Interaction { get; private set; }

        public InteractionEvent (InteractionContext<T> _context) : base("interaction:" + _context.Name) {
            Interaction = _context.Interaction;
            ActingCharacter = _context.Interaction.ActingCharacter;
            Phase = _context.Phase;
            Result = _context.Result;
        }
    }
}