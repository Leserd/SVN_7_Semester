using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Robot")
		{
			Destroy(col.gameObject);
			print("Robot died.");
		}
	}
}