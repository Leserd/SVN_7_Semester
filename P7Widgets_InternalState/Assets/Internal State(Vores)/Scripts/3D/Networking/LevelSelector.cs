using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LevelSelector : NetworkBehaviour {

	void Start()
	{
		if(Application.loadedLevel == 1)
		{
			ToggleCanvas();
		}
	}


	void OnLevelWasLoaded(int level)
	{
		print("LEvel: " + level);
		if(level == 1)
		{
			ToggleCanvas();
		}
	}


	void ToggleCanvas()
	{
		if(isServer)
		{
			if(GameObject.Find("ToolboxCanvas") && GameObject.Find("ToolboxCamera"))
			{
				GameObject.Find("ToolboxCanvas").GetComponent<Canvas>().enabled = false;
				GameObject.Find("ToolboxCamera").GetComponent<Camera>().enabled = false;
			}
		}

		if(!isServer && isClient)
		{

			if(GameObject.Find("LevelCanvas") && GameObject.Find("LevelCamera"))
			{
				GameObject.Find("LevelCamera").GetComponent<Camera>().enabled = false;
				GameObject.Find("LevelCanvas").GetComponent<Canvas>().enabled = false;
			}
		}
	}

}
