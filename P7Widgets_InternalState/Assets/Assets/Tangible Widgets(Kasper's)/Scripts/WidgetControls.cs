using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WidgetControls : MonoBehaviour {

	public int widgetIndex;

	[SerializeField] private bool buttonIsHeld = false;
	private WidgetDetectionAlgorithm widgetAlgorithm;

	void Start () {
		widgetAlgorithm = WidgetDetectionAlgorithm.instance;
	}
	
	void Update () {
		UpdateWidgetInfo ();

		if (buttonIsHeld) {
			print ("I'm touching the widget button!");
		}
	}

	// Updates the position, orientation and button checks of the widget
	void UpdateWidgetInfo() {
		foreach (Widget w in widgetAlgorithm.widgets) {
			if(w.flags.Contains(widgetIndex)) {
				// Position and orientation
				MoveWidget(w);
				// Button check for flag id 3
				buttonIsHeld = CheckButton(w, 3);
				return;
			}
		}
	}

	void MoveWidget(Widget w) {
		//Position
		transform.position = Camera.main.ScreenToWorldPoint (w.position) + Vector3.forward;

		//Rotation
		float angle = Vector3.Angle (Vector3.down, w.orientation) * Mathf.Sign (w.orientation.x);
		transform.eulerAngles = Vector3.forward * angle;
	}

	bool CheckButton(Widget w, int flagID) {
		//Button
		if (w.flags.Contains (flagID)) {
			return true;
		} else {
			return false;
		}
	}
}
