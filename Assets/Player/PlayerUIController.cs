using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using TankGame.UI;
using TankGame.Events;
using TankGame.Units;
using TankGame.Units.Commands;

namespace TankGame.Players.Input {

	public class PlayerUIController : MonoBehaviour {

		[SerializeField]
		private InputSystemUIInputModule uiInput;

		[SerializeField]
		private GraphicRaycaster gr;

		private ItemIcon grabbedItem;

		private InventorySlot grabbedFrom;

		private Vector2 mousePos;

		void Start() {
			EventBus.AddListener<PlayerEvent.ControlSwitch>(OnControlSwitch);
		}

		void Update() {
			if (grabbedItem != null) {
				grabbedItem.transform.position = mousePos;
				//Debug.Log(mousePos);
			}
		}

		/*	Event Functions	*/

		private void OnControlSwitch(PlayerEvent.ControlSwitch _event) {
			if (_event.SwitchTo.Equals(PlayerEvent.ControlSwitch.ControlType.Explicit)) uiInput.enabled = false;
			else uiInput.enabled = true;
		}

		/*	Input Functions	*/

		public void LeftClick(InputAction.CallbackContext context) {
			if (context.performed) {
				List<RaycastResult> results = GetClickedObjects();

				foreach (RaycastResult result in results) {
					Debug.Log(result.gameObject.name);
					if (result.gameObject.TryGetComponent<ItemIcon>(out ItemIcon icon)) {
						grabbedItem = icon;
						grabbedFrom = icon.ParentSlot;
						icon.transform.SetParent(gr.transform);
						icon.transform.SetAsLastSibling();
						break;
					}
				}
			}

			if (context.canceled) {
				//Debug.Log("Cancelled Context");

				if (grabbedItem == null) {
					//Debug.Log("Item is null");
					return;
				}

				List<RaycastResult> results = GetClickedObjects();
				//Debug.Log("Releasing Grabbed Item");

				foreach (RaycastResult result in results) {
					if (result.gameObject.TryGetComponent<InventorySlot>(out InventorySlot slot)
						&& !slot.ParentInventory.Equals(grabbedFrom.ParentInventory)
							&& !slot.Occupied) {
						//grabbedFrom.RemoveItem();
						//slot.FillSlot(grabbedItem);

						if (grabbedFrom.ParentInventory.TryGetComponent(out Character character)) {
							character.ExecuteCommand(new TransferItem(slot.ParentInventory, grabbedItem.Item));
						}
					}
				}

				grabbedItem.transform.SetParent(grabbedFrom.transform);
				grabbedItem.transform.localPosition = Vector3.zero;
				grabbedItem = null;
				grabbedFrom = null;
			}
		}

		public void Pointer(InputAction.CallbackContext context) {
			Vector2 input = context.ReadValue<Vector2>();

			mousePos = input;
		}

		private List<RaycastResult> GetClickedObjects() {
			Vector2 mousePos = uiInput.point.action.ReadValue<Vector2>();

			PointerEventData pointData = new PointerEventData(null);
			pointData.position = mousePos;
			List<RaycastResult> results = new List<RaycastResult>();

			gr.Raycast(pointData, results);

			return results;
		}
	}
}