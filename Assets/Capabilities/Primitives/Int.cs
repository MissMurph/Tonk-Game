using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Capabilities {

	public class Int : ICapability {
		public Type Type { get { return typeof(int); } }

		public string Name { get; private set; }

		private int value;

		public Int (string _name, int _value) {
			Name = _name;
			value = _value;
		}

		public bool Bool () {
			return false;
		}

		public double Double () {
			return value;
		}

		public float Float () {
			return value;
		}

		int ICapability.Int () {
			return value;
		}

		public string String () {
			return value.ToString();
		}
	}
}