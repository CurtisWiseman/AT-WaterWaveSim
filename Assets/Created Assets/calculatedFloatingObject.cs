using UnityEngine;

public class calculatedFloatingObject : MonoBehaviour
{
    public float waterLevel, floatDepth, waterForceDamp;
    public Vector3 centreOfBuoancy;
    public Rigidbody rb;
    private RaycastHit rayHit;
    public bool useMultiPoints = false;
    private Vector3[] actionPoints;
    private float waterForce;
    public float theScriptTime;

    private void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(useMultiPoints)
        {
            actionPoints = GetComponent<MeshFilter>().sharedMesh.vertices;

            for (int i = 0; i < actionPoints.Length; i++)
            {
                actionPoints[i] += transform.position + transform.TransformDirection(centreOfBuoancy);

                waterLevel = GetWaterLevelFromActionPoint(actionPoints[i]);
                
                waterForce = 1f - ((actionPoints[i].y - waterLevel) / floatDepth);
                if (waterForce > 0f)
                {
                    Vector3 uplift = -Physics.gravity * (waterForce - (rb.velocity.y * waterForceDamp)) / actionPoints.Length;
                    rb.AddForceAtPosition(uplift, actionPoints[i]);
                }
            }
        }
        else
        {
            waterLevel = GetWaterLevelFromActionPoint(transform.position);

            //Point of object to apply force to
            Vector3 actionPoint = transform.position + transform.TransformDirection(centreOfBuoancy);

            //Amount of upward force to apply based on submerged depth. Greater floatDepth = sink further
            waterForce = 1f - ((actionPoint.y - waterLevel) / floatDepth);
            
            //If force is to be applied
            if (waterForce > 0f)
            {
                //Lift up by inverting gravity and multiplying by waterForce minus the product of current upward velocity and water force dampening
                Vector3 uplift = -Physics.gravity * (waterForce - (rb.velocity.y * waterForceDamp));
                rb.AddForceAtPosition(uplift, actionPoint);
            }
        }
    }

    float GetWaterLevelFromActionPoint(Vector3 point)
    {
        bool foundWaterAbove = false;
        float workingWaterLevel = -100000000f;
        //Raycast vertically to find nearest surface
        if (Physics.Raycast(point - new Vector3(0, 10, 0), Vector3.up, out rayHit))
        {
            //If surface is water, set waterLevel to its height 
            if (rayHit.collider.gameObject.CompareTag("Water"))
            {
                workingWaterLevel = GetWaterLevelAtPoint(rayHit.collider, rayHit.point);
                foundWaterAbove = true;
            }
        }
        if (Physics.Raycast(point + new Vector3(0, 10, 0), Vector3.down, out rayHit) && !foundWaterAbove)
        {
            if (rayHit.collider.gameObject.CompareTag("Water"))
            {
                workingWaterLevel = GetWaterLevelAtPoint(rayHit.collider, rayHit.point);
            }
        }

        return workingWaterLevel;
    }

    float GetWaterLevelAtPoint(Collider col, Vector3 point)
    {
        Vector3 pObjSpace = point - col.gameObject.transform.position;
        bool use_cpu_waves = false;
        bool use_old_ways = false;

        
        if (use_cpu_waves)
        {
            PerlinWaves waveScript = col.gameObject.GetComponent<PerlinWaves>();
            pObjSpace.x = (pObjSpace.x * waveScript.perlinScale) + (waveScript.waveSpeed * Time.timeSinceLevelLoad);
            pObjSpace.z = (pObjSpace.z * waveScript.perlinScale) + (waveScript.waveSpeed * Time.timeSinceLevelLoad);
            return Mathf.PerlinNoise(pObjSpace.x, pObjSpace.z) * waveScript.waveHeight + col.gameObject.transform.position.y;
        }
        else
        {
            Material col_mat = col.gameObject.GetComponent<Renderer>().material;

            float length = col_mat.GetFloat("_Length");
            float height = col_mat.GetFloat("_Height");
            float speed = col_mat.GetFloat("_Speed");

            if (use_old_ways)
            {
                pObjSpace.x = (pObjSpace.x * length) + (speed * Time.timeSinceLevelLoad);
                pObjSpace.z = (pObjSpace.z * length) + (speed * Time.timeSinceLevelLoad);

                return Mathf.PerlinNoise(pObjSpace.x, pObjSpace.z) * height + col.gameObject.transform.position.y;
            }
            else
            {
                Vector2 uv = new Vector2(pObjSpace.x, pObjSpace.z) * 4.0f + new Vector2(0.2f, 1.0f) * theScriptTime;

                float y_val = 0.5f;

                Vector3 coord = new Vector3(uv.x * length, uv.y * length, theScriptTime * speed);
                y_val += ClassicNoiseCPU.cnoise(coord) * height;
                y_val += col.gameObject.transform.position.y;

                return y_val;
            }
        }
    }
}