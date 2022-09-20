using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    private List<TankStation> stations = new List<TankStation>();

    public void Interact(Character character) {
        character.transform.SetParent(this.transform, true);
        character.transform.position = Vector3.zero;
    }

    public List<TankStation> Stations () {
        return stations;
	}

    private void Start() {
        
    }

    private void Update() {
        
    }
}