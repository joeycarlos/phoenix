using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLookAt : MonoBehaviour {

    [SerializeField] GameObject target;

    // Use this for initialization
    void Start () {
        target = GameObject.FindGameObjectWithTag("Player"); ;
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target.transform);
    }
}
