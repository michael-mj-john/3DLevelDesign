using UnityEngine;
using System.Collections;

/// <summary>
/// This class extends Movement.cs adding additional support for Gamepad and animating the character.
/// </summary>
public class AdvancedMovement : Movement {

    public override void Update()
    {
        base.Update();

        //This will rotate the camera slightly when moving left or right when the user is using a Gamepad or using Mobile control schemes
        if(m_Input.current.type == InputManager.ControlType.Gamepad || m_Input.current.type == InputManager.ControlType.Mobile)
            if (m_Motor.collision.grounded)
                if (m_Camera.position.distanceFromTarget < -1)
                    m_Camera.orbit.hRotation += m_Input.current.moveH * 150f * 0.5f * Time.deltaTime;



        //If the character has an animator attached this code will input the Speed and Grounded variables.
        if (m_Animator)
        {
            Vector3 newVelocity = m_Rigidbody.velocity;
            newVelocity.y = 0;
            m_Animator.SetFloat("Speed", newVelocity.magnitude);
            m_Animator.SetBool("Grounded", m_Motor.collision.grounded);
        }
    }
}
