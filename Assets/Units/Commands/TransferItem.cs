using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.Units.Commands {

	public class TransferItem : Command<IInventory> {

		public ItemObject Item { get; private set; }

		private IInventory transferFrom;

		public TransferItem(IInventory target, ItemObject item) : base(target, "transfer_item") {
			Item = item;
		}

		public override void Start(Character character, Action<CommandContext> callback) {
			base.Start(character, callback);

			TargetTransform = Target().GetObject().transform;
			transferFrom = character.GetComponent<IInventory>();

			character.SubmitTarget(TargetTransform, OnPathComplete);
		}

		public override void OnTriggerEnter(Collider2D collision) {
			base.OnTriggerEnter(collision);

			Perform();

			if (transferFrom.TransferItem(Target(), Item)) {
				Complete();
			}
			else Cancel();
		}

		public override void Cancel() {
			base.Cancel();

			Character.Stop();
		}

		private void OnPathComplete(bool success) {
			if (!success) Cancel();
		}
	}
}