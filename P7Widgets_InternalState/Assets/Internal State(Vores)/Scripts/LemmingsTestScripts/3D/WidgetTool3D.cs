using UnityEngine;
using System.Collections;

public class WidgetTool3D : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public virtual void OnCollisionEnter(Collision col)
	{
		Test();
	}



	public virtual void Test()
	{
		print("base");
	}

}