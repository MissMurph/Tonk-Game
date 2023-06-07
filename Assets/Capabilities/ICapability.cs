using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Capabilities {
	//This is an early version honestly closer to resembling NBT tags

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