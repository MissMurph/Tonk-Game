using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Commands;
using UnityEngine;

namespace TankGame.Units.Ai {

	public interface IEvaluator {
		bool Act (Character character);
		string Name ();

		//Override this function to inject Command data straight into an evaluator
		void DecisionInjector (Decision decision) { }
	}
}