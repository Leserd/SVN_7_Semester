using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Toolbox_Move : MonoBehaviour
{

    private float _maxX;

    private float _panSpeed = 10f;

    private bool _moving = false;

    private RectTransform _rect;

    public int currentImage = 0;

    public Button leftArrow, rightArrow;

    const float SPACE_BETWEEN_TOOlS = 700;
    private float _minX = SPACE_BETWEEN_TOOlS;

    //*****Variables for new toolbox system***********
    public Vector2[] imgPositions;     //array index 0 is the leftmost image, index 1 (2nd element) is the middle image, index 2 (3rd element) is rightmost image
    public Vector3[] imgScales;          //array index 0 is the leftmost image, index 1 (2nd element) is the middle image, index 2 (3rd element) is rightmost image
    private List<Toolbox_ToolAssign> _toolImages = new List<Toolbox_ToolAssign>();
    private int toolsArrived;           //number of tools that have arrived to their position when rotating

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        FindImages();
    }



    void FindImages()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Toolbox_ToolAssign>())
            {
                _toolImages.Add(transform.GetChild(i).GetComponent<Toolbox_ToolAssign>());
            }
        }
        SetUpPosAndScale();
        //_maxX = SPACE_BETWEEN_TOOlS - SPACE_BETWEEN_TOOlS * (_toolImages.Count - 1);         //Every tool in the toolbox has an X-value SPACE_BETWEEN_TOOlS from the previous
    }




    private void SetUpPosAndScale()
    {

        for (int i = 0; i < _toolImages.Count; i++)
        {
            //print(_toolImages[i].name);
            imgPositions[i] = _toolImages[i].GetComponent<Image>().rectTransform.anchoredPosition;
            imgScales[i] = _toolImages[i].GetComponent<Image>().rectTransform.localScale;
            if (i == 1)
            {
                _toolImages[i].transform.SetSiblingIndex(2);
            }
        }
    }


    public void MoveUI(int dir)
    {
        if (_moving == false)
        {
            _moving = true;

            //Disable arrow buttons
            UpdateArrowButtons();

            //Rotate images in list
            RotateToolImagesList(dir);

            //Tell each image to move to new position
            UpdateToolPositions();
        }
    }



    private void RotateToolImagesList(int dir)
    {
        //print("Before: " + _toolImages[0] + ", " + _toolImages[1] + ", " + _toolImages[2]);
        Toolbox_ToolAssign[] temp = new Toolbox_ToolAssign[3];

        for (int i = 0; i < _toolImages.Count; i++)
        {
            temp[i] = _toolImages[i];
        }

        //List<Toolbox_ToolAssign> temp = _toolImages;
        //print("Temp: " + temp[0] + ", " + temp[1] + ", " + temp[2]);
        for (int i = 0; i < _toolImages.Count; i++)
        {
            //Toolbox_ToolAssign temp;
            if (i == 0 && dir == -1)    //left issue
            {
                //print("Before left: " + temp[0].transform.name + ", " + temp[1].transform.name + ", " + temp[2].transform.name);
                temp[i] = _toolImages[_toolImages.Count - 1];
                //print("After left: " + temp[0].transform.name + ", " + temp[1].transform.name + ", " + temp[2].transform.name);

            }
            else if (i == _toolImages.Count - 1 && dir == 1)    //right issue +
            {
                //print("Before right: " + temp[0].transform.name + ", " + temp[1].transform.name + ", " + temp[2].transform.name);
                temp[i] = _toolImages[0];
                //print("After right: " + temp[0].transform.name + ", " + temp[1].transform.name + ", " + temp[2].transform.name);
            }
            else // normal
            {
                //print("Before mid: " + temp[0].transform.name + ", " + temp[1].transform.name + ", " + temp[2].transform.name);
                temp[i] = _toolImages[i + dir];
                //print("After mid: " + temp[0].transform.name + ", " + temp[1].transform.name + ", " + temp[2].transform.name);
            }
        }

        for (int i = 0; i < _toolImages.Count; i++)
        {
            _toolImages[i] = temp[i];
        }


        //print("After: " + _toolImages[0] + ", " + _toolImages[1] + ", " + _toolImages[2]);
    }



    private void UpdateToolPositions()
    {
        for (int i = 0; i < _toolImages.Count; i++)
        {
            _toolImages[i].StartMoving(imgPositions[i], imgScales[i]);

            //bring image to front if it is the middle one
            if (i == 1)
            {
                _toolImages[i].transform.SetSiblingIndex(2);
            }
        }
    }


    public void ToolArrived()
    {
        toolsArrived++;
        if (toolsArrived == _toolImages.Count)
        {
            _moving = false;
            UpdateArrowButtons();
            toolsArrived = 0;
            //print("Tools reset");
        }
    }



    void UpdateArrowButtons()
    {
        leftArrow.interactable = !_moving;
        rightArrow.interactable = !_moving;
    }


    public Image CurrentImage
    {
        get { return _toolImages[1].GetComponent<Image>(); }
    }
}