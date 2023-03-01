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

        List<AbstractInteractionFactory> GetInteractions ();

        List<PreRequisite> GetPreRequisites () {
            return new List<PreRequisite>();
		}
    }
}