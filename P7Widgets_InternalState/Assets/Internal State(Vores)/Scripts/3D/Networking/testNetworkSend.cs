using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class testNetworkSend : NetworkBehaviour {

    [SyncVar]
    public int ID;

    void Start()
    {

    }

    public void ChangeID(int i)
    {

        ID = i;
        print(ID);
        Debug.Log("Change ID Run");

    }


}
