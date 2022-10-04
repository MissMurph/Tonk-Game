using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IControllable {

	private List<ISelectable> selected = new List<ISelectable>();

	[SerializeField]
	private Player player;

	public Vector2 MousePos {
		get
		{
			return mousePos;
		}
	}

	[SerializeField]
	private Vector2 mousePos;

	public Vector2 Moving {
		get
		{
			return movingDir;
		}
	}

	[SerializeField]
	private Vector2 movingDir;

	[SerializeField]
	private Camera mainCam;

	[SerializeField]
	private float camMoveSpeed;

	private PlayerInput input;

	/*	UNITY FUNCTIONS	*/

	private void Awake () {
		input = GetComponent<PlayerInput>();
	}

	private void Update () {
		transform.position += new Vector3(movingDir.x * camMoveSpeed, movingDir.y * camMoveSpeed, 0) * Time.deltaTime;
	}

	/*	REGULAR FUNCTIONS	*/
	public bool Select (ISelectable selectable) {
		if (selected.Contains(selectable)) {
			selected.Remove(selectable);
			return false;
		}
		else {
			selected.Add(selectable);
			return true;
		}
	}

	/*	INPUT FUNCTIONS	*/
	public void LeftClick (InputAction.CallbackContext context) {
		//Debug.Log("Left Click");
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero, 100f, LayerMasks.SelectableMask);

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
		//Debug.Log("Right Click");

		if (context.started) {

			RaycastHit2D interactableRay = Physics2D.Raycast(mainCam.ScreenToWorldPoint(mousePos), Vector3.forward, 100f, LayerMasks.InteractableMask);
			RaycastHit2D walkableRay = Physics2D.Raycast(mainCam.ScreenToWorldPoint(mousePos), Vector3.forward, 100f, LayerMasks.WalkableMask);

			if (interactableRay.collider != null) {
				foreach (ISelectable s in selected) {
					s.ExecuteCommand(Commands.Construct<InteractCommand, IInteractable>(Commands.InteractCommand, interactableRay.collider.gameObject.GetComponentInParent<IInteractable>()));
				}
			}
			else if (walkableRay.collider != null) {
				foreach (ISelectable s in selected) {
					s.ExecuteCommand(Commands.Construct<MoveCommand, Vector2>(Commands.MoveCommand, walkableRay.point));
				}
			}
		}
	}

	public void Move (InputAction.CallbackContext context) {
		Vector2 input = context.ReadValue<Vector2>();

		movingDir = input;
	}

	public void Input (InputAction.CallbackContext context) {
		return;
	}

	public GameObject GetObject () {
		return this.gameObject;
	}

	public bool Occupied() {
		return false;
	}

	public List<ISelectable> GetSelected () {
		return selected;
	}
}