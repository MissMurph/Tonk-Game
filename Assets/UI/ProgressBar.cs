using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.UI {

	public class ProgressBar : MonoBehaviour {

		[SerializeField]
		private RectTransform movingBar;

		public float Progress { get; private set; } = 0;

		public void Set (float value) {
			Progress = Mathf.Clamp(value, 0, 100);
			movingBar.position = new Vector3(100 - Progress, 0, 0);
		}
	}
}