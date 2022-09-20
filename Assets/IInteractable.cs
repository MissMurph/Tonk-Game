using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable {
    void Interact (Character character);

    //List<Command> Commands();

    //Command GetInteraction();
}