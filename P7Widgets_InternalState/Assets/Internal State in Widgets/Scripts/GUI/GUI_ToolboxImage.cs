using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_ToolboxImage {
    private Image _img;
    private Vector2 _curPos;
    private Vector2 _endPos;



    public GUI_ToolboxImage(Image i, Vector2 dest)
    {

    }



    public Vector3 EndPos
    {
        get { return _endPos; }
        set { _endPos = value;
            if (_img.rectTransform.anchoredPosition != _endPos)
            {
               // _img.GetComponent<Toolbox_ToolAssign>().StartMoving(_endPos);
            }
        }
    }
	
}
