using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Navigation {

	public interface ITraversable {
		void FindPath (PathRequest request, Action<PathResult> callback);
		GameObject GetObject ();
	}
}