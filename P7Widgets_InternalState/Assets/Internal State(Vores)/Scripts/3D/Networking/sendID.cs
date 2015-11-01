using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class sendID : NetworkBehaviour {

    [SyncVar] public int widgetID;
    int widget1 = 511;
    int widget2 = 522;

    [SyncVar] public GameObject netWidget;
	public List<WidgetControlScript> _players;
	Toolbox_Move _toolbox;
	private WidgetDetectionAlgorithm _widgetAlgorithm;
	public GameObject[] tools;

	// Use this for initialization
	void Start () {

        DontDestroyOnLoad(netWidget);
		_widgetAlgorithm = WidgetDetectionAlgorithm.instance;	

		_toolbox = GameObject.FindObjectOfType<Toolbox_Move>();
		_players = new List<WidgetControlScript>();

		_players.Add(GameObject.Find("Player 1").GetComponent<WidgetControlScript>());
		_players.Add(GameObject.Find("Player 2").GetComponent<WidgetControlScript>());

		
		//print(toolButtons.Length);

		if(!isServer && isLocalPlayer)
		{
			//Assign the correct function to call with tool buttons 
			Toolbox_ToolAssign[] toolButtons = GameObject.FindObjectsOfType<Toolbox_ToolAssign>();

			for(int i = 0; i < toolButtons.Length; i++)
			{
				Toolbox_ToolAssign button = toolButtons[i];
				int toolToAssign = button.toolToAssign;
				int widgetToAssign = button.widgetToAssign;
				//print(toolButtons[i].name + ", " + toolButtons[i].GetComponent<Button>());
				

				//List<int> ids = new List<int>();
				//ids.Add(widgetToAssign);
				//ids.Add(toolToAssign);

				int[] ids = new int[2];
				ids[0] = widgetToAssign;
				ids[1] = toolToAssign;
				print("wid: " + ids[0] + ", tool: " + ids[1]);
				button.GetComponent<Button>().onClick.AddListener(() => CmdCallToolChange(1,1));
			}
		}
	}


	// Update is called once per frame
	void Update()
	{

		if(isClient && !isServer)
		{
			if(Input.GetKeyDown(KeyCode.A))
			{

				CmdCallToolChange(1, 2);

			}
			if(Input.GetKeyDown(KeyCode.B))
			{
				CmdCallToolChange(2);
			}
		}
	}



	[Command]
	void CmdwidgetSwitch1()
	{

		GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, 1);
		Debug.Log("wSwitch1 Run");


	}
	[Command]
	void CmdwidgetSwitch2()
	{


		GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, 2);
		Debug.Log("wSwitch2 Run");

	}

	

	[Command]
	public void CmdCallToolChange()
	{
		Toolbox_ToolAssign ta = _toolbox.CurrentImage.GetComponent<Toolbox_ToolAssign>();
		//print(_toolbox.CurrentImage.name);
		if(ta == null)
			Debug.LogError("Couldn't find toolbox_toolassign");

		print("CmdCallToolChange0 called: " + ta.widgetToAssign + ", " + ta.toolToAssign);
		GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(ta.widgetToAssign, ta.toolToAssign);
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, 2);
	}

	[Command]
	public void CmdCallToolChange(int toolIndex)
	{
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, toolIndex);
		Assign(1, toolIndex);
		//Debug.Log("Call Tool Change: " + toolIndex);
	}

	[Command]
	public void CmdCallToolChange(int widgetID, int toolIndex)
	{
		int int1 = widgetID, int2 = toolIndex;
		//print("CmdCallToolChange2 called: " + widgetID + ", " + toolID);
		GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(int1, int2);
		//Assign(widgetID, toolIndex);
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, 2);
	}

	[Command]
	public void CmdCallToolChange(List<int> IDs)		//IDs[0] = widget, [1] = tool
	{
		//print("CmdCallToolChange2 called: " + widgetID + ", " + toolID);
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(IDs[0], IDs[1]);
		Assign(IDs[0], IDs[1]);
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, 2);
	}

	[Command]
	public void CmdCallToolChange(int[] IDs)		//IDs[0] = widget, [1] = tool
	{
		//print("CmdCallToolChange2 called: " + widgetID + ", " + toolID);
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(IDs[0], IDs[1]);
		Assign(IDs[0], IDs[1]);
		//GameObject.Find("GameManager").GetComponent<ToolAssign>().Assign(1, 2);
	}


	public void Assign(int widgetID, int toolID)
	{
		print("Assign called: " + widgetID + ", " + toolID);
		//Save widget as temporary variable
		WidgetControlScript widget = _players[widgetID - 1];
		print("Widget number: " + widgetID + " exists: " + _players[widgetID - 1].name);
		widget.testInt = toolID;
		//Find widget tool mount
		Transform mount = widget.ToolMount;

		//Instantiate tool in tool mount pos
		GameObject tool = Instantiate(tools[toolID - 1], mount.position, Quaternion.identity) as GameObject;

		//Assign tool mount as tool parent
		tool.transform.parent = mount;

		//Remove current tool from widget
		widget.RemoveTool();

		//Assign tool to widget
		widget.Tool = tool;

		NetworkServer.Spawn(tool);

		print("IsServer: " + widget.isServer + ", widgetID: " + widgetID + ", toolID: " + toolID);
	}

}
