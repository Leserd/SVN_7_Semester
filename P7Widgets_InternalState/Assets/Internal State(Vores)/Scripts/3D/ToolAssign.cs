using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ToolAssign : NetworkBehaviour {

	public GameObject[] tools;
	public Toolbox_ToolAssign ta;


	void Start()
	{

		DontDestroyOnLoad(gameObject);

		if(!isServer && isLocalPlayer)
		{
			AssignButtonListeners();
		}
	}



	void AssignButtonListeners()
	{
		
		//Assign the correct function to call with tool buttons 
		Toolbox_ToolAssign[] toolButtons = GameObject.FindObjectsOfType<Toolbox_ToolAssign>();

		for(int i = 0; i < toolButtons.Length; i++)
		{
			Toolbox_ToolAssign button = toolButtons[i];
			int toolToAssign = button.toolToAssign;
			int widgetToAssign = button.widgetToAssign;


			//List<int> ids = new List<int>();
			//ids.Add(widgetToAssign);
			//ids.Add(toolToAssign);

			int[] ids = new int[2];
			ids[0] = widgetToAssign;
			ids[1] = toolToAssign;
			print("wid: " + ids[0] + ", tool: " + ids[1]);
			button.GetComponent<Button>().onClick.AddListener(() => CmdAssign(1, toolToAssign));
		}
		
	}



	public void Assign(int widgetID, int toolID)
	{
		print("Assign called: "+ widgetID + ", " + toolID);

		//Save widget as temporary variable
		WidgetControlScript widget = GameVariables.Players[widgetID - 1];

		print("Widget number: " + widgetID + " exists: " + widget.name);

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

	[Command]
	public void CmdAssign(int widgetID, int toolID)
	{
		print("CmdAssign called: " + widgetID + ", " + toolID);

		//Save widget as temporary variable
		WidgetControlScript widget = GameVariables.Players[widgetID - 1];

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
	}  
}  