using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct TempWidget
{
	int id;
	Vector3 angles;
	Vector3 sideLengths;
}

public class TouchManager : MonoBehaviour {

	//private List<GameObject> touchList = new List<GameObject>();
	private GameObject[] touchesOld;
	private RaycastHit hit;
	public LayerMask touchInputMask;
	public GameObject touchText;
	private int touchCount = 0;
	private List<Vector2> inputList = new List<Vector2>();

	List<TempWidget> _tempWidgets = new List<TempWidget>();
	List<List<Vector2>> _touchLists = new List<List<Vector2>>(); 

	const float DIST_THRESHOLD = 300;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Hvis der er over 3 touch points, begynd at lede efter widgets
		if(Input.touchCount >= 3)
		{
			//Clear listen
			_touchLists.Clear();

			for(int i = 0; i < Input.touchCount; i++)
			{
				//Check om der findes en liste
				if(_touchLists.Count == 0)
				{
					//Hvis ikke, lav ny liste og smid touch point deri
					List<Vector2> tempList = new List<Vector2>();
					_touchLists.Add(tempList);
					tempList.Add(Input.touches[i].position);
				} else
				{
					//Find den liste der har punkter tættest på, OBS: FINDER IKKE DEN DER ER TÆTTEST PÅ
					for(int o = 0; o < _touchLists.Count; o++)
					{
						//Hvis punktet er tæt på, smid dette punkt i den liste også
						if(Vector2.Distance(Input.touches[i].position, _touchLists[o][0]) < DIST_THRESHOLD)
						{
							_touchLists[o].Add(Input.touches[i].position);
							
							if(_touchLists[o].Count == 3)
							{
								//Hvis den individuelle liste nu har 3 point, tjek om der kan findes en valid Widget deri
								Widget2 _tempWidget = new Widget2(_touchLists[o]);
								//CalculateAnglesInList(_touchLists[o]);
							}
						} else
						{
							//Hvis den er langt væk, lav ny liste til dette punkt
							List<Vector2> tempList = new List<Vector2>();
							_touchLists.Add(tempList);
							tempList.Add(Input.touches[i].position);
						}

						
					}
					//Check punkt-distance til punkt i liste
					//Hvis for langt væk, lav ny liste og smid deri
				}

				
			}
			//Gruppér efter distance

		}

		if(Input.touchCount > 1)
		{
			for(int i = 0; i<Input.touchCount; i++){
				float x = 0; 
				float y = 0; 
				float z = -30;
				Vector3 startPoint = Vector3.zero;
				Vector3 endPoint = Vector3.zero;

				if(i == Input.touchCount - 1)
				{
					startPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, z));
					endPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, z));

					Debug.DrawLine(startPoint, endPoint, Color.white);
				} else
				{
					startPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, z));
					endPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[i + 1].position.x, Input.touches[i + 1].position.y, z));

					Debug.DrawLine(startPoint, endPoint, Color.white);
				}
				//print("Start: " + startPoint.ToString() + ", End: " + endPoint.ToString());
			}
		}

		#if UNITY_EDITOR

		if(Input.touchCount == 1)
		{
			//print(Input.touches[0].position);
		}

		if(Input.GetKeyDown(KeyCode.K))
		{
			ShowTouchPoints();
		}

			
		#endif

		if(Input.touchCount > 0)
		{

		}
	}


	public void CalculateAnglesInList(List<Vector2> list)
	{
		for(int i = 0; i < list.Count; i++)
		{

		}
	}


	public void ShowTouchPoints()
	{
		inputList = new List<Vector2>();
		print("Starting count!");
		foreach(Touch touch in Input.touches)
		{
			//touchCount++;
			//GameObject t = (GameObject)Instantiate(touchText, new Vector3(0, 0+30*touchCount, 0), Quaternion.identity);
			//t.GetComponent<TextMesh>().text = touch.position.ToString();
			inputList.Add(touch.position);
		}
		print("Amount of touches: " + inputList.Count);

		for(int i = 0; i < inputList.Count; i++)
		{
			//print("Angle: " + Vector2.Angle(v, inputList[i]));
			//Make sure the touches you calculate are close to each other, e.g. distance < 100 (depending on size of screen)
			//Angle A = cos(A) = (b^2 + c^2 - a^2) / (2 * b * c)
			if(inputList.Count == 3)
			{
				float angle;
				float distA = 0;
				float distB = 0;
				float distC = 0;
				if(i == 0){
					distA = Vector2.Distance(inputList[0], inputList[1]);
					distB = Vector2.Distance(inputList[1], inputList[2]);
					distC = Vector2.Distance(inputList[2], inputList[0]);
				} else if(i == 1){
					distA = Vector2.Distance(inputList[1], inputList[2]);
					distB = Vector2.Distance(inputList[2], inputList[0]);
					distC = Vector2.Distance(inputList[0], inputList[1]);
				} else if(i == 2)
				{
					distA = Vector2.Distance(inputList[2], inputList[0]);
					distB = Vector2.Distance(inputList[0], inputList[1]);
					distC = Vector2.Distance(inputList[1], inputList[2]);
				}

				print("A: " + distA + ", B: " + distB + ", C: " + distC);

				angle = (distB * distB + distC * distC - distA * distA) / (2 * distB * distC);
				float rad = Mathf.Acos(angle);
				float result = rad * Mathf.Rad2Deg;

				print("Angle for Point " + i + ": " + result.ToString() + ".");
			} else
			{
				print("There are not 3 touch points on the screen.");
			}
		}
	}

	
}