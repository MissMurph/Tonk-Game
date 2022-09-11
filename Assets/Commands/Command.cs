using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Command {

	[SerializeField]
	private string name;

	//[SerializeField]
	//private Action handler;

	//private delegate void Action();

	[SerializeField]
	private UnityEvent action;
}