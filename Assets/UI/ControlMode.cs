using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Events;
using UnityEngine.UI;

namespace TankGame.UI {

	public class ControlMode : MonoBehaviour {

		//private Image localImage;
		private Text localText;

		private void Awake () {
			//localImage = GetComponent<Image>();
			localText = GetComponentInChildren<Text>();
		}

		private void Start () {
			EventBus.AddListener<PlayerEvent.ControlSwitch>(OnControlSwitch);
		}

		private void OnControlSwitch (PlayerEvent.ControlSwitch _event) {
			string text = _event.SwitchTo.ToString();

			localText.text = text;
		}
	}
}