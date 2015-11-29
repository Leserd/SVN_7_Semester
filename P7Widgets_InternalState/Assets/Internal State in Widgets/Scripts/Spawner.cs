using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    private Animation _animation;

    public AnimationClip spawnOpenClip;
    public AnimationClip spawnCloseClip;



    public void Awake()
    {
        _animation = GetComponent<Animation>();


        if (spawnOpenClip == null)
            spawnOpenClip = Resources.Load<AnimationClip>("Animations/GoalLift/SpawnOpen");

        if (spawnCloseClip == null)
            spawnCloseClip = Resources.Load<AnimationClip>("Animations/GoalLift/SpawnClose");
    }



    public void Open()
    {
        if (spawnOpenClip != null)
        {
            //AnimationClip open = Resources.Load<AnimationClip>("Animations/GoalLift/SpawnOpen");
            //_animation.Play(open.name);
            _animation.Play(spawnOpenClip.name);
            //DebugConsole.Log("Playing spawn open");
        }
        else
        {
            //DebugConsole.Log("Can't play spawnOpen");
        }
    }



    public void Close()
    {
        if (spawnCloseClip != null)
        {
            _animation.Play(spawnCloseClip.name);
            //DebugConsole.Log("Playing spawn close");
        }
        else
        {
            //DebugConsole.Log("Can't play spawn close");
        }
    }
}