using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**	This is a standard pattern for all Prefab Storage. Prefab storage cannot be modified, accessing prefabs is readonly
 *	Actual storage must be maintained in main class so that we can change in inspector. Accessible values will be stored
 *	in sub-classes so programatically organize when accessing. Sub-classes access instance and remain static
 *	MUST NOT BE ACCESSED IN AWAKE
 */
public class UIPrefabs : MonoBehaviour {

	private static UIPrefabs instance;

	[SerializeField]
	private GameObject inventorySlot;
	[SerializeField]
	private GameObject itemIcon;
	[SerializeField]
	private GameObject inventorySpace;
    [SerializeField]
    private GameObject cacheElement;
	[SerializeField]
	private GameObject progressBar;

	public static GameObject ProgressBar { get { return instance.progressBar; } }

	private void Awake () {
		instance = this;
	}

	public class CharacterPrefabs {
		public static GameObject InventorySlot { get { return instance.inventorySlot; } }
		public static GameObject ItemIcon { get { return instance.itemIcon; } }
		public static GameObject InventorySpace { get { return instance.inventorySpace; } }
	}

	public class WorldPrefabs {
		public static GameObject CacheElement { get { return instance.cacheElement; } }
	}
}