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
	private static GameManagerScript _gameManager;
	private static Goal _currentGoal;
	private static Transform _currentSpawn;
	private static Image _savedToolFill;

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



	public static GameManagerScript GameManager
	{
		get { return _gameManager; }
		set { _gameManager = value; }
	}



	public static Goal CurrentGoal
	{
		get { return _currentGoal; }
		set { _currentGoal = value; }
	}



	public static Transform CurrentSpawn
	{
		get { return _currentSpawn; }
		set { _currentSpawn = value; }
	}



	public static Image SavedToolFill
	{
		get { return _savedToolFill; }
		set { _savedToolFill = value; }
	}
}
