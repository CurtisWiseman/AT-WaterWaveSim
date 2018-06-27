using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shaderTime : MonoBehaviour
{

    public Material PMat;
    public float theScriptTime;
    public ClassicNoiseCPU nScriptSync;
    public calculatedFloatingObject objSync;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        theScriptTime = Time.time;
        nScriptSync.theScriptTime = theScriptTime;
        objSync.theScriptTime = theScriptTime;
        //PMat.SetFloat("_scriptTime", theScriptTime);
        GetComponent<Renderer>().material.SetFloat("_scriptTime", theScriptTime);
    }
}