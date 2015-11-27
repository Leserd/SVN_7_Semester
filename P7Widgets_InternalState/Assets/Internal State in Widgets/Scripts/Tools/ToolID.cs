using UnityEngine;
using System.Collections;

public class ToolID : MonoBehaviour {

    [SerializeField] private int _id;


    public int ID
    {
        get {
            print("ID is :" + _id);
            return _id;
            
        }

        set { _id = value; }
    }
}
