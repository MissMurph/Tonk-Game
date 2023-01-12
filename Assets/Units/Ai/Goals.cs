using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TankGame.Units.Ai {

    public class Goals : SerializedMonoBehaviour {

        [SerializeField]
        private Dictionary<string, Goal> goals = new Dictionary<string, Goal>();

        private void Awake () {

        }

        
    }

    /*[Serializable]
    public class GoalEntry {
        public string name;
        public int baseWeight;
        public DecisionEntry[] decisions;
        public int[] startNodes;
    }

    [Serializable]
    public class DecisionEntry {
        public string stateName;
        public int baseWeight;
        public int[] nextNodes;     //Each decision will have an ID corresponding to position in array
    }*/
}