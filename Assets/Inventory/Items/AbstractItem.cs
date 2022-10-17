using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.Items {

	public abstract class AbstractItem {
		public string Name { get; private set; }
		public Vector2Int Size { get; private set; }
		public Sprite Icon { get; private set; }

		protected AbstractItem(string _name, Vector2Int _size, Sprite _icon) {
			Name = _name;
			Size = _size;
			Icon = _icon;
		}
	}
}