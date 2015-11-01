using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public static class GameVariables {

	private static Button _levelDisconnect;
	private static Button _toolboxDisconnect;
	private static Toolbox_Move _toolboxCanvas;
	private static List<WidgetControlScript> _players;
	private static WidgetDetectionAlgorithm _widgetAlgorithm;



	public static Button LevelDisconnect{
		get { return _levelDisconnect; }
		set { _levelDisconnect = value; }
	}



	public static Button ToolboxDisconnect
	{
		get { return _toolboxDisconnect; }
		set { _toolboxDisconnect = value; }
	}



	public static Toolbox_Move ToolboxCanvas
	{
		get { return _toolboxCanvas; }
		set { _toolboxCanvas = value; }
	}



	public static List<WidgetControlScript> Players
	{
		get { return _players; }
		set { _players = value; }
	}



	public static WidgetDetectionAlgorithm WidgetAlgorithm
	{
		get { return _widgetAlgorithm; }
		set { _widgetAlgorithm = value; }
	}
}
