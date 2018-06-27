using UnityEngine;

public class floatingObject : MonoBehaviour
{
	public float waterLevel, floatDepth, waterForceDamp;
	public Vector3 centreOfBuoancy;
	public Rigidbody rb;
	private RaycastHit rayHit;

	void FixedUpdate ()
	{
		bool foundWaterAbove = false;
		//Raycast vertically to find nearest surface
		if (Physics.Raycast (this.transform.position, Vector3.up, out rayHit))
		{
			//If surface is water, set waterLevel to its height 
			if (rayHit.collider.gameObject.CompareTag ("Water"))
			{
				waterLevel = rayHit.point.y;
				foundWaterAbove = true;
			}
			//Else set waterLevel to arbitrary large negative value
			else
			{
				waterLevel = -100000000f;
			}
		}

		if (Physics.Raycast (this.transform.position, Vector3.down, out rayHit) && !foundWaterAbove)
		{
			if (rayHit.collider.gameObject.CompareTag ("Water"))
			{
				waterLevel = rayHit.point.y;
			}
			else
			{
				waterLevel = -100000000f;
			}
		}

		//Point of object to apply force to
		Vector3 actionPoint = transform.position + transform.TransformDirection(centreOfBuoancy);

		//Amount of upward force to apply based on submerged depth. Greater floatDepth = sink further
		float waterForce = 1f - ((actionPoint.y - waterLevel) / floatDepth);

		//If force is to be applied
		if (waterForce > 0f)
		{
			//Lift up by inverting gravity and multiplying by waterForce minus the product of current upward velocity and water force dampening
			Vector3 uplift = -Physics.gravity * (waterForce - rb.velocity.y * waterForceDamp);
			rb.AddForceAtPosition(uplift, actionPoint);
		}
	}
}