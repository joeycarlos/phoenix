using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMaker : MonoBehaviour {

    LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLine();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetupLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(0,0,0));
        lineRenderer.SetPosition(1, new Vector3(3,3,3));
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.useWorldSpace = true;
    }
}
