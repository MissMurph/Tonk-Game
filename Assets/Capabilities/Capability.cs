using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Capabilities {
	//This is an early version honestly closer to resembling NBT tags

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

	public interface ICapability {
		abstract Type Type { get; }
		abstract string Name { get; }
		abstract string String ();
		abstract int Int ();
		abstract float Float ();
		abstract double Double ();
		abstract bool Bool ();
	}
}