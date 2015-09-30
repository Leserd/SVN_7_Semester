using UnityEngine;
using System.Collections;

public class ImageSavePoint : MonoBehaviour {

	Transform _myTransform;
	Camera _camera;
	Vector3 _textPos;
	Vector2 _boxSize;

	// Use this for initialization
	void Awake () {
		_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		_myTransform = transform;
		_textPos = _camera.WorldToScreenPoint(transform.position);
		_boxSize = new Vector2(80, 25);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.Label(new Rect(_textPos.x - (_boxSize.x/2), Screen.height - _textPos.y - (_boxSize.y / 2), _boxSize.x, _boxSize.y), "Place Widget");

	}
}