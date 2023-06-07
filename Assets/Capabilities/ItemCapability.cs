using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.Capabilities {

	public class ItemCapability : ICapability {
		public Type Type { get { return typeof(ItemCapability); } }

		public string Name => "Capabilities:ItemObject";

		private ItemObject value;

		public ItemCapability (ItemObject _item) {
			value = _item;
		}

		public ItemObject Get () {
			return value;
		}

		public bool Bool () {
			Debug.LogWarning("Capability type is " + typeof(ItemCapability).ToString() + " not bool!");
			return false;
		}

		public double Double () {
			Debug.LogWarning("Capability type is " + typeof(ItemCapability).ToString() + " not double!");
			return 0;
		}

		public float Float () {
			Debug.LogWarning("Capability type is " + typeof(ItemCapability).ToString() + " not float!");
			return 0;
		}

		//returns the stack count of the itemObject
		public int Int () {
			return value.StackCount;
		}

		//returns the name of the itemObject
		public string String () {
			return value.Item.Name;
		}
	}
}