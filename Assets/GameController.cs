using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;

namespace TankGame {

	public class GameController : MonoBehaviour {

		private GameController instance;

		[SerializeField]
		private GameObject objectHolder;

		[SerializeField]
		private GameObject canvas;

		[SerializeField]
		private GameObject world;

		public static EventBus MainBus { get; private set; }

		private EventBus loadingContextBus;

		private bool init;

		private float upTime = 0f;

		private void Awake () {
			DontDestroyOnLoad(gameObject);

			instance = this;
			init = false;

			//MainBus = new EventBus();
			//loadingContextBus = new EventBus();

			//world.SetActive(true);
			//canvas.SetActive(true);
		}

		private void Update () {
			if (!init) {
				if (upTime >= 1f) {
					EventBus.Post(new InitializationEvent.InitUI(InitializationEvent.InitPhase.Init));
					init = true;
				}

				upTime += Time.deltaTime;
			}
		}

		private void OnDestroy () {
			instance = null;
		}
	}
}