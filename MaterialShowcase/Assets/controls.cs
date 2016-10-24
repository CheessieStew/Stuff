using UnityEngine;
using System.Collections;
using System;

public class controls : MonoBehaviour {
    CharacterController controller;
    public float speed;
    public float rotateSpeed;
    public Camera cam;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        if (!controller || !cam)
            throw new Exception("lel");
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update () {
        float fwd = Input.GetAxis("Vertical");
        float sdws = Input.GetAxis("Horizontal");
        float xrot = Input.GetAxis("Mouse X");
        float yrot = Input.GetAxis("Mouse Y");
        controller.SimpleMove((transform.forward*fwd + transform.right * sdws)* speed *  Time.deltaTime);
        transform.RotateAround(transform.position, transform.up, rotateSpeed * xrot * Time.deltaTime);
        cam.transform.RotateAround(cam.transform.position,cam.transform.right, - rotateSpeed * yrot * Time.deltaTime);
    }
}
