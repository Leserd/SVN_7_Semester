using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LevelSelector : NetworkBehaviour {


	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

        if (isServer)
        {
            GameObject.Find("ToolboxCanvas").SetActive(false);
            GameObject.Find("Toolbox Camera").SetActive(false);

        }

        if (!isServer && isClient)
        {

            GameObject.Find("Main Camera").SetActive(false);
            GameObject.Find("Canvas").SetActive(false);

        }

    }


}
