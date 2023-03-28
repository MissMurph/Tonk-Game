using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.Items {

	[Serializable]
	public class Item {
		[OdinSerialize] public string Name { get; private set; }
		[OdinSerialize] public Vector2Int Size { get; private set; }
		[OdinSerialize] public Sprite Icon { get; private set; }

		protected Item() {}
	}
}