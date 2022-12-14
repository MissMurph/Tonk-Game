using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Events {

	public class InitializationEvent : AbstractEvent {
		public InitPhase Phase { get; private set; }

		protected InitializationEvent (string _name, InitPhase _phase) : base(_name) {
			Phase = _phase;
		}

		public class InitUI : InitializationEvent {

			public InitUI (InitPhase _phase) : base("InitUI", _phase) {

			}
		}

		public enum InitPhase {
			Pre,
			Init,
			Post
		}
	}
}
