using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

/*
 * This script is placed on the widget gameObjects in the scene. 
 * It contains the controls of the widgets and the tools chosen by the user.
 */

public class WidgetControlScript : NetworkBehaviour {

	public int _widgetIndex;									//The index of the widget, to compare with settings in Widget Detection Algorithm gameObject.
	Transform _myTransform;										//Cache the transform component for better optimisation
	Transform _toolMount;										//Child of the player, point at which tools are placed
	[SerializeField] private bool _buttonIsHeld = false;		//Flag for whether or not the button on this widget is held in
	private WidgetDetectionAlgorithm _widgetAlgorithm;			//Instance of the widget algorithm
	float _toolFadeTime = 0.5f;									//The time before tools disappear when widget is not detected 

	float _smoothSpeed = 20;									//The speed at which lerping of rotation and position happens

	[SyncVar] private GameObject _tool;							//The tool assigned to this widget
	[SyncVar]
	public int testInt = 0;

	private GameObject _networkObject;							//The object that each client has authority over

	const float WIDGET_TOOL_DIST_THRESHOLD = 150;

	void Start () {
		_widgetAlgorithm = WidgetDetectionAlgorithm.instance;	
		_myTransform = transform;
		_toolMount = _myTransform.FindChild("ToolMount");
		if(_toolMount.childCount == 1)
			_tool = _toolMount.transform.GetChild(0).gameObject;
		HideTool();												//Hide the tool in the beginning, until widget is placed on the board

		Invoke("AssignNetworkObject", 1);
	}
	


	void Update () {
		if(isServer)
		{
			//print("Widget is server-controlled");
			UpdateWidgetInfo();

			if(_buttonIsHeld)
			{
				print("I'm touching the widget button!");
			}
		}
		else 
		{
			//print("Widget is NOT server-controlled");
			ToolboxCheckPosition();
		}	
	}


	void ToolboxCheckPosition()
	{
		//print(gameObject.name + ": " + Vector2.Distance(Camera.main.WorldToScreenPoint(transform.position), 
			//GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>().CurrentImage.rectTransform.position));
		
		foreach(Widget w in _widgetAlgorithm.widgets)
		{
			if(w.flags.Contains(_widgetIndex))				//If this widget IS detected...
			{
				// Update position and orientation
				MoveWidget(w);

				CheckDistanceToButton();

				// Button check for flag id 3
				_buttonIsHeld = CheckButton(w, 3);
				return;
			}
		}
	}


	void CheckDistanceToButton()
	{
		if(Vector2.Distance(Camera.main.WorldToScreenPoint(transform.position),
		GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>().CurrentImage.rectTransform.position) < WIDGET_TOOL_DIST_THRESHOLD)
		{
			if(_networkObject != null)
			{
				//GameObject.Find("GameManager").GetComponent<ToolAssign>().CmdAssign();

				Toolbox_ToolAssign ta = GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>().CurrentImage.GetComponent<Toolbox_ToolAssign>();

				if(ta == null)
					Debug.LogError("Couldn't find toolbox_toolassign");

				_networkObject.GetComponent<ToolAssign>().CmdAssign(_widgetIndex, ta.toolToAssign);
			}
			else
			{
				print("No network object was found!");
			}
		}
	}


	//Save the player's own network object to _networkObject variable
	void AssignNetworkObject()
	{
		sendID[] netObjects = GameObject.FindObjectsOfType<sendID>();

		for(int i = 0; i < netObjects.Length; i++)
		{
			if(netObjects[i].isLocalPlayer)
			{
				_networkObject = netObjects[i].gameObject;
			}
		}
	}


	// Updates the position, orientation and button checks of the widget
	void UpdateWidgetInfo()
	{
		if(_widgetAlgorithm.widgets.Length == 0)
		{
			Invoke("HideTool", _toolFadeTime);				//Hide tool if no widgets are detected
		}

		foreach(Widget w in _widgetAlgorithm.widgets)
		{
			if(w.flags.Contains(_widgetIndex))				//If this widget IS detected...
			{
				// Update position and orientation
				MoveWidget(w);

				//Make visible
				CancelInvoke("HideTool");					//Cancel any existing invokes for hiding this tool
				ShowTool();

				// Button check for flag id 3
				_buttonIsHeld = CheckButton(w, 3);
				return;
			} else
			{			
				//Make invisible
				Invoke("HideTool", _toolFadeTime);			//Hide tool if this widget is not detected.
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

			//Enable renderer
			if(child.GetComponent<MeshRenderer>())
			{
				child.GetComponent<MeshRenderer>().enabled = true;
			}

			//Enable tool script
			if(child.GetComponent<BoxCollider>())
			{
				child.GetComponent<BoxCollider>().enabled = true;
			}

		}
	}



	void HideTool()
	{
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



	public void RemoveTool()
	{
		if(_tool != null)
		{
			GameObject temp = _tool.gameObject;
			Destroy(_tool.gameObject);
		}
	}



	public GameObject Tool
	{
		get { return _tool; }
		set { _tool = value; }
	}



	public Transform ToolMount
	{
		get { return _toolMount; }
		set { _toolMount = value; }
	}
}