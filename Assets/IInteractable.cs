using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TankGame.Units;

namespace TankGame {

    public interface IInteractable {
        void Interact(Character character);

        GameObject GetObject();

        //List<Command> Commands();

        //Command GetInteraction();
    }
}