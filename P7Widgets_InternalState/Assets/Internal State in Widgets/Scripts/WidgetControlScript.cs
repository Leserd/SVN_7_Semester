using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * This script is placed on the widget gameObjects in the scene. 
 * It contains the controls of the widgets and the tools chosen by the user.
 */

public class WidgetControlScript : NetworkBehaviour {

	public int _widgetIndex;									//The index of the widget, to compare with settings in Widget Detection Algorithm gameObject.
	Transform _myTransform;										//Cache the transform component for better optimisation
	AudioSource _myAudio;										//Cache the audio source component for better optimisation
	Transform _toolMount;										//Child of the player, point at which tools are placed
	[SerializeField] private bool _buttonIsHeld = false;		//Flag for whether or not the button on this widget is held in
	private WidgetDetectionAlgorithm _widgetAlgorithm;			//Instance of the widget algorithm
	float _toolFadeTime = 0.5f;									//The time before tools disappear when widget is not detected 

	float _smoothSpeed = 20;									//The speed at which lerping of rotation and position happens

	public GameObject _tool;							        //The tool assigned to this widget
    public int curToolID;                                       //The id of the assigned tool

	private GameObject _networkObject;							//The object that each client has authority over

	float WIDGET_TOOL_DIST_THRESHOLD;							//Minimum distance between tool and widget before it counts as touching
	float PICK_UP_TIME = 1;										//The time the widget has to be held on the tool to pick it up
	float _pickUpElapsedTime = 0;								//The current amount of time the widget has been on the tool
	bool _isOnTool = false;

    private bool hidden = true;                                 //The visible state of the widget's tool

	public AudioClip pickUpToolSound;			
	public AudioClip showToolSound;
    public AudioClip hideToolSound;

    public Image toolSaveFill;

	private bool _readyToPickup = true;
	private float _resetElapsedTime = 0;
	private const float RESET_PICK_UP_TIME = 1f;

	void Awake()
	{
		WIDGET_TOOL_DIST_THRESHOLD = Screen.height;
	}

	void Start () {
		_widgetAlgorithm = WidgetDetectionAlgorithm.instance;	
		_myTransform = transform;
		_myAudio = GetComponent<AudioSource>();
		_toolMount = _myTransform.FindChild("ToolMount");
		if(_toolMount.childCount == 1)
			_tool = _toolMount.transform.GetChild(0).gameObject;

		if(Application.isEditor)
			ShowTool();												//Hide the tool in the beginning, until widget is placed on the board
		//Debug.LogError("Test");
			
		Invoke("AssignNetworkObject", 1);
	}
	


	void Update ()
    {
        StateMachine();
    }



    void OldProcedure()
    {
        if (isServer)
        {
            //print("Widget is server-controlled");
            if (Input.GetKey(KeyCode.W) && _widgetIndex == 1)
            {
                UpdateWidgetInfoEditor();
            }
            else if (Input.GetKey(KeyCode.E) && _widgetIndex == 2)
            {
                UpdateWidgetInfoEditor();
            }
            else
                UpdateWidgetInfo();

        }
        else
        {
            //print("Widget is NOT server-controlled");

            //Check for readiness to pick up tools
            ToggleReadyToPickUp();

            //Check position of widget on the toolbox

            if (Input.GetKey(KeyCode.W) && _widgetIndex == 1)
            {
                UpdateWidgetInfoEditor();
            }
            else if (Input.GetKey(KeyCode.E) && _widgetIndex == 2)
            {
                UpdateWidgetInfoEditor();
            }
            else
                UpdateWidgetInfoToolbox();

            //Update progress bar of tool saving
            UpdateProgressBar();
        }
    }



	//Not used at the moment
	void StateMachine()
	{
        switch (GameStateManager.State)
        {
            case GameState.GAME:
                if (Input.GetKey(KeyCode.W) && _widgetIndex == 1)
                {
                    UpdateWidgetInfoEditor();
                }
                else if (Input.GetKey(KeyCode.E) && _widgetIndex == 2)
                {
                    UpdateWidgetInfoEditor();
                }
                else
                    UpdateWidgetInfo();

                break;
            case GameState.TOOLBOX:
                //Check for readiness to pick up tools
                ToggleReadyToPickUp();

                //Check position of widget on the toolbox
                if (Input.GetKey(KeyCode.W) && _widgetIndex == 1)
                {
                    UpdateWidgetInfoEditor();
                }
                else if (Input.GetKey(KeyCode.E) && _widgetIndex == 2)
                {
                    UpdateWidgetInfoEditor();
                }
                else
                    UpdateWidgetInfoToolbox();

                //Update progress bar of tool saving
                UpdateProgressBar();

                break;
        }
    }

	void UpdateWidgetInfoToolbox()
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
			else
			{
				ResetTimeToPickup();
			}
		}
		if(_widgetAlgorithm.widgets.Length == 0)
		{
			ResetTimeToPickup();
		}
	}

	void ResetTimeToPickup()
	{
		if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.E))
		{
			//TODO: Revert (via animation) back to normal size/shape
			_isOnTool = false;
			_pickUpElapsedTime = 0;
		}
	}



	void ToggleReadyToPickUp()
	{
		if(_readyToPickup == false)
		{
			_resetElapsedTime += Time.deltaTime;
			if(_resetElapsedTime > RESET_PICK_UP_TIME)
			{
                toolSaveFill.color = Color.white;
                _resetElapsedTime = 0;
                _readyToPickup = true;
			}
		}
	}



	void CheckDistanceToButton()
	{
		if(_readyToPickup)
		{
			//if currentImage is available (not saved to tool already)
            if(_networkObject.GetComponent<ToolAssign>().SavedTools.Contains(GameVariables.ToolboxPanel.CurrentImage.GetComponent<Toolbox_ToolAssign>().toolButtonID) != true)
            {
                //If widget is within distance to pick up a tool
                if (Vector2.Distance(Camera.main.WorldToScreenPoint(transform.position),
                GameVariables.ToolboxPanel.CurrentImage.rectTransform.position) < WIDGET_TOOL_DIST_THRESHOLD)
                {
                    _isOnTool = true;
                    _pickUpElapsedTime += Time.deltaTime;   //time counts up
                   //TODO: Start picking-up effect on image


                    if (_pickUpElapsedTime >= PICK_UP_TIME) //If the widget has been on the tool long enough
                    {
                        if (_networkObject != null)
                        {
                            //DebugConsole.Log("Tool has been picked up");
                            Toolbox_ToolAssign ta = GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>().CurrentImage.GetComponent<Toolbox_ToolAssign>();

                            if (ta == null)
                                Debug.LogError("Couldn't find toolbox_toolassign");

                            PlaySound(pickUpToolSound);

                            if(_tool != null)
                            {
                                print("tool not null");
                                _networkObject.GetComponent<ToolAssign>().PickUpTool(_widgetIndex, _tool.GetComponent<ToolID>().ID, ta.toolToAssign);
                            }
                            else
                            {
                                print("tool null");
                                _networkObject.GetComponent<ToolAssign>().PickUpTool(_widgetIndex, 0, ta.toolToAssign);
                            }
                            //_networkObject.GetComponent<ToolAssign>().CmdAssign(_widgetIndex, ta.toolToAssign);
                            _pickUpElapsedTime = 0;

                            toolSaveFill.color = Color.green;

                            _readyToPickup = false;

                        }
                        else
                        {
                            //print("No network object was found!");
                            //DebugConsole.Log("No network object was found!");
                        }
                    }
                }

                //If widget is NOT within distance to pick up a tool
                else
                {
                    //UpdateProgressBar();
                    ResetTimeToPickup();
                }
            }
			
		}
	}



	void UpdateProgressBar()
	{
		if(_readyToPickup)
            toolSaveFill.fillAmount = Mathf.Clamp01(_pickUpElapsedTime);
	}


	//Save the player's own network object to _networkObject variable
	void AssignNetworkObject()
	{
		ToolAssign[] netObjects = GameObject.FindObjectsOfType<ToolAssign>();

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
			if(w.flags.Contains(_widgetIndex))					//If this widget IS detected...
			{
				// Update position and orientation
				MoveWidget(w);

				//Make visible
				if(Application.isEditor == false)
				{
					CancelInvoke("HideTool");					//Cancel any existing invokes for hiding this tool
				}
					
				ShowTool();

				// Button check for flag id 3
				_buttonIsHeld = CheckButton(w, 3);
				return;
			} else
			{			
				//Make invisible
				Invoke("HideTool", _toolFadeTime);				//Hide tool if no widgets are detected
			}
		}
	}



		// Updates the position, orientation and button checks of the widget
	void UpdateWidgetInfoEditor()
	{
		//Position
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward;
		pos = new Vector3(pos.x, pos.y, 0);

        if (!hidden)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _smoothSpeed);
        }
        else
            transform.position = pos;

        if (GameStateManager.State == GameState.GAME)
		    ShowTool();

		if(GameStateManager.State == GameState.TOOLBOX)
		{
			CheckDistanceToButton();
		}
	}



	void MoveWidget(Widget w)
	{
		//Position
		Vector3 pos = Camera.main.ScreenToWorldPoint(w.position) + Vector3.forward;
		pos = new Vector3(pos.x, pos.y, 0);

        if (!hidden)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _smoothSpeed);
        }
        else
            transform.position = pos;

        //Rotation
        float angle = Vector3.Angle(Vector3.down, w.orientation) * Mathf.Sign(w.orientation.x);
		//transform.eulerAngles = Vector3.forward * angle;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.forward * angle), Time.deltaTime * _smoothSpeed);

		//reset the elapsed time for resetting readyToPickUp
		if(_readyToPickup == false)
			_resetElapsedTime = 0;
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
        if (hidden != false)
            hidden = false;

        foreach (Transform child in _toolMount)
		{

			//Enable renderer
			if(child.GetComponent<MeshRenderer>())
			{
                if (child.GetComponent<MeshRenderer>().enabled == false){

                    PlaySound(showToolSound);
                }

                child.GetComponent<MeshRenderer>().enabled = true;
			}

			//Enable tool script
			if(child.GetComponent<BoxCollider>())
			{
				child.GetComponent<BoxCollider>().enabled = true;
			}


            //enable tool script
            if (child.GetComponent<MeshCollider>())
            {
                child.GetComponent<MeshCollider>().enabled = true;
            }


        }
	}



	void HideTool()
	{
        if (hidden != true)
            hidden = true;

        if (Application.isEditor == false)
		{
			foreach(Transform child in _toolMount)
			{
				//Disable/enable renderer
				if(child.GetComponent<MeshRenderer>())
				{
                    if (child.GetComponent<MeshRenderer>().enabled == true)
                    {
                        PlaySound(hideToolSound);
                    }
                    
                    child.GetComponent<MeshRenderer>().enabled = false;
				}

				//Disable/enable tool script
				if(child.GetComponent<BoxCollider>())
				{
					child.GetComponent<BoxCollider>().enabled = false;
				}

                //Disable/enable tool script
                if (child.GetComponent<MeshCollider>())
                {
                    child.GetComponent<MeshCollider>().enabled = false;
                }

            }
           
		}
	}



	
	


	public void RemoveTool()
	{
		if(_tool != null)
		{
			//if(!Application.isEditor && !isServer)
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



	public void PlaySound(AudioClip clip)
	{
		if(clip != null && _myAudio.isPlaying != true)
		{
			_myAudio.clip = clip;
			_myAudio.Play();
		}
			
	}
}