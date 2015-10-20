using UnityEngine;
using System.Collections;

public class ToolAssign : MonoBehaviour {

	public GameObject[] tools;
	public WidgetControlScript[] widgets;

	public void Assign(int widgetID, int toolID)
	{
		//Save widget as temporary variable
		WidgetControlScript widget = widgets[widgetID - 1];

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
	}
}