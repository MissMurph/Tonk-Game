using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Entities {

	public interface ITaggable {
		void Register (Entity entity);
		abstract Type Type { get; }
		
	}

	public interface ITaggable<T> where T : ITaggable {
		T Get ();
		Type Type { get { return typeof(T); } }
	}
}