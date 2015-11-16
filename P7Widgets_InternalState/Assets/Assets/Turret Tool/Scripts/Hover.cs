using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public GameObject playerObj; 
    public Rigidbody playerRigidbody;
    bool inTrigger = false;


    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = playerObj.GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(inTrigger == true)
        {
            makeHover();
        }
 

    }

    void OnTriggerEnter(Collider col)
    {
        inTrigger = true;
    }

    void OnTriggerExit(Collider coll)
    {
        inTrigger = false;
    }
    void makeHover()
    {

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
       // Vector3 dir = new Vector3(playerObj.transform.localRotation.x, playerObj.transform.localRotation.y, playerObj.transform.localRotation.z);
        Vector3 dir = transform.TransformDirection(Vector3.up);

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = dir * proportionalHeight * hoverForce;
            playerRigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }


    }

}

