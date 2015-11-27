using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {

	private AudioSource _audio;
	private AudioClip _conveyorSound;					//The sound of the conveyor belt 

    private Animation _animation;
    private AnimationClip _conveyorBeltClip;

	void Awake()
	{
		_audio = GetComponent<AudioSource>();
	}

	void Update()										//Maybe this should be in OnTriggerStay?
	{
		//if collision size > 0
		//foreach Collision col in ****
		//if any col has tag Robot
		//start conveyor belt animation loop and sound

		//if no robots are collied with, stop belt animation and sound
	}


    void OnCollisionStay(Collision col)
    {
        if(col.transform.tag == "Robot")
        {
            if(_audio.isPlaying == false)
            {
                PlaySound(_conveyorSound);
            }
        }
    }



	public void PlaySound(AudioClip clip)
	{
		if(clip != null)
		{
			_audio.clip = clip;
			_audio.Play();
		}
		else
		{
			Debug.LogWarning("No audio clip was found.");
		}
	}


	public void StopSound()
	{
		if(_audio.isPlaying)
		{
			_audio.Stop();
		}
	}
}