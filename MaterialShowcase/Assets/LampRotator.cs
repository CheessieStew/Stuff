using UnityEngine;
using System.Collections;

public class LampRotator : MonoBehaviour {
    float cur;
    public float maxangle;
	// Use this for initialization
	void Start () {
        cur = 0;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, transform.up, maxangle*( Mathf.Sin(cur + Time.deltaTime) - Mathf.Sin(cur)));
        cur += Time.deltaTime;
        if (cur >= 2 * Mathf.PI)
            cur -= 2 * Mathf.PI;
	}
}
