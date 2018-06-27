using UnityEngine;

public class PerlinWaves : MonoBehaviour
{
	public float perlinScale = 0.5f;
	public float waveHeight = 2.5f;
	public float waveSpeed = 0.25f;
    public float theScriptTime;

	void Appledate ()
	{
		//Get copy of mesh filter
		MeshFilter meshFil = GetComponent<MeshFilter> ();


		//Get a copy of the vertex list
		Vector3[] vertList = meshFil.mesh.vertices;

		//Loop through and calculate perlin values
		for (int i=0; i< vertList.Length; i++)
		{
			//Calculate a Perlin reference value from it's position and the scale of perlin noise
			//Then add an offset for 
			float pX = (vertList[i].x * perlinScale) + (waveSpeed * Time.timeSinceLevelLoad);
			float pZ = (vertList[i].z * perlinScale) + (waveSpeed * Time.timeSinceLevelLoad);
			vertList[i].y = Mathf.PerlinNoise (pX, pZ) * waveHeight;
		}

		meshFil.mesh.vertices = vertList;
		meshFil.mesh.RecalculateNormals ();
		meshFil.mesh.RecalculateBounds ();
	}


    void Update()
    {
        theScriptTime = Time.time;
        GetComponent<Renderer>().material.SetFloat("_scriptTime", theScriptTime);
        //Get copy of mesh filter
        MeshFilter meshFil = GetComponent<MeshFilter>();


        //Get a copy of the vertex list
        Vector3[] vertList = meshFil.mesh.vertices;

        //Loop through and calculate perlin values
        for (int i = 0; i < vertList.Length; i++)
        {
            //Calculate a Perlin reference value from it's pos and the scale of pnoise
            //Then add an offset for 
            float pX = (vertList[i].x * perlinScale) + (waveSpeed * theScriptTime);
            float pZ = (vertList[i].z * perlinScale) + (waveSpeed * theScriptTime);
            vertList[i].y = Mathf.PerlinNoise(pX, pZ) * waveHeight;
        }

        meshFil.mesh.vertices = vertList;
        meshFil.mesh.RecalculateNormals();
        meshFil.mesh.RecalculateBounds();
    }
}