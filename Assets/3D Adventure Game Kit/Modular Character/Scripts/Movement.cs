using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]

/// <summary>
/// This class base movement class which handles how the character moves.
/// </summary>
public class Movement : AbstractBehaviour{   

    Vector3 cameraForward = Vector3.zero;    //This is a stored variable which calculates the forward direction of the camera so the character will move according to the direction of the character.
    Vector3 cameraRight = Vector3.zero;      //This is the stored variable of the right vector based on the camera which is calculated using the forward camera vector.

    public virtual void Update () {

        //Retrieve the forward vector of the character based on the camera position and normalize. 
        cameraForward = m_Camera.transform.TransformDirection(Vector3.forward);
        cameraForward.y = 0f;
        cameraForward = cameraForward.normalized;

        //Get the right vector using the forward vector.
        cameraRight = new Vector3(cameraForward.z, 0.0f, -cameraForward.x);

        Vector3 movementVector = Vector3.zero;

        //Calculate the movementVector using the input and direction vectors.
        movementVector = (m_Input.current.moveH * cameraRight + m_Input.current.moveV * cameraForward);

        //Limit the overall speed of the character if multiple directions are inputted at the same time.
        movementVector *= (Mathf.Abs(m_Input.current.moveH) == 1 && Mathf.Abs(m_Input.current.moveV) == 1) ? .7f : 1;

        //Give the CharacterMotor the desired movement vector to begin movement.
        m_Motor.movement.movementDirection = movementVector;
    }
}
