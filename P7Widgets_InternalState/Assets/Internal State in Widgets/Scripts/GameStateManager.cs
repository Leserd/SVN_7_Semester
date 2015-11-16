using UnityEngine;
using System.Collections;

public static class GameStateManager {
	private static GameState _state = GameState.SETUP;

	public static GameState State
	{
		get { return _state; }
		set { _state = value; }
	}
}

public enum GameState
{
	SETUP,					//Setting up various variables
	TOOLBOX,				//When the widgets work in the toolbox
	MENU,					//When the menu is shown and the widgets do not need to be calculated
	GAME,					//When the widgets work during the game
}