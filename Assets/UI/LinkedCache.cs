using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;

public class LinkedCache : MonoBehaviour {

	private static Dictionary<Type, List<LinkedElement>> linkedMap = new Dictionary<Type, List<LinkedElement>>();

	private void Start () {
		EventBus.Subscribe<InitializationEvent.InitUI>(InitializeUI);
	}

	private void InitializeUI (InitializationEvent.InitUI evnt) {
		LinkedElement[] foundElements = GetComponentsInChildren<LinkedElement>();

		for (int i = 0; i < foundElements.Length; i++) {

			if (linkedMap.TryGetValue(foundElements[i].LinkedType, out List<LinkedElement> list)) {
				list.Add(foundElements[i]);
			}
			else {
				List<LinkedElement> elementList = new List<LinkedElement>();
				linkedMap.Add(foundElements[i].LinkedType, elementList);

				elementList.Add(foundElements[i]);
			}
		}
	}

	public static LinkedElement<T> FindLinkedElement<T> (T requestedObject) where T : MonoBehaviour {
		if (linkedMap.TryGetValue(typeof(T), out List<LinkedElement> list)) {
			foreach (LinkedElement element in list) {
				LinkedElement<T> linkedElement = element.GetAsType<T>();

				if (linkedElement.GetLinked().Equals(requestedObject)) {
					return linkedElement;
				}
			}
		}

		Debug.LogError("No Linked Element Found! Are you sure the Linked Type & Name Matches?");
		return null;
	}

	public static void OnElementDestroy (LinkedElement element) {
		if (linkedMap.TryGetValue(element.LinkedType, out List<LinkedElement> list)) {
			foreach (LinkedElement elmnt in list) {
				if (ReferenceEquals(element, elmnt)) {
					list.Remove(elmnt);
					return;
				}
			}
		}
	}

	public static void OnElementInstantiate (LinkedElement element) {
		if (linkedMap.TryGetValue(element.LinkedType, out List<LinkedElement> list)) {
			list.Add(element);
		}
	}
}