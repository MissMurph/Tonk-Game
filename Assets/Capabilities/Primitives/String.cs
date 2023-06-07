using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TankGame.Capabilities {
	public class String : ICapability {
		public Type Type { get { return typeof(string); } }

		public string Name { get; private set; }

		private string value;

		public String (string _name, string _value) {
			Name = _name;
			value = _value;
		}

		//Will always return false, would like to have this parse the words "true" & "false", could make a bunch of parsing rules
		public bool Bool () {
			return false;
		}

		public double Double () {
			return 0;
		}

		public float Float () {
			return 0;
		}

		public int Int () {
			return 0;
		}

		string ICapability.String () {
			return value;
		}
	}
}