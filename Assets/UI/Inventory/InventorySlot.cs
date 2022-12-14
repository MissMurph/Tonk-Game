using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using TankGame.UI;
using UnityEngine;

namespace TankGame.UI {

	public class InventorySlot : MonoBehaviour {

		public InventoryElement ParentInventory { get; private set; }

		public ItemIcon OccupyingItem;

		public bool Occupied {
			get
			{
				return OccupyingItem != null;
			}
		}

		private void Awake () {
			ParentInventory = GetComponentInParent<InventoryElement>();
		}
	}
}