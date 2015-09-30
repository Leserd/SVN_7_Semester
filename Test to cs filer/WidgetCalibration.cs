using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WidgetCalibration : MonoBehaviour {

	public Text textField;
	public Image bar;
	public float delay;

	private string standardText;

	private WidgetDetectionAlgorithm algor;
	// Use this for initialization
	IEnumerator Start () {
		yield return null;
		algor = GetComptectionAlgorithm> ();
		standardText = textField.text;
		algor.minLength = 0;
		algor.maxLength = Mathf.Infinity;
		StartCoroutine (WidgetFindAlgorithm());
	}

	IEnumerator WidgetFindAlgorithm() {
		Widget[] widgets;
		while (true) {
			do {
				yield return null;
				widgets = algor.widgets;
				textField.text = "" + standardText + ": TouchPoints: " + Input.touchCount;


			} while (widgets.Length != 1);

			float timeMark = Time.time;
			textField.text = "Wait...";
			while (timeMark + delay > Time.time && algor.widgets.Length == 1) {
				bar.fillAmount = 1-((timeMark + delay) - Time.time) / delay;
				yield return null;
			}
			bar.fillAmount = 0;

			if (algor.widgets.Length == 1) {
				Widget widget = algor.widgets [0];
				float length = Vector3.Distance (Input.touches [widget.points [0]].position, Input.touches [widget.points [1]].position);
				textField.text = "Length: " + length;
				PlayerPrefs.SetFloat("minWidgetLength", 0.9f * length);
				PlayerPrefs.SetFloat("maxWidgetLength", 1.1f * length);
				while(Input.touchCount != 0) {
					yield return null;
				}	
			}


		}
	}

	public void ExitButton() {
		Application.LoadLevel(0);
	}
}
