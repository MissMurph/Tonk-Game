using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Util;
using UnityEngine;

namespace TankGame.Units.Ai {

	[Serializable]
	public abstract class Number : IEvaluator {

		[SerializeField] public Operations[] Operation { get; private set; } = new Operations[2];

		public abstract int GetInt ();
		public abstract float GetFloat ();
		public abstract double GetDouble ();
		public abstract string GetString ();

		public abstract bool Act (Character character);

		public abstract string Name ();

		//public abstract IEvaluator Copy ();
	}
}