using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{

	public bool buoy = false;
	public Rigidbody rb;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerStay(Collider col)
	{
		buoy = true;
		//foreach (ContactPoint cont in col.contacts)
		{
			//cont.otherCollider.attachedRigidbody.AddForceAtPosition (new Vector3(0,10,0), cont.point);
		}
		col.attachedRigidbody.AddForce (new Vector3 (0, 15, 0));
		//col.attachedRigidbody.
	}

	void OnTriggerEnter(Collider col)
	{
		buoy = true;
	}

	void OnTriggerExit(Collider col)
	{
		buoy = false;
	}
}