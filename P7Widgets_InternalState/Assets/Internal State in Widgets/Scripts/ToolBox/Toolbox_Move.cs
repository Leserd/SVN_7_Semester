using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Toolbox_Move : MonoBehaviour {

	private float _maxX;
	private float _minX = SPACE_BETWEEN_TOOlS;

	private float _panSpeed = 10f;

	private bool _moving = false;

	private RectTransform _rect;

	List<Toolbox_ToolAssign> images = new List<Toolbox_ToolAssign>();

	public int currentImage = 0;

	public Button leftArrow, rightArrow;

    const float SPACE_BETWEEN_TOOlS = 700;

	void Start()
	{
		if(leftArrow != null)
			leftArrow.interactable = false;
		_rect = GetComponent<RectTransform>();
		FindImages();
	}



	void FindImages()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			if(transform.GetChild(i).GetComponent<Toolbox_ToolAssign>())
			{
				images.Add(transform.GetChild(i).GetComponent<Toolbox_ToolAssign>());
			}
		}
		_maxX = SPACE_BETWEEN_TOOlS - SPACE_BETWEEN_TOOlS * (images.Count - 1);         //Every tool in the toolbox has an X-value SPACE_BETWEEN_TOOlS from the previous
    }



	public void MoveUI(int dir)
	{
		if(_moving == false)
		{
			if(images.Count > 1)
				StartCoroutine(Move(dir));
		}
	}



	IEnumerator Move(int dir)
	{
		_moving = true;
		Vector2 destination = new Vector2(_rect.anchoredPosition.x + (SPACE_BETWEEN_TOOlS * dir), _rect.anchoredPosition.y);

		while(_rect.anchoredPosition != destination){
			_rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, destination, _panSpeed * Time.deltaTime);

			if(Vector2.Distance(_rect.anchoredPosition, destination) < 3f)
			{
				_rect.anchoredPosition = destination;
				_moving = false;
				UpdateButtonStates();
				currentImage -= dir;
				break;
			}

			yield return new WaitForEndOfFrame();
		}
	}



	void UpdateButtonStates()
	{
		if(_rect.anchoredPosition.x != _minX && _rect.anchoredPosition.x != _maxX)
		{
			leftArrow.interactable = true;
			rightArrow.interactable = true;
		}
		else if(_rect.anchoredPosition.x == _minX)
		{
			leftArrow.interactable = false;
		}
		else if(_rect.anchoredPosition.x == _maxX)
		{
			rightArrow.interactable = false;
		}
	}


	public Image CurrentImage
	{
		get { return images[currentImage].GetComponent<Image>(); }
	}
}