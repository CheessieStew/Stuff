using UnityEngine;
using System.Collections;

public class PlaceholderButton : MonoBehaviour {

    bool Pushed;
    float DistToGo;
    float DistMoved;

	// Use this for initialization
	void Start () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        print("collision");
        if (!Pushed)
            DistToGo = -0.5f;
       
    }

    // Update is called once per frame
    void Update () {
        transform.position += new Vector3(0, DistToGo * Time.deltaTime, 0);
        DistMoved += DistToGo * Time.deltaTime;
        if (Mathf.Abs(DistMoved) >= Mathf.Abs(DistToGo) && DistToGo != 0)
        {
            DistMoved = 0;
            if (!Pushed)
            {
                print("push finished");
                Pushed = true;
                DistToGo = 0.5f;
            }
            else
            {
                print("unpush finished");
                Pushed = false;
                DistToGo = 0f;
            }
        }
	}
}
