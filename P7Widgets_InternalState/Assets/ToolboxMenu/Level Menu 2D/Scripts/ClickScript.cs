using UnityEngine;
using System.Collections;

public class ClickScript : MonoBehaviour
{

    void Awake()
    {
        LevelMenu2D.I.OnItemClicked += HandleOnItemClicked;

        //Swipe Gestures
        // SwipeDetector.OnSwipeLeft += HandleOnSwipeLeft;
        // SwipeDetector.OnSwipeRight += HandleOnSwipeRight;
    }

    void HandleOnItemClicked(int itemIndex, GameObject itemObject)
    {
        //Custom Code here
        iTween.ColorTo(itemObject, Color.red, 0.2f);
        iTween.ColorTo(itemObject, iTween.Hash("color", Color.white, "delay", 0.2f, "time", 0.2f));
    }

    void HandleOnSwipeLeft()
    {
        LevelMenu2D.I.gotoNextItem();
    }

    void HandleOnSwipeRight()
    {
        LevelMenu2D.I.gotoBackItem();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}