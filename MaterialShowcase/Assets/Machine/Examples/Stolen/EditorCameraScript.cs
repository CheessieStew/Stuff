using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class EditorCameraScript : MonoBehaviour
{
    //TODO - wypieprzyć to gdzieś indziej
    public float CameraSensitivity = 90;
    public float ScrollSensitivity = 1000;
    public float Speed = 50;
    public float ClimbSpeed = 20;

    public Vector3 BoundingBoxSize;
    public Vector3 BoundingBoxLocation;
    public Transform CamRigX;
    public Transform Cam;

    private Vector3 startingLocation;
    private Quaternion startingRotation;
    private float rotationX;
    private float rotationY;



    void Start()
    {
        Controls.EditorCamera = this;
        startingLocation = transform.position;
        startingRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Confined;;
        rotationX = transform.rotation.eulerAngles.y;
        rotationY = CamRigX.rotation.eulerAngles.x;
    }

    void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            startingLocation = transform.position;
            startingRotation = transform.rotation;
        }
        #endif
        Gizmos.color = Color.yellow;
        Matrix4x4 cubeTransform = Matrix4x4.TRS(startingLocation, startingRotation, transform.lossyScale);
        Gizmos.matrix = cubeTransform;
        Gizmos.DrawWireCube(Vector3.zero+BoundingBoxLocation, BoundingBoxSize);
    }

    void ClampPosition()
    {
        Vector3 relativeMovement = transform.position - BoundingBoxLocation - startingLocation;
        Vector3 xBound = startingRotation * new Vector3(BoundingBoxSize.x, 0, 0) /2;
        Vector3 yBound = startingRotation * new Vector3(0, BoundingBoxSize.y, 0) / 2;
        Vector3 zBound = startingRotation * new Vector3(0, 0, BoundingBoxSize.z) / 2;
        float xDiff = (Vector3.Dot(relativeMovement, xBound) / xBound.magnitude);
        float yDiff = (Vector3.Dot(relativeMovement, yBound) / yBound.magnitude);
        float zDiff = (Vector3.Dot(relativeMovement, zBound) / zBound.magnitude);
        Vector3 movement = Vector3.zero;
        if (Mathf.Abs(xDiff)>xBound.magnitude)
        {
            movement -= Mathf.Sign(xDiff) * xBound.normalized* (Mathf.Abs(xDiff) - xBound.magnitude);
        }
        if (Mathf.Abs(yDiff) > yBound.magnitude)
        {
            movement -= Mathf.Sign(yDiff) * yBound.normalized * (Mathf.Abs(yDiff) - yBound.magnitude);
        }
        if (Mathf.Abs(zDiff) > zBound.magnitude)
        {
            movement -= Mathf.Sign(zDiff) * zBound.normalized * (Mathf.Abs(zDiff) - zBound.magnitude);
        }
        transform.position += movement;
    }


    //TODO - maski
    void GrabSomething()
    {
        if (Controls.Draggable)
            return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
            return;

        if (!hit.collider || !hit.collider.transform)
            return;
        Transform obj = hit.collider.transform;

        DraggablePhysicsObject draggable = obj.GetComponent<DraggablePhysicsObject>();
        while (!draggable && obj.parent)
        {
            obj = obj.parent;
            draggable = obj.GetComponent<DraggablePhysicsObject>();
        }
        if (draggable)
        {
            draggable.BeginDragAction();
            return;
        }

        MachineBehaviour machine = obj.GetComponent<MachineBehaviour>();
        if (machine)
        {
            print("Starting machine: " + machine.name);
            machine.ProcessIngredients();
        }
    }

    void FixedUpdate()
    {
        transform.position += transform.rotation * Controls.CameraMovement * Time.fixedDeltaTime;
        ClampPosition();

        #if UNITY_EDITOR
        if (Controls.Pause)
        {
            UnityEditor.EditorApplication.isPaused = true;
        }
        #endif

        if (Controls.CameraRotate)
        {
            rotationX += Controls.CameraRotationX * CameraSensitivity * Time.deltaTime;
            rotationY -= Controls.CameraRotationY * CameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            CamRigX.localRotation = Quaternion.AngleAxis(rotationY, Vector3.right);
            if (Cam.position.y < startingLocation.y - BoundingBoxSize.y/2)
            {
                print("lol");
                rotationY+= Controls.CameraRotationY * CameraSensitivity * Time.deltaTime;
                CamRigX.localRotation = Quaternion.AngleAxis(rotationY, Vector3.right);
            }
        }
        Cam.localPosition += new Vector3(0, 0, Controls.CameraZoom * Time.deltaTime);

        if (Cam.localPosition.z > -1) Cam.localPosition = new Vector3(0, 0, -1);
    }

    void Update()
    {
        if (Controls.DraggableGrab)
        {
            GrabSomething();
        }
    }

}