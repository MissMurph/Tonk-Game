using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Items {

	public class GenericItem : AbstractItem {

		public GenericItem(string name, Vector2Int size, Sprite icon) : base("Generic" + name, size, icon) {

		}
	}
}