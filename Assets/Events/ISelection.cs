using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ISelection : IEventSystemHandler {

	void OnSelect(ISelectable selectable);
}