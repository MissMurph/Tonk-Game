using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TankGame.Units;

namespace TankGame.Units.Interactions {

    public interface IInteractable {
        GameObject GetObject();

        //void Interact (AbstractInteraction interaction);

        List<AbstractInteractionFactory> GetInteractions ();

        //List<Command> Commands();

        //Command GetInteraction();
    }
}