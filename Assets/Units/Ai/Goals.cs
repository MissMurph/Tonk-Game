using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

    public class Goals : MonoBehaviour {

        [SerializeField]
        private GoalEntry[] goals;

        public static Dictionary<string, Goal> LoadedGoals { get; private set; }

        private void Awake () {
            foreach (GoalEntry entry in goals) {

                Decision[] nodes = new Decision[entry.decisions.Length];

                for (int i = 0; i < nodes.Length; i++) {
                    DecisionEntry dEntry = entry.decisions[i];
                    //nodes[i] = new Decision();
                }
            }
        }

        
    }

    [Serializable]
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
    }
}