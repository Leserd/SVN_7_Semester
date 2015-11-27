using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Toolbox_ToolAssign : MonoBehaviour {

	public int toolButtonID;
    
	public int toolToAssign;
	public int widgetToAssign;
	public Sprite image;

    private Image _myImage;
    private const float MOVE_LERP_SPEED = 5f;
    private const float SCALE_LERP_SPEED = 5f;

    void Start()
	{
        _myImage = GetComponent<Image>();

		if(image != null)
			GetComponent<Image>().sprite = image;
	}



    public void StartMoving(Vector2 dest, Vector3 scale)
    {
        StartCoroutine(MoveToDestination(dest, scale));
    }



    private IEnumerator MoveToDestination(Vector2 dest, Vector3 scale)
    {
        //_moving = true;

        while (_myImage.rectTransform.anchoredPosition != dest)
        {
            _myImage.rectTransform.anchoredPosition = Vector2.Lerp(_myImage.rectTransform.anchoredPosition, dest, MOVE_LERP_SPEED * Time.deltaTime);
            _myImage.rectTransform.localScale = Vector3.Lerp(_myImage.rectTransform.localScale, scale, SCALE_LERP_SPEED * Time.deltaTime);
            if (Vector2.Distance(_myImage.rectTransform.anchoredPosition, dest) < 3f)
            {
                _myImage.rectTransform.anchoredPosition = dest;
                break;
            }

            yield return new WaitForEndOfFrame();
            
        }

        transform.parent.GetComponent<Toolbox_Move>().ToolArrived();
        StopCoroutine("MoveToDestination");
    }
}