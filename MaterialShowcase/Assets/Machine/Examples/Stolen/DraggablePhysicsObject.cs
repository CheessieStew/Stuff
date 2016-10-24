using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class DraggablePhysicsObject : MonoBehaviour 
{
    public bool canDrag
    {
        get
        {
            return (Controls.Draggable == this);
        }
    }
    protected new Rigidbody rigidbody; //stary i tak jest deprecated

    Vector3 lastPosition;
    Vector3 velocity = Vector3.zero;
    float proportion = 0.7f;

    public virtual void Start()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();
        if (!rigidbody)
            throw new Exception(name + " requires a Rigidbody to be draggable");
        lastPosition = transform.position;
    }

    protected virtual void SetPhysics(bool gravity, bool kinematic)
    {
        if (!rigidbody)
            return;
        if (!gravity)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        rigidbody.useGravity = gravity;
        rigidbody.isKinematic = kinematic;

    }

    #region Drag handling
    public virtual void BeginDragAction()
    {
        SetPhysics(false, false);
        Controls.BeginDrag(this);
    }

    public virtual void DragAction()
    {
        rigidbody.AddForce(Controls.DraggableMovement * 2); //Vector3.zero;
        //rigidbody.AddForce(-Physics.gravity * rigidbody.mass);
        if (Controls.DraggableMovement.magnitude == 0f)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        //MoveTo(transform.position + Controls.DraggableMovement*Time.deltaTime);
    }

    public virtual void EndDragAction()
    {
        if (!canDrag) return;
        Controls.EndDrag(this);
        SetPhysics(gravity: true, kinematic: false);
        if (rigidbody)
        {
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = UnityEngine.Random.insideUnitSphere;
        }
    }

    protected virtual void MoveTo(Vector3 where)
    {
        transform.position = where;
    }

    //NOTE: obecnie zbędne
    //fajnie by było znieść wszystkie siły działające na ciągnięty obiekt
    // (by obrywał drugi, a nasz nie)
    // nie trzeba by w kółko ustawiać velocity = 0 w dragaction
    protected virtual void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white, 5f);
        }
    }

    #endregion

    #region Scroll handling
    private void ScrollAction()
    {
        Rotate(Controls.DraggableRotation * Time.fixedDeltaTime,Controls.DraggableRotationAxis);
    }

    protected virtual void Rotate(float angle, Vector3 axis)
    {
        Debug.DrawRay(transform.position, axis);
        transform.RotateAround(transform.position, axis, angle);
    }
    #endregion

    //NOTE: może warto poeksperymentować z wartością proportion
    void FixedUpdate()
    {
        if (canDrag)
        {
            DragAction();
            ScrollAction();
        }
        var curVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
        velocity = (1 - proportion) * velocity + proportion * curVelocity;
        if (Controls.DraggableThrow)
            EndDragAction();
    }
}
