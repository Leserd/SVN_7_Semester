﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ipNetworking : NetworkManager {

    bool notServer;

    public void StartupHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
        notServer = false;

    }

    public void JoinGame()
    {
        SetIPAdress();
        SetPort();
        NetworkManager.singleton.StartClient();
        notServer = true;
     
    }

    void SetIPAdress()
    {
        string ipAdress = GameObject.Find("InputIPAdress").transform.FindChild("Text").GetComponent<Text>().text;
        

        if(ipAdress == "")
        {
            ipAdress = "localhost";
        }

        NetworkManager.singleton.networkAddress = ipAdress;

    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }



    void OnLevelWasLoaded(int level)
    {
        if(level == 0)
        {

            StartCoroutine(SetupMenuButtons());


        }
        if(level == 1)
        {
            
            Debug.Log("GAME SCENE LOADED"); 
            GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);

            GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(isPressed);
            GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(LoadMenu);

            GameObject.Find("ToolboxDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ToolboxDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);

            GameObject.Find("ToolboxDisconnect").GetComponent<Button>().onClick.AddListener(isPressed);
            GameObject.Find("ToolboxDisconnect").GetComponent<Button>().onClick.AddListener(LoadMenu);

        }
        if (level == 2)
        {

            


        }


    }

    IEnumerator SetupMenuButtons()
    {

        yield return new WaitForSeconds(0.3f);

        if(notServer == false)
        {
            InitServerButton();
            InitToolboxButton();

        }
        else if(notServer == true)
        {
            InitServerButton();
            InitToolboxButton();
        }



    }
    
    void InitServerButton()
    {
        GameObject.Find("ButtonServer").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonServer").GetComponent<Button>().onClick.AddListener(StartupHost);

    }

    void InitToolboxButton()
    {

        GameObject.Find("ButtonConnectTool").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonConnectTool").GetComponent<Button>().onClick.AddListener(JoinGame);

    }

    void LoadMenu()
    {
        Application.LoadLevel(0);
    }

    void isPressed()
    {

        Debug.Log("Disconnect is pressed");

    }


}
