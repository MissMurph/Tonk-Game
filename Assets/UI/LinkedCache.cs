using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TankGame.Events;
using UnityEngine;

public class LinkedCache : MonoBehaviour {

	private Dictionary<Type, List<LinkedElement>> linkedMap;

	//private Queue<LinkedElement> linkRequestQueue;

	private static LinkedCache instance;

	private void Awake () {
		instance = this;
		linkedMap = new Dictionary<Type, List<LinkedElement>>();
		//linkRequestQueue = new Queue<LinkedElement>();
	}

	private void Start () {
		//EventBus.Subscribe<InitializationEvent.InitUI>(InitializeUI);
	}

	/*private void Update () {
		if (linkedMap is not null && linkRequestQueue.TryDequeue(out LinkedElement uiElement)) {
			List<LinkedElement> elements = GetTypeCache(uiElement.LinkedType);
			elements.Add(uiElement);
		}
	}*/

	private void InitializeUI (InitializationEvent.InitUI evnt) {
		LinkedElement[] foundElements = GetComponentsInChildren<LinkedElement>();

		for (int i = 0; i < foundElements.Length; i++) {
			List<LinkedElement> list = instance.GetTypeCache(foundElements[i].LinkedType);

			list.Add(foundElements[i]);
		}
	}

	public static bool Contains<T> (T worldObject) where T : MonoBehaviour {
		List<LinkedElement> list = instance.GetTypeCache(typeof(T));

		foreach (LinkedElement element in list) {
			LinkedElement<T> linkedElement = element as LinkedElement<T>;

			if (ReferenceEquals(linkedElement.GetLinked(), worldObject)) {
				return true;
			}
		}

		return false;
	}

	public static LinkedElement<T> FindLinkedElement<T> (T requestedObject) where T : MonoBehaviour {
		List<LinkedElement> list = instance.GetTypeCache(typeof(T));

		foreach (LinkedElement element in list) {
			LinkedElement<T> linkedElement = element as LinkedElement<T>;

			if (ReferenceEquals(linkedElement.GetLinked(), requestedObject)) {
				return linkedElement;
			}
		}

		Debug.LogWarning("No Linked Element Found! Are you sure the Linked Type & Name Matches?");
		return null;
	}

	private List<LinkedElement> GetTypeCache (Type type) {
		if (linkedMap.TryGetValue(type, out List<LinkedElement> list)) {
			return list;
		}
		else {
			list = new List<LinkedElement>();
			linkedMap.Add(type, list);
			return list;
		}
	}

	public static void OnElementDestroy (LinkedElement element) {
		List<LinkedElement> list = instance.GetTypeCache(element.LinkedType);

		foreach (LinkedElement elmnt in list) {
			if (ReferenceEquals(element, elmnt)) {
				list.Remove(elmnt);
				return;
			}
		}

		Debug.LogWarning("Error caching linked element, element already found");
	}

	public static void OnElementInstantiate (LinkedElement element) {
		List<LinkedElement> list = instance.GetTypeCache(element.LinkedType);

		if (!list.Contains(element)) {
			list.Add(element);
			return;
		}

		Debug.LogError("Error caching linked element, element already found");
	}
}