using UnityEngine;
using System.Collections;

/// <summary>
/// This class extends Jump.cs allowing the player to do multiple jumps.
/// </summary>
public class MultiJump : Jump {

    public int jumpsRemaining = 0;          //The remaining jumps within the current jump action decreases with every jump.
    public int jumpCount = 1;               //Total amount of jumps the player can perform.
    public bool overrideRotation = true;    //This bool can be used to tell the rotation within the CharacterMotor to instantly rotate to the desired rotation.
     
    /// <summary>
    /// This method handles the conditions on whether the jump is sucessful.
    /// </summary>
    public override void OnJump()
    {
        //Similar to the Jump.cs OnJump() however follows on by calculating what jump the character is on and if there are remaining jumps still available allowing the player to jump
        //even when they are not grounded.
        if (m_Motor.collision.grounded)
        {
            jumpsRemaining = jumpCount - 1;
            BaseJump();
        }
        else
        {
            if(jumpsRemaining > 0)
            {
                //Resets the rigidbody velocity to zero to create a nice effect for changing direction with the mid-air jumps this is also helped using the overrideRotation shown below.
                m_Rigidbody.velocity = Vector3.zero;
                jumpsRemaining--;
                BaseJump();
            }
        }
    }

    /// <summary>
    /// This method is the logic behind the jump calculating the direction and the force.
    /// </summary>
    public override void BaseJump()
    {
        //Brings over most of it's code from Jump.cs however the new code override checks that the jump count isn't the initial jump, and every subsequent jump will override the rotation vector. 
        if (overrideRotation)
            if (jumpsRemaining != jumpCount - 1)
                m_Motor.rotation.overrideVector = m_Motor.movement.movementDirection;
        
        base.BaseJump();
    }

}
