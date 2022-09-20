using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

	private List<ISelectable> selected = new List<ISelectable>();

	public World world;

	[SerializeField]
	private Camera mainCam;

	[SerializeField]
	private float camMoveSpeed;

	public Vector2 MousePos {
		get {
			return mousePos;
		}
	}

	private Vector2 mousePos;

	public Vector2 Moving {
		get {
			return movingDir;
		}
	}
	
	private Vector2 movingDir;

	[SerializeField]
	private LayerMask selectableLayer;

	/*	UNITY FUNCTIONS	*/
	private void Update () {
		transform.position += new Vector3(movingDir.x * camMoveSpeed, movingDir.y * camMoveSpeed, 0) * Time.deltaTime;
	}


	/*	REGULAR FUNCTIONS	*/
	private void Select (ISelectable selectable) {
		if (selected.Contains(selectable)) selected.Remove(selectable);
		else {
			//Debug.Log(selectable.GetObject().name);
			selected.Add(selectable);
		}
	}


	/*	INPUT FUNCTIONS	*/
	public void Fire (InputAction.CallbackContext context) {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero, 100f, selectableLayer);

		if (hit.collider != null) {
			ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();
			
			Select(selectable);
		}
	}

	public void Look (InputAction.CallbackContext context) {
		Vector2 input = context.ReadValue<Vector2>();

		mousePos = input;
	}

	public void RightClick (InputAction.CallbackContext context) {
		if (context.started) {
			RaycastHit2D hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(mousePos), Vector2.zero, 100f, world.walkableMask);

			Debug.Log("yeet");
			if (hit.collider != null) {
				foreach (ISelectable s in selected) {
					//Debug.Log(hit.point);
					s.EnqueueCommand(Commands.Construct<MoveCommand, Vector2>(Commands.MoveCommand, hit.point));
				}
			}
		}
	}

	public void Move (InputAction.CallbackContext context) {
		Vector2 input = context.ReadValue<Vector2>();

		movingDir = input;
	}
}