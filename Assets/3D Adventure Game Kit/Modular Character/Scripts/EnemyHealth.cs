using UnityEngine;
using System.Collections;

/// <summary>
/// This class extends Health and adds additional support for enemy health, changing the death method and communicating the information to the Enemy AI.
/// </summary>
public class EnemyHealth : Health {

    public EnemyAI ai;  //Reference to the EnemyAi.cs on the character.

    /// <summary>
    /// Method uses the base Start from Health.cs however retrieving any necessary references.
    /// </summary>
    public override void Start()
    {
        base.Start();
        ai = GetComponent<EnemyAI>();

    }

    /// <summary>
    /// Method overrides the Dead method from Health.
    /// </summary>
    public override void Dead()
    {
        //Communicate the information to the EnemyAI.
        ai.state = EnemyAI.State.Dead;
        //Activate Death Animation.
        m_Animator.SetTrigger("Dead");
        //Begin the countdown to destroy the character.
        StartCoroutine("DestroyCharacter");
    }

    /// <summary>
    /// Method overrides the Knockback method from Health.
    /// </summary>
    public override void Knockback(Vector3? origin)
    {
        //Force the character to be knocked backwards rather than using an origin as the player will jump on the players head.
        m_Rigidbody.velocity = Vector3.zero;
        Vector3 direction = -transform.forward;
        //Apply direction to the Motor.
        m_Motor.ChangeVelocity(direction * knockbackForce);
        //Communicate the state to the EnemyAI.
        ai.state = EnemyAI.State.Damaged;
    }

    /// <summary>
    /// IEnumerator which changes the color of the character and declares the character dead.
    /// </summary>
    IEnumerator DestroyCharacter()
    {
        //Freeze the character before destroying it.
        m_Rigidbody.isKinematic = true;
        //Destroy it's collision.
        Destroy(GetComponent<CapsuleCollider>());
        //Change the characters color.
        m_Material.color = Color.black;
        //Wait for 2 Seconds.
        yield return new WaitForSeconds(2f);
        //Destroy the character.
        Destroy(gameObject);

    }

    /// <summary>
    /// Method overrides Health.cs EndInvincibility and just updates the AI with information.
    /// </summary>
    public override void EndInvincibility()
    {
        //Uses the script from Health.cs
        base.EndInvincibility();
        //If the character isn't dead, the character begins chasing after the invincibility time is finished. 
        if(currentHealth > 0)
            ai.state = EnemyAI.State.Chase;
    }


}
