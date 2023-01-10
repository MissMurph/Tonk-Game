using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public interface IEvaluator {
		bool Act ();
		string Name ();

		IEvalBuilder Builder();
	}

	public interface IEvalSerializer<T> {
		IEvaluator Deserialize ();
	}

	public interface IEvalBuilder {
		IEvaluator Build ();
	}
}