using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using TankGame.Tanks;
using UnityEngine;

namespace TankGame.UI {

	public class StoragePanel : MonoBehaviour {

		[SerializeField]
		private TankInventory parentInventory;

		private StorageSpace[,] storageSpaces;

		[SerializeField]
		private GameObject storageSpacePrefab;

		private RectTransform rTransform;

		[SerializeField]
		private const int cellSize = 32;

		private void Awake() {
			storageSpaces = new StorageSpace[parentInventory.Size.x, parentInventory.Size.y];
			rTransform = GetComponent<RectTransform>();

			rTransform.anchoredPosition = new Vector2(parentInventory.Size.x * cellSize / 2, 0);
		}

		void Start() {
			for (int x = 0; x < parentInventory.Size.x; x++) {
				for (int y = 0; y < parentInventory.Size.y; y++) {
					GameObject obj = Instantiate(storageSpacePrefab, transform);
					StorageSpace space = obj.GetComponent<StorageSpace>();

					//obj.transform.localPosition = 

					RectTransform rect = obj.GetComponent<RectTransform>();
					rect.anchoredPosition = new Vector3((x * cellSize) + cellSize/2, (y * cellSize) + cellSize/2, 0);

					storageSpaces[x, y] = space;

					
				}
			}
		}

		public void DrawerOpen () {
			rTransform.anchoredPosition = new Vector2(-parentInventory.Size.x * cellSize / 2, 0);
		}

		public void DrawerClose () {
			rTransform.anchoredPosition = new Vector2(parentInventory.Size.x * cellSize / 2, 0);

		}
	}
}