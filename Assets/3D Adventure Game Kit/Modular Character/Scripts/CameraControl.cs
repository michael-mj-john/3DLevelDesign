using UnityEngine;
using System.Collections;
/// <summary>
/// Camera control class handles camera movement and camera collisions, 
/// a large portion of the code for the collision detection is an altered version of http://renaissancecoders.com/ Unity3D Character Controller tutorial, 
/// if you want a stronger understanding of the code behind the collision detection follow that tutorial.
/// </summary>
public class CameraControl : MonoBehaviour {

    /// <summary>
    /// This handles all positional variables from zooming in and out to position offsets.
    /// </summary>
    [System.Serializable]
    public class PositionSettings
    {
        public Vector3 targetPosOffest = new Vector3(0, 3.4f, 0);   //Offset from the target.
        public float lookSmooth = 100f;
        public float distanceFromTarget = -8;   //Distance from the target.
        public float zoomSpeed = 10;           //Speed of the zoom in and out.
        public float maxZoom = -2;              //Closest the camera can zoom in.
        public float minZoom = -15;             //Furthest out the camera can zoom out.
        public float hideThreshold = 1;         //What distance the target model is hidden at.     
        public bool smoothFollow;               //Smooths out the motion of the camera.
        public float smooth;                    //The value the camera is smoothed out.

        [HideInInspector]
        public float adjustedDistance;  //Stored value of the current adjusted Distance from the target.
    }

    /// <summary>
    /// This handles the variables for rotating the camera around the target.
    /// </summary>
    [System.Serializable]
    public class OrbitSettings
    {
        public float vRotation = -20;       //Current Up Down Rotation.
        public float hRotation = -180;      //Current Left Right Rotation.
        public float maxvRotation = 25;     //Maximum Up Down Rotation.
        public float minvRotation = -85;    //Maximum Up Down Rotation.
        public float vOrbitSpeed = 150;     //Verical Orbitting Speed.
        public float hOrbitSpeed = 150;     //Horizontal Orbitting Speed.
        public float resetSmooth = 10;      //Resetting Speed.

    }
    /// <summary>
    /// This handles variables for debuging out clip points.
    /// </summary>
    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;

    }

    //References to this classes above.
    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public DebugSettings debug = new DebugSettings();
    public CollisionHandler collision = new CollisionHandler();
    //Reference to the target input.
    public InputManager input;

    public Transform target;    //current target for the camera.
    Vector3 targetPos = Vector3.zero;   //The targets position.
    Vector3 camVel = Vector3.zero;      //The camera's velocity.
    Vector3 adjustedDestination = Vector3.zero; //The adjusted destination of the camera after colliding.
    Vector3 destination = Vector3.zero;     //The current destination of the camera.
    bool isResetting = false;       //Bool to reset the camera behind the target.

    /// <summary>
    /// Called in start finds the expected target and references it's input manager.
    /// </summary>
    void FindTarget()
    {
        if (!target)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (!input)
        {
            input = target.GetComponent<InputManager>();
        }
    }
    /// <summary>
    /// Finds the target, Initializes the collision handler of the camera and sets up the clip points and moves the camera to the target.
    /// </summary>
    void Start()
    {
        FindTarget();
        collision.Initialize(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
        MoveToTarget();
    }
    /// <summary>
    /// Update handles the zoom functionality of the camera.
    /// </summary>
    void Update()
    {
        ZoomInOnTarget();
    }

    /// <summary>
    /// Fixed update orbiting and rotation of the camera, also updates all clip points and attempts to debug draw them.
    /// </summary>
    void FixedUpdate()
    {
        //Logic for main camera use.
        MoveToTarget();
        LookAtTarget();
        OrbitTarget();
        
        //Update camera clip points.
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

        //Debug Collision

        for(int i = 0; i < 5; i++)
        {
            if (debug.drawDesiredCollisionLines)
            {
                Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
            }

            if (debug.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
            }

        }
        //Check if the camera is colliding.
        collision.CheckColliding(targetPos);

        //Hide the model if it is too close to the camera.
        HideModel();

    }
    /// <summary>
    /// Moves the camera towards the target.
    /// </summary>
    void MoveToTarget()
    {
        //Checks the offset
        targetPos = target.position + position.targetPosOffest;

        //Calculates the destination using orbit and distance variables.
        destination = Quaternion.Euler(orbit.vRotation, orbit.hRotation, 0) * -Vector3.forward * position.distanceFromTarget;
        destination += targetPos;
        //If the camera is colliding with anything
        if (collision.colliding)
        {
            //Calculate destination using adjusted values.
            adjustedDestination = Quaternion.Euler(orbit.vRotation, orbit.hRotation, 0) * Vector3.forward * collision.GetAdjustedDistanceWithRayFrom(targetPos);
            adjustedDestination += targetPos;

            if (position.smoothFollow)
            {
                //Smooth Damp the following.
                transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camVel, position.smooth);
            }
            else
            {
                //Position uses adjustedDestination.
                transform.position = adjustedDestination;
            }
         }
        else
        {
            if (position.smoothFollow)
            {
                //Smooth Damp the following.
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, position.smooth);
            }
            else
            {
                //Position used destination.
                transform.position = destination;
            }
        }

    }

    /// <summary>
    /// Rotates the camera to look towards the target.
    /// </summary>
    void LookAtTarget()
    {
        if (targetPos - transform.position != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
        }
    }

    /// <summary>
    /// Orbit control for the camera resets the camera behind the player if desired and moves the orbit values based on input.
    /// </summary>
    void OrbitTarget()
    {
        //Reset the camera if bool is set or button is pressed.
        if (input.current.cameraReset > 0 || isResetting)
        {
            if (target.GetComponent<CharacterMotor>().movement.movementDirection == Vector3.zero)
            {
                isResetting = true;
                //Value is behind the target.
                orbit.hRotation = Mathf.Lerp(orbit.hRotation, target.eulerAngles.y - 180, orbit.resetSmooth * Time.deltaTime);
                float value = orbit.hRotation - (target.eulerAngles.y - 180);
                if (Mathf.Abs(value) < 1)
                {
                    isResetting = false;
                }

            }
            else
            {
                isResetting = false;
            }
        }
        //Change the orbit values based on input and speed values
        orbit.vRotation += input.current.cameraV * orbit.vOrbitSpeed * Time.deltaTime;
        orbit.hRotation += input.current.cameraH * orbit.hOrbitSpeed * Time.deltaTime;

        //Resets the horizontal rotation on a full orbit to keep values within a 360 degree value.
        if(orbit.hRotation > 180)
        {
            orbit.hRotation -= 360;
        }
        else if (orbit.hRotation < -180)
        {
            orbit.hRotation += 360;
        }
        
        //Cap the vertical rotation based on max and min values.
        if(orbit.vRotation > orbit.maxvRotation)
        {
            orbit.vRotation = orbit.maxvRotation;
        }
        if (orbit.vRotation < orbit.minvRotation)
        {
            orbit.vRotation = orbit.minvRotation;
        }
    }

    /// <summary>
    /// Zoom Logic based on input type.
    /// </summary>
    void ZoomInOnTarget()
    {
        //Change how the camera zooms based on input type
        if (input.current.type == InputManager.ControlType.Keyboard)
        {
            position.distanceFromTarget += input.current.scrollInput * position.zoomSpeed * Time.deltaTime;
        }
        else
        {
            position.distanceFromTarget += input.current.cameraV * position.zoomSpeed / 200 * Time.deltaTime;
        }

        //Cap the zoom using max and min values.
        if (position.distanceFromTarget > position.maxZoom)
        {
            position.distanceFromTarget = position.maxZoom;
        }

        if (position.distanceFromTarget < position.minZoom)
        {
            position.distanceFromTarget = position.minZoom;
        }

    }

    /// <summary>
    /// If the camera is too close to the target the target disappears giving a first person view and not allowing the user to see the inside of the model.
    /// </summary>
    void HideModel()
    {
        if (target.gameObject.GetComponentInChildren<SkinnedMeshRenderer>())
        {

            if (Vector3.Distance(targetPos, transform.position) < position.hideThreshold)
            {
                target.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
            else
            {
                target.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            }
        }
        
    }


    [System.Serializable]
    public class CollisionHandler
    {
        public LayerMask collisionLayer;    //Layer mask for collision targets for the camera.
        public bool colliding = false;      //Is the camera colliding.
        public Vector3[] adjustedCameraClipPoints;  //Array of adjusted camera clip points.
        public Vector3[] desiredCameraClipPoints;   //Array of desired camera clip points.

        Camera camera;
        
        /// <summary>
        /// Setup the camera's collision handler setting the camera variable and filling the ClipPoint Arrays.
        /// </summary>
        /// <param name="cam"></param>
        public void Initialize(Camera cam)
        {
            camera = cam;
            adjustedCameraClipPoints = new Vector3[5];
            desiredCameraClipPoints = new Vector3[5];
        }

        /// <summary>
        /// Updates the Camera's clip points based on the camera's transform.
        /// </summary>
        /// <param name="cameraPosition"></param>
        /// <param name="atRotation"></param>
        /// <param name="intoArray"></param>
        public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            if (!camera)
                return;
            //Clear the contents of intoArray

            intoArray = new Vector3[5];
            float z = camera.nearClipPlane;
            //Calculate the collision space.
            float x = Mathf.Tan(camera.fieldOfView / 4) * z;
            float y = x / camera.aspect;

            //top left
            intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; //Added and rotated the point relative to the camera.
            //top right
            intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
            //bottom left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
            //bottom right
            intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
            //camera's position
            intoArray[4] = cameraPosition - camera.transform.forward;
        }

        /// <summary>
        /// Checks for collision using the ClipPoints
        /// </summary>
        /// <param name="clipPoints"></param>
        /// <param name="fromPosition"></param>
        /// <returns></returns>
        bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition,clipPoints[i] - fromPosition);
                float distance = Vector3.Distance(clipPoints[i], fromPosition);
                if(Physics.Raycast(ray, distance, collisionLayer))
                {
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Get a new distance based on camera collision
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public float GetAdjustedDistanceWithRayFrom(Vector3 from)
        {

            float distance = -1;

            for(int i =0; i < desiredCameraClipPoints.Length; i++)
            {

                Ray ray = new Ray(from,desiredCameraClipPoints[i] - from);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if (distance == -1)
                        distance = hit.distance;
                    else
                    {

                        if (hit.distance < distance)
                        {
                            distance = hit.distance;
                            distance = Mathf.Round(distance);
                        }

                    }
                }

            }

            if (distance == -1)
                return 0;
            else
                return distance;
        }

        /// <summary>
        /// Check if the camera is colliding
        /// </summary>
        /// <param name="targetPosition"></param>
        public void CheckColliding(Vector3 targetPosition)
        {

            if(CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
            {
                colliding = true;
            }
            else
            {
                colliding = false;
            }

        }

    }

}
