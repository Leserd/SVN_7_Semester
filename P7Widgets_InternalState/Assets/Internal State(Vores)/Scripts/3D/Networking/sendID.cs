using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class sendID : NetworkBehaviour {

    [SyncVar] public int widgetID;
    int widget1 = 511;
    int widget2 = 522;

    [SyncVar] public GameObject netWidget;


	// Use this for initialization
	void Start () {

        DontDestroyOnLoad(netWidget);
	}
	
	// Update is called once per frame
	void Update () {

        if (isClient && !isServer)
        {
            if (Input.GetKeyDown("space"))
            {

                CmdwidgetSwitch1();

            }
            if (Input.GetMouseButtonDown(0))
            {
                CmdwidgetSwitch2();
            }
        }

	
	}

    [Command]
    void CmdwidgetSwitch1()
    {

        GameObject.Find("Manager").GetComponent<testNetworkSend>().ChangeID(widget1);
        Debug.Log("wSwitch1 Run");


    }
    [Command]
    void CmdwidgetSwitch2()
    {


        GameObject.Find("Manager").GetComponent<testNetworkSend>().ChangeID(widget2);
        Debug.Log("wSwitch2 Run");

    }


}
