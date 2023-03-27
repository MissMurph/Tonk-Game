using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Ai;
using TankGame.Units.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Commands : SerializedMonoBehaviour {

		private static Commands instance;

		[OdinSerialize]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		private Dictionary<string, Command> commands = new Dictionary<string, Command>();

		private void Awake () {
			instance = this;
		}

		private void Start() {

		}

		public static Command GetTree (string name) {
			return instance.commands.GetValueOrDefault(name);
		}

		private void OnDestroy () {
			instance = null;
		}
	}
}