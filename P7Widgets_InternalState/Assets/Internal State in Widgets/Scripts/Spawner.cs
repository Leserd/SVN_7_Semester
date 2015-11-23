using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    private Animation _animation;

    public AnimationClip spawnOpenClip;
    public AnimationClip spawnCloseClip;



    public void Awake()
    {
        _animation = GetComponent<Animation>();
    }



    public void Open()
    {
        if (spawnOpenClip != null)
        {
            _animation.Play(spawnOpenClip.name);
        }
    }



    public void Close()
    {
        if (spawnCloseClip != null)
        {
            _animation.Play(spawnCloseClip.name);
        }
    }
}