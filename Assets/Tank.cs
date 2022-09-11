using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour, IInteractable {

    private List<TankStation> stations = new List<TankStation>();

    [SerializeField]
    private Command embark;

    public void Interact(Character character) {
        character.transform.SetParent(this.transform, true);
        character.transform.position = Vector3.zero;
    }

    public List<Command> Commands () {
        return null;
	}

    public List<TankStation> Stations () {
        return stations;
	}

    private void Start() {
        
    }

    private void Update() {
        
    }

	public Command GetInteraction() {
        return null;
	}
}