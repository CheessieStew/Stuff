using UnityEngine;
using System.Collections.Generic;

public class PipeBehaviour : MonoBehaviour {

    public GameObject FirstBone;
    public GameObject LastBone;
    public float Speed = 2.5f;

    class BoneInfo
    {
        float _HowManyInside;
        public float HowManyInside
        {
            get
            {
                return _HowManyInside;
            }
            set
            {
                _HowManyInside = value;
                if (_HowManyInside >= 1)
                {
                    Debug.Log(bone.name + " is now full");
                    full = true;
                }
                if (_HowManyInside <= 0)
                {
                    Debug.Log(bone.name + " is not full anymore");
                    full = false;
                }
                if (bone)
                {
                    Vector3 newScale =  NormalScale * (HowManyInside + 1);
                    newScale.x = NormalScale.x;
                    bone.transform.localScale = newScale;
                }
            }
        }
        public Vector3 NormalScale;
        public GameObject bone;
        public bool full;
        public BoneInfo(GameObject b)
        {
            bone = b;
            NormalScale = b.transform.localScale;
            HowManyInside = 0;
            full = false;
        }
        
    }
    LinkedList<BoneInfo> Bones;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IngredientBehaviour>())
        {
            Bones.First.Value.HowManyInside++;
        }
    }


	// Use this for initialization
	void Start ()
    {
        Bones = new LinkedList<BoneInfo>();
        GameObject bone = FirstBone;
        Bones.AddLast(new BoneInfo(bone));
        while (bone != LastBone)
        {
            bone = bone.GetComponent<ConfigurableJoint>().connectedBody.gameObject;
            Bones.AddLast(new BoneInfo(bone));
        } 


    }

    // Update is called once per frame
    void Update()
    {
        for (LinkedListNode<BoneInfo> node = Bones.Last; node != null; node = node.Previous)
        {
            if (node.Value.full)
            {
                if (node.Next != null)
                    node.Next.Value.HowManyInside += (node.Value.HowManyInside + 1) * Time.deltaTime * Speed;
                node.Value.HowManyInside -= (node.Value.HowManyInside + 1) * Time.deltaTime * Speed;
            }
            if (node.Value.HowManyInside < 0)
                node.Value.HowManyInside = 0;
        }
        //print(msg);
    }
}

