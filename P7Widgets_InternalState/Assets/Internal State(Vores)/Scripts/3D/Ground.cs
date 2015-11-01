using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Ground : NetworkBehaviour {


	//void OnCollisionExit(Collision col)
	//{
	//	if(col.gameObject.tag == "Robot")
	//	{
	//		col.gameObject.GetComponent<RobotMovement3D>().Grounded = false;
	//	}
	//}



	//void OnCollisionStay(Collision col)
	//{
	//	if(col.gameObject.tag == "Robot" && col.gameObject.GetComponent<RobotMovement3D>().Grounded == false)
	//	{
	//		col.gameObject.GetComponent<RobotMovement3D>().Grounded = true;
	//	}
	//}
}