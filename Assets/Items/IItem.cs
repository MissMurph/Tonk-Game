using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem {
	string Name();
	Vector2 Size();
	int StackLimit();
}