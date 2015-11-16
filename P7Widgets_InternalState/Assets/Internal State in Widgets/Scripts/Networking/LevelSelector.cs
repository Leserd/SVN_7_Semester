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
		if(!isServer && isClient)
		{
			if(GameObject.Find("ToolboxCanvas") && GameObject.Find("ToolboxCamera"))
			{
				GameObject.Find("ToolboxCanvas").GetComponent<Canvas>().enabled = true;
				GameObject.Find("LevelCamera").GetComponent<Camera>().enabled = false;
			}
		}

		if(isServer)
		{

			if(GameObject.Find("LevelCanvas") && GameObject.Find("LevelCamera"))
			{
				GameObject.Find("LevelCanvas").GetComponent<Canvas>().enabled = true;
				GameObject.Find("ToolboxCamera").GetComponent<Camera>().enabled = false;
			}
		}
	}

}
