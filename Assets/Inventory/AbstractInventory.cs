using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Items {

    public abstract class AbstractInventory : MonoBehaviour, IInteractable {

        //protected List<ItemObject> storedItems = new List<ItemObject>();

        protected Dictionary<Item, int> stackDictionary = new Dictionary<Item, int>();

        [SerializeField]
        protected StackEntry[] stackEntries;

        protected InteractionManager manager;

        protected virtual void Awake () {
            manager = GetComponent<InteractionManager>();

            foreach (StackEntry entry in stackEntries) {
                stackDictionary.TryAdd(Items.GetItem(entry.itemName), entry.stackLimit);
            }
        }

        /*public List<ItemObject> GetStored () {
            return new List<ItemObject>(storedItems);
        }*/

        public abstract List<ItemObject> GetStored ();


        protected abstract AbstractInteraction TryEnterItem (ItemObject item, Character character, string name);
        protected abstract AbstractInteraction TryTakeItem (ItemObject item, Character character, string name);

        public AbstractInteraction TryTakeItemUI (ItemObject item, Character character) {
            return TryTakeItem(item, character, "TakeItem");
        }

        public AbstractInteraction TryEnterItemUI (ItemObject item, Character character) {
            return TryEnterItem(item, character, "EnterItem");
        }

        public GameObject GetObject () {
            return gameObject;
        }

        public virtual List<AbstractInteractionFactory> GetInteractions () {
            List<AbstractInteractionFactory> output = new List<AbstractInteractionFactory> {
                new InteractionFactory<ItemObject>("EnterItem", TryEnterItem, () => new List<ItemObject>()),
                new InteractionFactory<ItemObject>("TakeItem", TryTakeItem, GetStored)
            };

            return output;
        }

		public InteractionManager GetManager() {
            return manager;
		}

		public class InvInteraction : AbstractInteraction<InvInteraction> {

            internal ItemObject Item { get; private set; }

            internal InvInteraction (ItemObject item, Character character, InteractionFunction destination, IInteractable parent, string name) : base(destination, character, name, parent) {
                Item = item;
            }
        }
    }

    [System.Serializable]
    public class StackEntry {
        public string itemName;
        public int stackLimit;
    }
}