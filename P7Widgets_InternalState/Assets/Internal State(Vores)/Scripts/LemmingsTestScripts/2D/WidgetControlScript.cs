﻿using UnityEngine;
using System.Collections;

/*
 * This script is placed on the widget gameObjects in the scene. 
 * It contains the controls of the widgets and the tools chosen by the user.
 */

public class WidgetControlScript : MonoBehaviour {

	public int _widgetIndex;									//The index of the widget, to compare with settings in Widget Detection Algorithm gameObject.
	Transform _myTransform;
	Transform _toolMount;
	[SerializeField] private bool _buttonIsHeld = false;		//Flag for whether or not the button on this widget is held in
	private WidgetDetectionAlgorithm _widgetAlgorithm;		//instance of the widget algorithm
	float _toolFadeTime = 0.5f;

	void Start () {
		_widgetAlgorithm = WidgetDetectionAlgorithm.instance;
		_myTransform = transform;
		_toolMount = _myTransform.FindChild("ToolMount");
		HideTool();
	}
	


	void Update () {
		UpdateWidgetInfo();

		if(_buttonIsHeld)
		{
			print("I'm touching the widget button!");
		}
	}



	// Updates the position, orientation and button checks of the widget
	void UpdateWidgetInfo()
	{
		if(_widgetAlgorithm.widgets.Length == 0)
		{
			Invoke("HideTool", _toolFadeTime);
		}

		foreach(Widget w in _widgetAlgorithm.widgets)
		{
			if(w.flags.Contains(_widgetIndex))
			{
				// Position and orientation
				MoveWidget(w);
				//Make visible
				CancelInvoke("HideTool");
				ShowTool();
				// Button check for flag id 3
				_buttonIsHeld = CheckButton(w, 3);
				return;
			} else
			{
				//Make invisible
				Invoke("HideTool", _toolFadeTime);
			}
		}
	}



	void MoveWidget(Widget w)
	{
		//NOTE TO SELF: LERP THIS SOMEHOW
		//Position
		transform.position = Camera.main.ScreenToWorldPoint(w.position) + Vector3.forward;

		//Rotation
		float angle = Vector3.Angle(Vector3.down, w.orientation) * Mathf.Sign(w.orientation.x);
		transform.eulerAngles = Vector3.forward * angle;
	}



	bool CheckButton(Widget w, int flagID)
	{
		//Button
		if(w.flags.Contains(flagID))
		{
			return true;
		} else
		{
			return false;
		}
	}



	void ShowTool()
	{
		foreach(Transform child in _toolMount)
		{

			//Disable/enable renderer
			if(child.GetComponent<SpriteRenderer>())
			{
				child.GetComponent<SpriteRenderer>().enabled = true;
			}

			//Disable/enable tool script
			if(child.GetComponent<BoxCollider2D>())
			{
				child.GetComponent<BoxCollider2D>().enabled = true;
			}
			
		}
	}



	void HideTool()
	{
		//Wait some time before 
		//yield return new WaitForSeconds(_toolFadeTime);
		
		//foreach(Transform child in _toolMount)
		//{
		//	//Disable/enable renderer
		//	if(child.GetComponent<SpriteRenderer>())
		//	{
		//		child.GetComponent<SpriteRenderer>().enabled = false;
		//	}

		//	//Disable/enable tool script
		//	if(child.GetComponent<BoxCollider2D>())
		//	{
		//		child.GetComponent<BoxCollider2D>().enabled = false;
		//	}

		//}
	}



	void OnMouseOver()
	{

	}
}