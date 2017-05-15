using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]


/// <summary>
/// This class handles variables based on gravity and force.
/// </summary>
[System.Serializable]
public class GeneralSettings
{
    public float gravityForce = 10f;                                            //Strength of gravity.
    public Vector3 gravityDirection = Vector3.down;                             //Change the direction gravity will drag the player.
    public bool gravityOverrideOnSurface = true;                                //Changes the gravity to pull towards the surface which the character is stood on, this stops the character controller sliding down smaller slopes.
    public string movingPlatformTag = "Moving Platform";
    public bool onMovingPlatform = false;
    [HideInInspector] public Vector3 gravityCurrentDirection = Vector3.down;    //Debug variable showing the current direction gravity is affecting.
    [HideInInspector] public bool forceInfluence = false;                       //This variable is used to stop multiple forces being affecting the character at the same time, fixing certain bugs which can occur.
}


/// <summary>
/// This class handles all movement based variables.
/// </summary>
[System.Serializable]
public class MovementSettings
{
    public float movementSpeed = 8f;                                    //The speed the character moves around.
    public Vector3 movementDirection = Vector3.zero;                    //This variable is used to input the direction the character will move.
    [Range(0, 1)] public float groundedVelocityInfluence = 1f;          //Velocity influence changes the strength of the force and how quickly it will affect the current movement.
    [Range(0, 1)] public float airVelocityInfluence = 0.5f;             //Grounded and Air allows for different movement influence depending on the state of the character.
    public bool sliding = false;                                        //Variable is used to define if the character is currently sliding.
    public float slideLimit = 35f;                                      //Angle limit of the ground to decide when the character is considered sliding.
    public float slideForce = 5000f;                                    //Strength of the slide.
    public string slideTag = "Slide";                                   //Force sliding state by using this tag on the terrain.
    public bool velocityMovement = false;                                //This variable is used to allow velocity take full control of the character when nothing is being pressed.
}


/// <summary>
/// This class handles all variables which affect the rotation of the character.
/// </summary>
[System.Serializable]
public class RotationSettings
{
    public float rotationSpeed = 5f;                    //How fast the character turns to desired location.
    public Vector3? rotationDirection = null;           //Current desired location.
    public bool velocityRotation = false;                //Overrides the desired location and forces the character to face based on their current velocity.
    public Vector3? overrideVector = null;              //This vector is used to force an instant rotation for the character, this value is generally set as null however when a value is inputted the character will turn to the vector.
    [Range(0, 1)] public float airRotation = .1f;       //How much the character can rotate while in mid air, a value of 1 gives full rotation in mid air while 0 does not allow the character to turn while not grounded.
}


/// <summary>
/// This class handles collision data for the character.
/// </summary>
[System.Serializable]
public class CollisionSettings
{
    public bool debugRay = true;                            //This allows the ray to be visible within the scene view if necessary to see if it hits the grounded correctly.
    public bool grounded = false;                                   //Variable is used to define if the character is currently grounded.
    public LayerMask groundLayerMask;                               //List of layers which the character can declare grounded on contact.
    public float groundRayDistance = 1.5f;                          //Length of grounded ray cast, generally doesn't need to be changed unless scale of characters are much larger or smaller than the example.
    public RaycastHit groundHit;                                    //Raycast data when the ground is hit accessible for other classes/methods that may need information.
    public bool wallCollision = false;
    public LayerMask wallLayerMask;                               //List of layers which the character can declare grounded on contact.
    public float wallRayDistance = 1.5f;                          //Length of grounded ray cast, generally doesn't need to be changed unless scale of characters are much larger or smaller than the example.
    public RaycastHit wallHit;
    [HideInInspector] public Vector3 groundNormal = Vector3.zero;   //the angle of the face the character is currently stood on, used to influence gravity with gravityOverrideOnSurface and other applications.
}


/// <summary>
/// The Main class which handles all movement, rotation, collisions and velocity placed on the character. All classes which use the CharacterMotor including itself extend through AbstractBehaviour which
/// is a alternate version of MonoBehaviour which collects references to certain components affected by these scripts such as Rigidbody, Colliders and Cameras.
/// </summary>
public class CharacterMotor : AbstractBehaviour
{
    //These variables create references to the classes above for use within the main CharacterMotor class.
    public GeneralSettings general = new GeneralSettings();
    public MovementSettings movement = new MovementSettings();
    public RotationSettings rotation = new RotationSettings();
    public CollisionSettings collision = new CollisionSettings();




    void Start()
    {
        m_Rigidbody.useGravity = false;
        m_Rigidbody.freezeRotation = true;
    }
    /// <summary>
    /// This update calls non physic based methods, within this update the methods called check if the character is grounded and rotates the character.
    /// </summary>
    void Update()
    {
        CheckGrounded();
        CheckCollision();
        Rotation();
    }


    /// <summary>
    /// This method checks if the character is in contact with the ground and sets the state boolean, it also controls the sliding state and its requirements to be active.
    /// </summary>
    void CheckGrounded()
    {
        //We reset the sliding value to false, so if the character isn't sliding the rest of the method will continue leaving the value as false.
        movement.sliding = false;


        //Define the grounded ray using the specified ray distance variable within the collision class.
        Ray ray = new Ray(transform.position + Vector3.up, -transform.up * collision.groundRayDistance);


        //This is active using the debug grounded ray variable allowing for a visible ray to be shown in the scene view to verify if the ray is the correct size. This can be removed after you have a correct grounded ray distance value.
        if (collision.debugRay)
            Debug.DrawRay(ray.origin, ray.direction * collision.groundRayDistance);


        //If the raycast hits anything on the groundLayerMask defined in the collision class, the content is run.
        if (Physics.Raycast(ray, out collision.groundHit, collision.groundRayDistance, collision.groundLayerMask))
        {
            if (collision.groundHit.transform == transform)
                return;
            //Records the angle of the face the character is stood on this is stored in a variable for use by the gravity method.
            collision.groundNormal = collision.groundHit.normal;
            //Calculates the angle between the ground's normal and the up vector and compares it to the defined slideLimit within movement. 
            //If the value is greater than the limit the character is set to sliding, if the ground object doesn't use the specified tag, in this case "Prop".
            if (Vector3.Angle(collision.groundNormal, Vector3.up) > movement.slideLimit)
            {
                if (collision.groundHit.transform.tag != "Prop")
                    movement.sliding = true;


            }


            //If the tag of the ground object is the same as the defined slideTag in movement the character is forced into the sliding state ignoring the slideLimit.
            if (collision.groundHit.transform.tag == movement.slideTag)
            {
                movement.sliding = true;
            }


            //If the raycast hit's the ground collision.grounded is set to true;
            collision.grounded = true;
            if (m_Rigidbody.velocity.y < 0)
            {
                movement.velocityMovement = false;
            }


            //If the raycast collides with an object with the tag defined as movingPlatformTag and the player wasn't currently on a moving platform, 
            //the bool is set to true and the character is parented with the moving platform's transform.
            //The player then receives the same translation and rotational information as the moving platform.
            if (collision.groundHit.transform.tag == general.movingPlatformTag)
            {
                if (!general.onMovingPlatform)
                {
                    //If 
                    general.onMovingPlatform = true;
                    transform.parent = collision.groundHit.transform;
                }


            }
            //If the onMovingPlatform bool is set to true however the collision does not detect a moving platform underneath the player.
            //The player is unparented from all transforms and the bool is reset. Releasing the player from the moving platform.
            if (general.onMovingPlatform)
            {
                if (collision.groundHit.transform.tag != general.movingPlatformTag)
                {
                    general.onMovingPlatform = false;
                    transform.parent = null;
                }
            }
        }
        else
        {
            //Follow on from the previous section checking if there is no collision for the ground ray. But the bool onMovingPlatform is true everything is reset.
            if (general.onMovingPlatform)
            {


                if (collision.groundHit.transform == null) {
                    general.onMovingPlatform = false;
                    transform.parent = null;


                }
            }
            //If the raycast does not hit anything, collision.grounded is set to false;
            collision.grounded = false;


        }
    }


    /// <summary>
    /// This method places a ray directly forward from the player to detect where the player is facing into a wall. This is used for additional animation support and wall jumping.
    /// </summary>
    void CheckCollision()
    {
        //Create a ray which moves forward based on set parameters.
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward * collision.wallRayDistance);


        //If the debugRay bool is true, we draw the ray to visualize if it's the correct length and location.
        if (collision.debugRay)
            Debug.DrawRay(ray.origin, ray.direction * collision.wallRayDistance);


        //When the ray collides with an object with the same layer as defined in the wallLayerMask the bool is set to true or false, the hit data is also transferred to the wallHit variable.
        if (Physics.Raycast(ray, out collision.wallHit, collision.wallRayDistance, collision.wallLayerMask))
        {
            collision.wallCollision = true;
        }
        else
        {
            collision.wallCollision = false;
        }


    }


    /// <summary>
    /// This method calculates the desired rotation of the character based on the input and rotations towards that location.
    /// </summary>
    void Rotation()
    {
        
        if (rotation.rotationDirection.HasValue)
        {
            //This checks the type of input that is currently controlling the rotation of the character. 
            if (rotation.overrideVector.HasValue)
            {
                //If the overrideVector is not set to Null the rotationDirection becomes the overrideVector.
                rotation.rotationDirection = rotation.overrideVector.Value;
            }
            else if (rotation.velocityRotation || movement.sliding)
            {
                //If the character is sliding or has the velocityRotation boolean set as true, it faces the direction of the velocity
                rotation.rotationDirection = m_Rigidbody.velocity;
            }
            else
            {
                //This will attempt to rotate the character to his movementDirections if none of the conditions apply.
                rotation.rotationDirection = movement.movementDirection;


            }
        }
        else
        {
            //However if the rotationDirection doesn't have a value the character will face the direction of the inputted movement.
            rotation.rotationDirection = movement.movementDirection;


        }


        //While rotationDirection should always have a value this will catch any exceptions to that rule and the if statement will being rotating the character to the desired rotation.
        if (rotation.rotationDirection.HasValue)
        {
            if (rotation.rotationDirection.Value == Vector3.zero)
                return;
            //Initially we transfer the rotationDirection vector into a Quaternion.
            Quaternion newRotation = Quaternion.LookRotation(rotation.rotationDirection.Value) ;
            newRotation.z = transform.rotation.z;
            newRotation.x = transform.rotation.x;


            //If the overrideVector has a value the transform.rotation will just be overridden with the new rotation value and overrideVector will be reset to null.
            //If the overrideVector does not contain a value transform.rotation will gradually translate to the desired rotation.
            if (rotation.overrideVector.HasValue)
            {
                transform.rotation = newRotation;
            }
            else
            {
                //Create a new rotation speed variable which can be changed.
                float newRotationSpeed = rotation.rotationSpeed;


                //Rotation speed is affected based on the character being grounded or not using the airRotation variable.
                if (rotation.airRotation < 1)
                    newRotationSpeed = collision.grounded ? newRotationSpeed : newRotationSpeed * rotation.airRotation;
                
                //Input the new rotation by the gradually rotating the character's rotation towards the new rotation based on the new calculated rotation speed.
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * newRotationSpeed);
            }


        }


    }


    /// <summary>
    /// This update calls physic based methods, the methods called apply gravity to the character and move the character.
    /// </summary>
    void FixedUpdate()
    {
        Gravity();
        Movement();
        rotation.overrideVector = null;
    }


    /// <summary>
    /// This method controls how the character is affected by gravity.
    /// </summary>
    void Gravity()
    {
        //This functionally stops the character from sliding down light slopes even when the character isn't considered sliding, 
        //when the character is grounded and the boolean is true the characters gravity will be set to the opposite direction of the ground.
        if (general.gravityOverrideOnSurface && collision.grounded)
        {
            general.gravityCurrentDirection = -collision.groundNormal;
        }
        else
        {
            //However if the character is either not grounded or the boolean is false gravity is set to the predetermined value.
            general.gravityCurrentDirection = general.gravityDirection;
        }




        //Apply current gravity direction to the character multiplied by the strength of gravity.
        m_Rigidbody.AddForce(general.gravityCurrentDirection * general.gravityForce);


    }


    /// <summary>
    /// This method changes the movement of the character based on the input through movement.movementDirection.
    /// </summary>
    void Movement()
    {
        //Checks if the velocityMovement variable is active and if there is no movement input the character will retain its velocity.
        if (movement.velocityMovement && movement.movementDirection == Vector3.zero)
        {
            return;
        }


        //Store the characters current velocity.
        Vector3 velocity = m_Rigidbody.velocity;


        //Calculates a new desired velocity using movementDirection multiplied by movementSpeed removing the previous velocity.
        Vector3 newVelocity = ((movement.movementDirection * movement.movementSpeed) - velocity);


        //Here we clamp the values maximum velocity change by depending on the velocityInfluence based on the grounded state.
        float velocityMax = collision.grounded ? movement.groundedVelocityInfluence : movement.airVelocityInfluence;


        newVelocity.x = Mathf.Clamp(newVelocity.x, -velocityMax, velocityMax);
        newVelocity.z = Mathf.Clamp(newVelocity.z, -velocityMax, velocityMax);
        newVelocity.y = 0;


        //Finally we apply the new velocity as a force to the rigidbody.
        m_Rigidbody.AddForce(newVelocity, ForceMode.VelocityChange);






        //If the character is sliding, an additional force will be applied down on the character forcing the character to slide.
        if (movement.sliding)
        {
            m_Rigidbody.AddForce(Vector3.down * movement.slideForce * Time.deltaTime);
        }


    }




    /// <summary>
    /// This method handles multiple external forces being applied at the same time which can cause large unintended velocity spikes for the character.
    /// </summary>
    public void ChangeVelocity(Vector3 direction)
    {
        if (!general.forceInfluence)
        {
            m_Rigidbody.velocity += direction;
            general.forceInfluence = true;
            Invoke("ResetForceInfluence", 0.1f);
        }


    }


    /// <summary>
    /// The method resets the general.forceInfluence variable to false. Invoke by the AddForce Method, this limits the number of forces which can affect the character at one time.
    /// </summary>
    void ResetForceInfluence()
    {
        general.forceInfluence = false;
    }


}