using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

    public abstract class State {

        public string Name { get; private set; }
        public int BaseWeight { get; private set; }
        public int Weight { get; private set; }

        public List<State> Next { get; private set; }
        
        public abstract void Act ();
    }
}