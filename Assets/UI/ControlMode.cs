using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Events;
using UnityEngine.UI;
using TMPro;

namespace TankGame.UI {

	public class ControlMode : MonoBehaviour {

		//private Image localImage;
		[SerializeField]
		private TextMeshProUGUI localText;

		private void Awake () {
			//localImage = GetComponent<Image>();
			localText = GetComponentInChildren<TextMeshProUGUI>();
		}

		private void Start () {
			EventBus.Subscribe<PlayerEvent.ControlSwitch>(OnControlSwitch);
		}

		private void OnControlSwitch (PlayerEvent.ControlSwitch _event) {
			string text = _event.SwitchTo.ToString();

			localText.text = text;
		}
	}
}