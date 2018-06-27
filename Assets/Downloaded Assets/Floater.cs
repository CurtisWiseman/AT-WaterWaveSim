using UnityEngine;
using System.Collections;

public class Floater : MonoBehaviour
{
	public float waterLevel, floatHeight;
	public Vector3 buoyancyCentreOffset;
	public float bounceDamp;
	public Rigidbody rb;
	private RaycastHit rayHit;

	void FixedUpdate ()
	{
		if (Physics.Raycast (this.transform.position, Vector3.up, out rayHit))
		{
			if (rayHit.collider.gameObject.CompareTag ("Water"))
			{
				waterLevel = rayHit.barycentricCoordinate.y;
			}
		}
		else if (Physics.Raycast (this.transform.position, Vector3.down, out rayHit))
		{
			if (rayHit.collider.gameObject.CompareTag ("Water"))
			{
				waterLevel = rayHit.barycentricCoordinate.y;
			}
		}

		Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
		float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
		
		if (forceFactor > 0f)
		{
			Vector3 uplift = -Physics.gravity * (forceFactor - rb.velocity.y * bounceDamp);
			rb.AddForceAtPosition(uplift, actionPoint);
		}
	}
}