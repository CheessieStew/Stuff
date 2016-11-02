using UnityEngine;
using System.Collections;

public class PlaceholderController : MonoBehaviour {

    public float speed = 25;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float fwd = Input.GetAxis("Vertical");
        float sdws = Input.GetAxis("Horizontal");
        float up = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftControl) ? 1 : 0);
        transform.position += new Vector3(sdws, up/2, fwd) * speed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1);
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponentInChildren<HandBehaviour>().Grab();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            GetComponentInChildren<HandBehaviour>().Throw();
        }
	}
}
