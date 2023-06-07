using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TankGame.Units;
using TankGame.Units.Ai;

namespace TankGame.Units.Interactions {

    public interface IInteractable {
        GameObject GetObject();

        void OnCollection (Collector collector);

        Source GetManager();
    }
}