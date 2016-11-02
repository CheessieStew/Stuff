using UnityEngine;
using System.Collections.Generic;

public class HandBehaviour : MonoBehaviour {
    static readonly float AverageWeight = 0.1f;

    [Tooltip ("Velocity multiplier when throwing - unrealistic but fun")]
    public float ThrowStrength = 1.2f;

    RigidbodyConstraints OldConstraints;
    List<Rigidbody> ItemsInRange;
    Rigidbody HeldItem;
    bool Closed;
    Vector3 AvVelocity;
    Vector3 LastPosition;


	void Start () {
        ItemsInRange = new List<Rigidbody>();
	}
	

    //do odpalenia po puszczeniu przycisku na kontrolerze, który jest parentem ręki
    public void Throw()
    {
        if (!Closed)
            return;
        print("throw");
        if (HeldItem != null)
        {
            HeldItem.useGravity = true;
            HeldItem.transform.parent = null; //może chcemy ładniej, czy coś
            HeldItem.constraints = OldConstraints;
            HeldItem.velocity = AvVelocity * ThrowStrength;
            HeldItem = null;
        }
        Open();
    }

    void Open()
    {
        GetComponent<Renderer>().material.color = Color.white;
        Closed = false;
    }

    //do odpalenia po naciśnięciu przycisku na kontrolerze, który jest parentem ręki
    public void Grab() 
    {
        if (Closed)
            return;
        print("grab");
        if (HeldItem == null && ItemsInRange.Count > 0)
        {
            // dodatkowe ograniczenia co do tego, co możemy podnieść i tak
            // dalej umieścić w predykacie finda lub przydodawaniu do listy 
            // to, czy jest kinematic sprawdzam tutaj, a nie przy dodawaniu do listy
            // bo w pewnych przypadkach na trigger może wejść kinematic obiekt,
            // który dopiero później stanie się non-kinematic
            HeldItem = ItemsInRange.Find(body => !body.isKinematic);
            HeldItem = ItemsInRange[0];
            HeldItem.useGravity = false;
            OldConstraints = HeldItem.constraints;
            HeldItem.constraints = RigidbodyConstraints.FreezeAll;
            HeldItem.transform.parent = transform;

            // jeśli chcemy, by przedmiot leżał ładnie w ręce 
            // (musi mieć dobrze dobrany pivot i domyślną rotację):
            //HeldItem.transform.localPosition = Vector3.zero;
            //HeldItem.transform.localRotation = Quaternion.identity;
        }
        Close();
    }

    void Close()
    {
        GetComponent<Renderer>().material.color = Color.red;
        Closed = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
            ItemsInRange.Add(other.GetComponent<Rigidbody>());
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            if (HeldItem == body)
            {
                Throw();
                Close();
            }
            ItemsInRange.Remove(body);
        }
    }

    void FixedUpdate()
    {

    }

	// Update is called once per frame
	void Update () {
        Vector3 distance = (transform.position - LastPosition);
        AvVelocity = AverageWeight * AvVelocity + (1 - AverageWeight) * distance / Time.fixedDeltaTime;
        LastPosition = transform.position;
    }
}
