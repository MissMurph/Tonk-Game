using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

    public abstract class State {

        public abstract string Name { get; protected set; }

        public abstract Transform Target { get; protected set; }
        
        public abstract void Act (Character actor);
    }
}