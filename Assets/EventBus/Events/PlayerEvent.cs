using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Commands;
using UnityEngine;

namespace TankGame.Events {

	public class PlayerEvent : AbstractEvent {

		protected PlayerEvent(string name) : base(name) {

		}

		public class Selection : PlayerEvent {

			public ISelectable Selectable {
				get; private set;
			}

			public bool SelectStatus {
				get; private set;
			}

			public Selection(ISelectable _selectable, bool selectStatus) : base("selection") {
				Selectable = _selectable;
				SelectStatus = selectStatus;
			}
		}

		public class ControlSwitch : PlayerEvent {

			public ControlType SwitchTo { get; private set; }

			public ControlSwitch(ControlType switchTo) : base("control_switch") {
				SwitchTo = switchTo;
			}

			public enum ControlType {
				Implicit,
				Explicit
			}
		}
	}
}