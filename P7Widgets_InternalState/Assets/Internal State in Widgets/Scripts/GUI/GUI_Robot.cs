using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_Robot
{
    private GUIRobotState _state = GUIRobotState.Alive;
    private Image _robotImage;
    private const string GUI_PATH = "GUI/RobotGUI/";
    

    public GUI_Robot(Image img)
    {
        _robotImage = img;
        UpdateImage();
    }



    public void UpdateImage()
    {
        _robotImage.sprite = Resources.Load<Sprite>(GUI_PATH + "Robot" + _state.ToString());
    }



    public GUIRobotState State
    {
        get { return _state; }
        set { _state = value;
            UpdateImage();
        }
    }



    public Image RobotImage
    {
        get { return _robotImage; }
        set { _robotImage = value; }
    }

}

public enum GUIRobotState
{
    Alive,
    Dead,
    Scored
}