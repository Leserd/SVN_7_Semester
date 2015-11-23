using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{

	void Start () {
        Destroy(gameObject, 0.5f);
	}

}
