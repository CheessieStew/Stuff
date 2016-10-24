using UnityEngine;
using System.Collections;
using System;

public static class Controls
{
    #region EditorCameraControls

    private static EditorCameraScript _camera;
    public static EditorCameraScript EditorCamera
    {
        get
        {
            return _camera;
        }
        set
        {
            if (!_camera)
                _camera = value;
            else throw new Exception("There can be only one!");
        }
    }

    public static bool CameraRotate
    {
        get
        {
            return Input.GetKey(KeyCode.Mouse1);
        }
    }

    private static bool _cameraOnline;
    private static bool CameraOnline
    {
        get
        {
            if (_cameraOnline && Draggable)
            {
                _cameraOnline = false;
            }
            else if (!Draggable
                && Input.GetAxis("Horizontal")==0f 
                && Input.GetAxis("Vertical") ==0f)
            {
                _cameraOnline = true;
            }
            return (Draggable == null) && _cameraOnline;
            
        }
    }

    public static float CameraRotationX
    {
        get
        {
            if (CameraOnline)
                return Input.GetAxis("Mouse X");
            else return 0;
        }
    }

    public static float CameraRotationY
    {
        get
        {
            if (CameraOnline)
                return Input.GetAxis("Mouse Y");
            else return 0;
        }
    }

    public static Vector3 CameraMovement
    {
        get
        {
            if (CameraOnline)
            { 

                Vector3 movement = 
                    new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                movement *= EditorCamera.Speed;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    movement += new Vector3(0, -1, 0) * EditorCamera.ClimbSpeed;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    movement += new Vector3(0, 1, 0) * EditorCamera.ClimbSpeed;
                }
                return movement;
            }
            else return Vector3.zero;
        }
    }

    public static float CameraZoom
    {
        get
        {
            if (CameraOnline)
                return Input.GetAxis("Mouse ScrollWheel")*EditorCamera.ScrollSensitivity;
            else return 0f;
        }
    }
    #endregion

    #region DraggableControls

    //NOTE: Te stałe powinny zależeć od edytora
    //Podobnie stałe dotyczące kamery, które obecnie posiada ona sama
    public static float DraggableMovementSpeed = 7;
    public static float DraggableDragSpeed = 50;
    public static float DraggableMaxSpeed = 25;
    public static DraggablePhysicsObject Draggable{ get; private set; }

    public static void BeginDrag(DraggablePhysicsObject who)
    {
        if (!Draggable)
        {
            Draggable = who;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else throw new Exception("There can be only one!");
    }

    //TODO/NOTE:
    //chicelibyśmy cos jak Cursor.setposition(worldtoscreen(draggable.position))
    //nie da się - ale i tak będziemy potrzebowali własnego kursora
    //na razie niestety kursor zawsze ustawia się na środku po zakończeniu draga
    public static void EndDrag(DraggablePhysicsObject who)
    {
        if (Draggable == who)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Draggable = null;
        }
        else throw new Exception("EndDrag illegal call");
    }

    public static bool ComponentMoveOut
    {
        get
        {
            return (Input.GetKey(KeyCode.LeftAlt));
        }
    }

    public static Vector3 DraggableMovement
    {
        get
        {
            if (EditorCamera && Draggable)
            {
                Vector3 camRot = EditorCamera.transform.rotation.eulerAngles;
                Vector3 wsad = Quaternion.Euler(0, camRot.y, 0)
                    * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))
                    * DraggableMovementSpeed;
                Vector3 drag = EditorCamera.transform.rotation
                    * new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0)
                    * (Draggable.transform.position - EditorCamera.Cam.transform.position).magnitude / 50
                    * DraggableDragSpeed;

                Vector3 sum = wsad + drag;

                Vector3 pos = Camera.main.WorldToScreenPoint(Draggable.transform.position);

                float xpos = (pos.x - Camera.main.pixelWidth / 2);
                int xProblem = 0;
                if (Mathf.Abs(xpos)> Camera.main.pixelWidth/2.1)
                    xProblem = (int)Mathf.Sign(xpos);

                float ypos = (pos.y - Camera.main.pixelHeight / 2);
                int yProblem = 0;
                if (Mathf.Abs(ypos) > Camera.main.pixelHeight / 2.1)
                    yProblem = (int)Mathf.Sign(ypos); 

                float sumOnRight = Vector3.Dot(EditorCamera.Cam.right, sum);
                float sumOnUp = Vector3.Dot(EditorCamera.Cam.up, sum);
                float sumOnForward = Vector3.Dot(EditorCamera.Cam.forward, sum);

                if (Mathf.Sign(sumOnRight) == xProblem)
                {
                    sum -= EditorCamera.Cam.right * sumOnRight;
                }
                if (Mathf.Sign(sumOnUp) == yProblem)
                    sum -= EditorCamera.Cam.up * sumOnUp;

                if (pos.z < 1.5 && sumOnForward < 0)
                    sum -= EditorCamera.Cam.forward * sumOnForward;
               
                if (sum.magnitude>DraggableMaxSpeed)
                {
                    sum *= DraggableMaxSpeed / sum.magnitude;
                }
                //TODO - brzydko? za dużo operacji?

                return sum;

            }
            else throw new Exception("lol");

        }
    }

    private static float _draggableWholeRotation;
    private static float _draggableRotation;
    public static float DraggableRotation
    {
        get
        {
            var AxisSum = Input.GetAxis("Mouse ScrollWheel")/10;
            if (AxisSum == 0f)
            {
                if (Mathf.Abs(_draggableRotation) > 0.05f * Mathf.Abs(_draggableWholeRotation))
                    _draggableRotation -= 0.05f * _draggableWholeRotation;
                else _draggableRotation = 0;
            }
            else _draggableWholeRotation = _draggableRotation 
                  = AxisSum * 2500; //TODO: razy jakaś stała z edytora
            return _draggableRotation;

        }
    }

    public static Vector3 DraggableRotationAxis
    {
        get
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                return EditorCamera.transform.right;
            }
            return EditorCamera.transform.up;
        }
    }

    public static bool DraggableGrab
    {
        get
        {
            return (Input.GetKeyDown(KeyCode.Mouse0));
        }
    }

    public static bool DraggableThrow
    {
        get
        {
            return (!Input.GetKey(KeyCode.Mouse0));
        }
    }

    public static int Ghost90
    {
        get
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                return 90;
            return 0;
        }
    }

    #endregion

    #region misc
    public static bool Pause
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
    }
    #endregion

}
