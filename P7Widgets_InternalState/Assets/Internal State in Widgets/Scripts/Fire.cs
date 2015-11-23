using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

    AudioSource _audio;
    public AudioClip fireClip;
    public int curLevel = 3;



	// Use this for initialization
	void Awake () {
        _audio = GetComponent<AudioSource>();
	}
	


	// Update is called once per frame
	void Update () {
	    if(GameVariables.GameManager != null)
        {
            if (GameVariables.GameManager.CurrentLevel == curLevel)
                PlaySound(fireClip);
        }
	}



    public void PlaySound(AudioClip clip)
    {
        if (clip != null && _audio.isPlaying == false)
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
