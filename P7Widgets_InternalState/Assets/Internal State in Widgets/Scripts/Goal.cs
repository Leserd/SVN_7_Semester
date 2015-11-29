using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
	
	public int level;

	private float _timeBeforeDestroy = 0.1f;
	private Vector3 _startPos;
	private Vector3 _endPos;

	//const float DISTANCE_THRESHOLD = 0.5f;					//The minimum distance from start to end before arriving
	const float LERP_TIME = 1.5f;

    private Animation _animation;

    public AnimationClip goalOpenClip;
    public AnimationClip goalCloseClip;


    void Awake()
	{
		_startPos = transform.position;
		if(GetComponent<Spawner>())
			this.enabled = false;

        _animation = GetComponent<Animation>();

        if (goalOpenClip == null)
            goalOpenClip = Resources.Load<AnimationClip>("Animations/GoalLift/GoalOpen");

        if (goalCloseClip == null)
            goalCloseClip = Resources.Load<AnimationClip>("Animations/GoalLift/GoalClose");
    }



	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Robot" && col.GetComponent<Rigidbody>().isKinematic == false)
		{
			print("Robot reached goal!");
			GameVariables.GameManager.RobotScored(col.transform);
			//col.GetComponent<RobotMovement>().enabled = false;
			//col.GetComponent<Rigidbody>().isKinematic = true;
			Destroy(col.gameObject, _timeBeforeDestroy);					

			//CheckForRemainingRobots();
		}
	}



	void CheckForRemainingRobots()
	{
		if(level == GameVariables.GameManager.CurrentLevel)
		{
			if(GameVariables.GameManager.Score >= GameVariables.GameManager.robotsToSpawn[level - 1] && GameVariables.GameManager.RobotsRemaining == 0)
			{
				//Start coroutine with MoveToNextLocation
				StartCoroutine(MoveToNextLocation());
			}
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Y))
		{
			StartCoroutine(MoveToNextLocation());
		}
        if (Input.GetKeyDown(KeyCode.T))
        {
            GetComponent<Animation>().Play(goalOpenClip.name);
        }
    }

	public IEnumerator MoveToNextLocation()
	{
		float elapsedTime = 0;
		//Find next spawnlocation
		_endPos = GameVariables.GameManager.LevelLocation(GameVariables.GameManager.CurrentLevel).FindChild("SpawnLoc").position;

        //Play close-container animation and wait the duration of it before starting to move
        Close();
		//While(true) lerp there
		//if arrived, stopcoroutine

		yield return new WaitForSeconds(1f);
		
		while(elapsedTime < LERP_TIME)
		{
			transform.position = Vector3.Lerp(_startPos, _endPos, (elapsedTime / LERP_TIME));
			elapsedTime += Time.deltaTime;	
			
			yield return new WaitForEndOfFrame();
		}

		GameVariables.CurrentSpawn = this.transform;

        //transform.position = _endPos;
 

        //Make this a child of the correct level object
        transform.parent = GameVariables.GameManager.LevelLocation(level);

		//Give this unit the Spawner script so that the game manager recognises it as spawn
		gameObject.AddComponent<Spawner>();


        //Assign this as new spawn location
        GameVariables.CurrentSpawn = transform;

        gameObject.name = "Spawner" + (GameVariables.GameManager.CurrentLevel+1);

		//Tell game manager to prepare for a new level
		GameVariables.GameManager.PrepareNewLevel();

		//Destroy this Goal component as it will from now on only be a spawner
		Destroy(GetComponent<Goal>());
        if (GetComponent<ThisIsGoal>())
            Destroy(GetComponent<ThisIsGoal>());

	}


    public void Open()
    {
        if(goalOpenClip != null && GetComponent<Animation>())
        {
            //AnimationClip open = Resources.Load<AnimationClip>("Animations/GoalLift/GoalOpen");
            //_animation.Play(open.name);
            _animation.Play(goalOpenClip.name);
            //DebugConsole.Log("Playing goal open");
        }
        else
        {
            //DebugConsole.Log("Can't play goalOpen");
        }
    }



    public void Close()
    {
        if (goalCloseClip != null && GetComponent<Animation>())
        {
            GetComponent<Animation>().Play(goalCloseClip.name);
           // DebugConsole.Log("Playing goal close");
        }
        else
        {
           // DebugConsole.Log("Can't play goal close");
        }
    }
}
