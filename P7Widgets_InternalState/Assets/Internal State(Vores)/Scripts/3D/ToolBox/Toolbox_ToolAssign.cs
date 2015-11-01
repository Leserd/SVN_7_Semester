using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Toolbox_ToolAssign : MonoBehaviour {

	public int toolButtonID;

	public int toolToAssign;
	public int widgetToAssign;
	public Sprite image;

	void Start()
	{
		if(image != null)
			GetComponent<Image>().sprite = image;
	}
}