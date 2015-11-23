using UnityEngine;
using System.Collections;

public class bullet3D : MonoBehaviour {

    AudioSource _audio;
    
    public AudioClip destroyedSound;

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
			GameVariables.GameManager.RobotDied(col.transform);
            Destroy(col.gameObject);
        }
    }



	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Robot")
		{
			GameVariables.GameManager.RobotDied(col.transform);
			Destroy(col.gameObject);
		}
		else if(col.gameObject.tag == "ForceField" )
		{
            col.GetComponent<AudioSource>().clip = destroyedSound;
            col.GetComponent<AudioSource>().Play();
            Destroy(gameObject);
		}
        else if (col.name.Contains("Trampoline") != true && col.name.Contains("Platform") != true)
        {
            Destroy(gameObject);
        }
	}



    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _audio.clip = clip;
            _audio.Play();
        }
        else
        {
            Debug.LogWarning("No audio clip was found.");
        }
    }
}
