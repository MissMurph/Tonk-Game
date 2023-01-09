using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class StateMachine : MonoBehaviour {

		public State State { get; private set; }

		private float stateUpdateTime = 0.25f;  //Four updates per second

		protected virtual void Start () {
			StartCoroutine(UpdateState());
		}

		IEnumerator UpdateState () {
			if (Time.timeSinceLevelLoad < 1f) {
				yield return new WaitForSeconds(1f);
			}

			while (true) {
				yield return new WaitForSeconds(stateUpdateTime);

				/*if (!ExecutingCommand && commandQueue.TryDequeue(out Command command)) {
					StopCoroutine(currentCoroutine);
					PerformCommand(command);
				}
				else if (ActiveCommand != null) {
					ActiveCommand.Perform();
				}
				*/
			}
		}
	}
}