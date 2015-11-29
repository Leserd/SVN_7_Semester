using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class ToolAssign : NetworkBehaviour {

	public GameObject[] tools;
    private List<Toolbox_ToolAssign> toolboxList;
    private List<int> savedTools;

	void Start()
	{

		DontDestroyOnLoad(gameObject);

        savedTools = new List<int>();

        CreateToolboxList();

		if(!isServer && isLocalPlayer)
		{
			//AssignButtonListeners();
		}
	}




	public void Update()
	{
		if(isServer)
		{
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				CmdAssign(1, 1);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				CmdAssign(1, 2);
			}
			if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				CmdAssign(1, 3);
			}
			if(Input.GetKeyDown(KeyCode.Alpha4))
			{
				CmdAssign(2, 1);
			}
			if(Input.GetKeyDown(KeyCode.Alpha5))
			{
				CmdAssign(2, 2);
			}
			if(Input.GetKeyDown(KeyCode.Alpha6))
			{
				CmdAssign(2, 3);
			}
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

			button.GetComponent<Button>().onClick.AddListener(() => CmdAssign(1, toolToAssign));
		}
		
	}



    void CreateToolboxList()
    {
        toolboxList = new List<Toolbox_ToolAssign>();

        Toolbox_ToolAssign[] tas = FindObjectsOfType<Toolbox_ToolAssign>();
        foreach(Toolbox_ToolAssign ta in tas)
        {
            toolboxList.Add(ta);
        }

        toolboxList = toolboxList.OrderBy(x => x.GetComponent<Toolbox_ToolAssign>().toolButtonID).ToList();
    }



	public void Assign(int widgetID, int toolID)
	{
        //print("CmdAssign called: " + widgetID + ", " + toolID);

        //Save widget as temporary variable
        WidgetControlScript widget = GameVariables.Players[widgetID - 1];

        //Find widget tool mount
        Transform mount = widget.ToolMount;

        //Remove current tool from widget
        widget.RemoveTool();

        //For safety measures, destroy all children of widget
        Transform[] children = mount.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != mount.transform)
                Destroy(child.gameObject);
        }

        //Instantiate tool in tool mount pos
        GameObject tool = Instantiate(tools[toolID - 1], mount.position, widget.transform.rotation) as GameObject;

        //Assign tool mount as tool parent
        tool.transform.parent = mount;

        //Assign tool to widget
        widget.Tool = tool;

        //NetworkServer.Spawn(tool);
    }

	[Command]
	public void CmdAssign(int widgetID, int toolID)
	{
		//print("CmdAssign called: " + widgetID + ", " + toolID);

		//Save widget as temporary variable
		WidgetControlScript widget = GameVariables.Players[widgetID - 1];

		//Find widget tool mount
		Transform mount = widget.ToolMount;

		//Remove current tool from widget
		widget.RemoveTool();

        //For safety measures, destroy all children of widget
        Transform[] children = mount.transform.GetComponentsInChildren<Transform>(); 
        foreach(Transform child in children)
        {
            if(child != mount.transform)
                Destroy(child.gameObject);
        }

        //Instantiate tool in tool mount pos
        GameObject tool = Instantiate(tools[toolID - 1], mount.position, widget.transform.rotation) as GameObject;

        //Assign tool mount as tool parent
        tool.transform.parent = mount;

        //Assign tool to widget
        widget.Tool = tool;

		NetworkServer.Spawn(tool);
	}  




    public void PickUpTool(int widgetID, int oldToolID, int newToolID)
    {
        //savedTools.Add(newToolID);
        //print("PickUpTool: " + widgetID + "," + oldToolID + "," + newToolID);
        if(oldToolID != 0)
        {
            EnableTool(oldToolID);
        }

        DisableTool(newToolID);

        //Call commmand to server
        if(!isServer)
            CmdAssign(widgetID, newToolID);

        Assign(widgetID, newToolID);

    }



    public void EnableTool(int toolID)
    {
        Image img = toolboxList[toolID - 1].transform.GetChild(0).GetComponent<Image>();
        //print(img.name);

        if (savedTools.Contains(toolID))
            savedTools.Remove(toolID);

        img.enabled = true;
    }



    public void DisableTool(int toolID)
    {
        Image img = toolboxList[toolID - 1].transform.GetChild(0).GetComponent<Image>();

        savedTools.Add(toolID);

        img.enabled = false;
    }



    public List<int> SavedTools
    {
        get { return savedTools; }
        set { savedTools = value; }
    }
}  