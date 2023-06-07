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

        protected Source manager;

        protected virtual void Awake () {
            manager = GetComponent<Source>();

            foreach (StackEntry entry in stackEntries) {
                stackDictionary.TryAdd(Items.GetItem(entry.itemName), entry.stackLimit);
            }
        }

        /*public List<ItemObject> GetStored () {
            return new List<ItemObject>(storedItems);
        }*/

        public abstract List<ItemObject> GetStored ();


        protected abstract IResult RequestAdd (Actor actor, Interactionlet packet);
        protected abstract IResult RequestRemove (Actor actor, Interactionlet packet);
        protected abstract IResult ActAdd (Actor actor, Interactionlet packet);
        protected abstract IResult ActRemove (Actor actor, Interactionlet packet);
        protected abstract void ListenAdd (Interactionlet packet);
        protected abstract void ListenRemove (Interactionlet packet);

        /*public Interaction TryTakeItemUI (ItemObject item, Character character) {
            return RequestRemove(item, character, "TakeItem");
        }

        public Interaction TryEnterItemUI (ItemObject item, Character character) {
            return RequestAdd(item, character, "EnterItem");
        }*/

        public GameObject GetObject () {
            return gameObject;
        }

		public Source GetManager() {
            return manager;
		}

		public void OnCollection (Collector collector) {
            collector.Submit("add", ActAdd, RequestAdd, this);
            collector.Submit("remove", ActRemove, RequestRemove, this);
            collector.AttachListener("add", ListenAdd);
            collector.AttachListener("remove", ListenRemove);
        }
    }

    [System.Serializable]
    public class StackEntry {
        public string itemName;
        public int stackLimit;
    }
}