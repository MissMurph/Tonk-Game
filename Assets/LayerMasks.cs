using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//none of this is good code, this is here for my convenience

namespace TankGame {

	public class LayerMasks : MonoBehaviour {

		public LayerMaskEntry[] maskEntries;

		[SerializeField]
		private LayerMask walkableMask;
		public static LayerMask WalkableMask;

		[SerializeField]
		private LayerMask unwalkableMask;
		public static LayerMask UnwalkableMask;

		[SerializeField]
		private LayerMask selectableMask;
		public static LayerMask SelectableMask;

		[SerializeField]
		private LayerMask interactableMask;
		public static LayerMask InteractableMask;

		private void Awake() {
			WalkableMask = walkableMask;
			UnwalkableMask = unwalkableMask;
			SelectableMask = selectableMask;
			InteractableMask = interactableMask;

			/*foreach (LayerMaskEntry entry in maskEntries) {
				LayerMask[] array = entry.childMasks;
				LayerMask mask = entry.mask;

				if (array.Length == 0) continue;

				for (int i = 0; i < array.Length; i++) {
					AddLayerToMask(array[i].value, mask);
				}
			}*/
		}

		public static void AddLayerToMask(int layer, LayerMask mask) {
			mask.value |= layer;
		}

		public static bool IsInLayerMask(int layer, LayerMask mask) {
			//Debug.Log("Layer: " + LayerMask.LayerToName(layer) + "   |   Mask: " + LayerMask.LayerToName(mask));
			return mask == (mask | (1 << layer));
		}
	}

	[Serializable]
	public class LayerMaskEntry {
		public LayerMask mask;
		public LayerMask[] childMasks;
	}
}