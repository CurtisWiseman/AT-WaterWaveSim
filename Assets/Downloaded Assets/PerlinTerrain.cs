using UnityEngine;
using System.Collections;

public class PerlinTerrain : MonoBehaviour
{
    public float perlinScale;
    public float waveSpeed;
    public float waveHeight;
    public float offset;

    void Update ()
	{
        CalcNoise();
    }

    void CalcNoise()
	{
        MeshFilter mF = GetComponent<MeshFilter>();
        MeshCollider mC = GetComponent<MeshCollider>();
        
        mC.sharedMesh = mF.mesh;

        Vector3[] verts = mF.mesh.vertices; 

        for (int i=0; i< verts.Length; i++)
		{
            float pX = (verts[i].x * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed) + offset;
            float pZ = (verts[i].z * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed) + offset;
            verts[i].y = Mathf.PerlinNoise(pX, pZ) * waveHeight;
        }

        mF.mesh.vertices = verts;
        mF.mesh.RecalculateNormals();
        mF.mesh.RecalculateBounds();
    }
}