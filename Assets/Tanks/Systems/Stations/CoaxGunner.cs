using TankGame.Players;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;


namespace TankGame.Tanks.Systems.Stations {

	public class CoaxGunner : Station {
		private Seat localSeat;

		public override bool Manned {
			get
			{
				return manningCharacter is not null;
			}
		}

		protected override Character manningCharacter {
			get
			{
				return localSeat.Occupant;
			}
		}

		protected override void Awake () {
			base.Awake();

			localSeat = GetComponent<Seat>();
		}

		protected override void Start () {
			base.Start();

			manager.AddListener<GenericInteraction>("Sit", OnSit);
			manager.AddListener<GenericInteraction>("Unsit", OnUnsit);
		}

		private void OnSit (InteractionContext<GenericInteraction> context) {
			if (ReferenceEquals(context.Interaction.ActingCharacter, Player.PlayerCharacter)) {
				Player.SwitchControl(InputReceiver);
			}
		}

		private void OnUnsit (InteractionContext<GenericInteraction> context) {
			if (ReferenceEquals(context.Interaction.ActingCharacter, Player.PlayerCharacter)) {
				Player.ResetControl();
			}
		}

		public override GenericInteraction TryMan(Character character, string name) {
			return localSeat.TrySit(character, "Sit");
		}
	}
}