using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Robot")
		{
			GameVariables.GameManager.RobotDied(col.transform);
			Destroy(col.gameObject);
		}
	}
}