using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Widget2 {
	private int _id;													//The ID of the widget
	private List<Vector2> _points = new List<Vector2>();				//The touch points of the widget
	private Vector3 _angles = new Vector3();
	private PredefinedWidgets _widgetManager;

	private List<float> _angleList = new List<float>();

	const float ANGLE_CLOSENESS_THRESHOLD = 20;

	public int ID
	{
		get { return _id; }
		set { _id = value; }
	}

	public List<Vector2> Points
	{
		get { return _points; }
		set { _points = value; }
	}

	public Widget2(List<Vector2> points)
	{
		Debug.Log("Widget has been created.");
		_points = points;
		_widgetManager = GameObject.Find("WidgetManager").GetComponent<PredefinedWidgets>();
		DetermineValidity();
	}

	void DetermineValidity()
	{
		float sideA, sideB, sideC;
		sideA = Vector2.Distance(_points[0], _points[1]);
		sideB = Vector2.Distance(_points[1], _points[2]);
		sideC = Vector2.Distance(_points[2], _points[0]);

		Debug.Log("Side lengths: " + sideA + ", " + sideB + ", " + sideC + ".");

		//Calculate angles and match them with something
		_angleList = CalcAngle(sideA, sideB, sideC);

		//Sort the angle-list for comparison with predefined widgets
		_angles = SortAngles();

		//float.CompareTo on all angles of this widget to all angles in pre-defined widgets
		CompareToPredefined();
		//Print whether the widget is valid or not
		
	}

	List<float> CalcAngle(float a, float b, float c)
	{
		List<float> result = new List<float>();
		float angle, rad, degrees;

		//A
		angle = (b * b + c * c - a * a) / (2 * b * c);
		rad = Mathf.Acos(angle);
		degrees = rad * Mathf.Rad2Deg;
		result.Add(degrees);
		
		//B
		angle = (c * c + a * a - b * b) / (2 * c * a);
		rad = Mathf.Acos(angle);
		degrees = rad * Mathf.Rad2Deg;
		result.Add(degrees);
		
		//C
		angle = (a * a + b * b - c * c) / (2 * a * b);
		rad = Mathf.Acos(angle);
		degrees = rad * Mathf.Rad2Deg;
		result.Add(degrees);

		//save results in a list for sorting.
		_angleList = result;

		//return new Vector3(a,b,c);
		return result;
	}

	Vector3 SortAngles()
	{
		Vector3 result = Vector3.zero;
		float temp;

		Debug.Log("Angles before sorting: " + _angleList[0] + ", " + _angleList[1] + ", " + _angleList[2] + ".");

		for(int i = 0; i < 2; i++)
		{
			for(int o = 0; o < 2; o++)
			{
				if(_angleList[o] < _angleList[o+1]) {
					continue;
				} else
				{
					temp = _angleList[o + 1];
					_angleList[o + 1] = _angleList[o];
					_angleList[o] = temp;
				}
			}
		}

		result = new Vector3(_angleList[0], _angleList[1], _angleList[2]);

		Debug.Log("Angles after sorting: " + result.ToString() + ".");

		return result;
	}

	void CompareToPredefined()
	{
		bool valid = false;

		for(int i = 0; i < _widgetManager.WidgetAngles.Length; i++)
		{
			float dist = Vector3.Distance(_angles, _widgetManager.WidgetAngles[i]);
			if(dist < ANGLE_CLOSENESS_THRESHOLD)
			{
				Debug.Log("Predefined Widget number " + i + " is " + dist.ToString() + " different from these angles.");
				valid = true;
			} else
			{
				Debug.Log("Predefined Widget number " + i + " is too different from these angles.");
			}
		}
		Debug.Log("Validity of " + ID.ToString() + ": " + valid.ToString());
		if(valid == false)
		{
			
		}
	}
}