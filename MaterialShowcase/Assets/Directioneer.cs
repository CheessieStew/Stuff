using UnityEngine;
using System.Collections;

public class Directioneer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.blue);
        Debug.DrawRay(transform.position, transform.right * 10, Color.red);
        Debug.DrawRay(transform.position, transform.up * 10, Color.green);
	}
}
