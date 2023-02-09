using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public interface IEvaluator {
		bool Act ();
		string Name ();

		//IEvaluator Copy();
	}
}