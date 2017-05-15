using UnityEngine;
using System.Collections;

/// <summary>
/// This class extends MultiJump.cs adding the functionality to wall jump, slide jump, animation and sound.
/// </summary>
public class AdvancedJump : MultiJump {

    public AudioClip jumpSfx;

    /// <summary>
    /// This method handles the conditions on whether the jump is sucessful.
    /// </summary>
    public override void OnJump()
    {
        //This bool is used to check if the character is able to jump then the character will play the jump animation.
        bool jumped = false;

        //Still the same from Jump.cs and Multijump.cs we are checking the character is grounded initially and attempting to jump from the grounded position,
        //Then if the character isn't grounded we check the types of jump he could perform while in midair.
        if (m_Motor.collision.grounded)
        {
            //When the character is grounded. If the character is sliding we perform a slide jump, if not we perform a normal jump.
            if (m_Motor.movement.sliding)
            {
                //Slide jumps and basic jumps count as part of the multijump.
                jumpsRemaining = jumpCount - 1;
                SlideJump();
                jumped = true;
            }
            else
            {
                jumpsRemaining = jumpCount - 1;
                BaseJump();
                jumped = true;
            }
        }
        else
        {
            //When the character is not grounded. If the character is colliding with the wall we do a wall jump, else if we are not colliding with the wall we attempt to do the next part of the multijump.
            if (m_Motor.collision.wallCollision)
            {
                //Wall jumps aren't classed as a multijump to allow the player to combo their jumps together to get higher up. As jumpsremaining isn't altered.
                WallJump();
                jumped = true;
            }
            else
            {
                if (jumpsRemaining > 0)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    jumpsRemaining--;
                    BaseJump();
                    jumped = true;
                }
            }
        }

        //If Jumped is set to true by any of the conditions being true to tell the animator to Jump.
        if (jumped)
        {
            if (m_Animator)
            {
                m_Animator.SetTrigger("Jump");
            }

            if (m_Audio)
            {
                m_Audio.PlayOneShot(jumpSfx, 1);
            }
        }

    }

    /// <summary>
    /// This method is the logic behind jumping when sliding calculating the direction and the force.
    /// </summary>
    void SlideJump()
    {
        //Calculate a new jump force, and reset the current velocity.
        float newJumpForce = jumpForce / 1.2f;
        m_Rigidbody.velocity = Vector3.zero;
        //Calculate the jump direction based on angle of the floor.
        Vector3 jumpDir = (m_Motor.collision.groundNormal * (1.2f * newJumpForce));
        //Apply the value to the ChangeVelocity method.
        m_Motor.ChangeVelocity(jumpDir);
        //Override the rotation of the character with the velocity of the character.
        m_Motor.rotation.overrideVector = m_Rigidbody.velocity;
        //Allow velocity to take control of the character.
        m_Motor.movement.velocityMovement = true;
    }

    /// <summary>
    /// This method is the logic behind jumping when colliding with a wall calculating the direction and the force.
    /// </summary>
    void WallJump()
    {
        //Retrieve a new jump force and reset the current velocity.
        float newJumpForce = jumpForce;
        m_Rigidbody.velocity = Vector3.zero;
        //Calculate the jump direction using the walls angle
        Vector3 wallDir = m_Motor.collision.wallHit.normal;
        Vector3 jumpDir = (wallDir + (transform.up / 1.2f)) * newJumpForce / 1.2f;
        //Apply the value to the ChangeVelocity Method.
        m_Motor.ChangeVelocity(jumpDir);
        //override the rotation with the jump direction
        m_Motor.rotation.overrideVector = jumpDir;
    }

}
