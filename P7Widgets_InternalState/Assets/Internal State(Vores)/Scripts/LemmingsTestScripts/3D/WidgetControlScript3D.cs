using UnityEngine;
using System.Collections;

/*
 * This script is placed on the widget gameObjects in the scene. 
 * It contains the controls of the widgets and the tools chosen by the user.
 */

public class WidgetControlScript3D : MonoBehaviour {

	public int _widgetIndex;									//The index of the widget, to compare with settings in Widget Detection Algorithm gameObject.
	Transform _myTransform;
	Transform _toolMount;
	[SerializeField] private bool _buttonIsHeld = false;		//Flag for whether or not the button on this widget is held in
	private WidgetDetectionAlgorithm _widgetAlgorithm;			//instance of the widget algorithm
	float _toolFadeTime = 0.5f;

	float _smoothSpeed = 40;									//The speed at which lerping of rotation and position happens

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
		//Position
		Vector3 pos = Camera.main.ScreenToWorldPoint(w.position) + Vector3.forward;
		pos = new Vector3(pos.x, pos.y, 0);
		transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _smoothSpeed);

		//Rotation
		float angle = Vector3.Angle(Vector3.down, w.orientation) * Mathf.Sign(w.orientation.x);
		//transform.eulerAngles = Vector3.forward * angle;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.forward * angle), Time.deltaTime * _smoothSpeed);
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
			if(child.GetComponent<MeshRenderer>())
			{
				child.GetComponent<MeshRenderer>().enabled = true;
			}

			//Disable/enable tool script
			if(child.GetComponent<BoxCollider>())
			{
				child.GetComponent<BoxCollider>().enabled = true;
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
		//	if(child.GetComponent<MeshRenderer>())
		//	{
		//		child.GetComponent<MeshRenderer>().enabled = false;
		//	}

		//	//Disable/enable tool script
		//	if(child.GetComponent<BoxCollider>())
		//	{
		//		child.GetComponent<BoxCollider>().enabled = false;
		//	}

		//}
	}
}