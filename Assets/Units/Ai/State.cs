using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

    public abstract class State {

        public abstract string Name { get; protected set; }

        //public abstract Transform Target { get; protected set; }
        
        public delegate void OnEnd();

        public event OnEnd OnComplete;
        
        public abstract void Act (Character actor);

        //Called whenever a Character moves from one state to this one
        public virtual void Enter (Character actor) {

        }

        //Called whenever a Character moves from this state to another one
        public virtual void Exit (Character actor) {

        }

        public bool Complete { get; protected set; } = false;

        protected virtual void End () {
            if (OnComplete != null) OnComplete.Invoke();
        }
    }
}