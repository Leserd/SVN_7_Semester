using UnityEngine;
using System.Collections;

public class bullet3D : MonoBehaviour {


	void Start(){
		Destroy(gameObject, 1f);					//Destroy this bullet in 1 second unless it hits something beforehand
	}


    void OnCollisionEnter(Collision col)
    {
		if(col.gameObject.tag == "ForceField")
		{
			Destroy(gameObject);
		}
       

        if (col.gameObject.tag == "Robot")
        {
			GameVariables.GameManager.RobotDied();
            Destroy(col.gameObject);
        }
    }

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Robot")
		{
			GameVariables.GameManager.RobotDied();
			Destroy(col.gameObject);
		}
		else if(col.gameObject.tag == "ForceField" || col.name.Contains("Platform") != true)
		{
			Destroy(gameObject);
		}
	}
}
