using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Commands {

    public class CommandBuilder : SerializedMonoBehaviour {

        [OdinSerialize]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public Command GetTree (string name) {
            return commands.GetValueOrDefault(name);
        }

        /*public Command GetDefault (Character character) {

        }*/
    }
}