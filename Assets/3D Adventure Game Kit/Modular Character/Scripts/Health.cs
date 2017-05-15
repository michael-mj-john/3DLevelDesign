using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles all the base methods for modifying a character health.
/// </summary>
public class Health : AbstractBehaviour {

    public int healthMax = 5;               //This variable is the maxiumum amount of health at one time.
    public int currentHealth = 5;           //The current health the character has available.
    public float invincibilityTime = 1f;   //The time the character has before being allowed to be damaged again.
   
    public bool knockbackOnDamage = true;           //Bool enables knockback when the character is damaged.
    public float knockbackForce = 10f;              //If knockback is enabled this is the force the character is knock backed.
    public bool changeColorOnDamage = true;         //If this is enabled the character will turn a different color throughout the invincibility timer.

    [HideInInspector]public bool invincibility = false;     //If this bool is enabled the character cannot take damage.

    //Variables are set on the Start method to reset the characters position on death.
    [HideInInspector]public Vector3 respawnPoint; 
    [HideInInspector]public Quaternion respawnRotation;

    /// <summary>
    /// Start is used to set the initial location variables for respawning the characters.
    /// </summary>
    public virtual void Start()
    {
        respawnPoint = transform.position;
        respawnRotation = transform.rotation;
    }

    /// <summary>
    /// Damage Method handles the conditions on how the player is damaged, knockback and other additional effects which happen when the character loses life.
    /// </summary>
    public virtual void Damage(int value, Vector3? origin)
    {
        //Checks if the character can currently be damaged through the invinicibility bool.
        if (!invincibility)
        {
            //Remove the value sent to the Damage from the ChangeHealth.
            ChangeHealth(-value);
            //Check if the current health is less than or equal to 0, if so kill the player.
            if (currentHealth <= 0)
            {
                Dead();
            }
            else
            {
                //Else we initiate the effects damage has on the character.
                //Knockback with knock the player back from the origin vector is the vector has value.
                if(knockbackOnDamage)
                    Knockback(origin);

                //Change the colour of the character on damage
                if(changeColorOnDamage)
                    ChangeColor();

                //If the characters has an invincibility time set the character to invincible and call it to end after the allocated time.
                if (invincibilityTime > 0)
                {
                    invincibility = true;
                    Invoke("EndInvincibility", invincibilityTime);
                }
            }
        }
    }

    /// <summary>
    /// Public method which communicates with change health to heal the player.
    /// </summary>
    public virtual void Heal(int value)
    {
        ChangeHealth(value);
    }

    /// <summary>
    /// Method used to change the characters health, clamping the values and communicating with any visual indication of the health changing.
    /// </summary>
    public virtual void ChangeHealth(int value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, healthMax);

        VisualUpdate();
    }

    /// <summary>
    /// This void is used for extended classes such as PlayerHealth.cs to update the UI when the health changes.
    /// </summary>
    public virtual void VisualUpdate()
    {

    }

    /// <summary>
    /// What happens when the characters health becomes 0 and dies, This in particular resets the characters health and moves the character to the respawn location, updating the UI.
    /// </summary>
    public virtual void Dead()
    {
        transform.position = respawnPoint;
        transform.rotation = respawnRotation;
        currentHealth = healthMax;
        VisualUpdate();
    }

    /// <summary>
    /// Knockback logic launching the player away from the origin location.
    /// </summary>
    public virtual void Knockback(Vector3? origin)
    {
        //Resetting the velocity to allow for the new velocity to take stronger effect.
        m_Rigidbody.velocity = Vector3.zero;
        //The direction is based on whether the origin has a value, if it does that is the direction, else the direction is just backwards from the character.
        Vector3 direction = (origin.HasValue) ? transform.position - origin.Value : -transform.forward;
        //Add a bonus up factor to the knockback
        direction += transform.up;
        //Apply the velocity.
        m_Motor.ChangeVelocity(direction * knockbackForce);

    }

    /// <summary>
    /// Logic for changing the colour and returning the character to his default colour.
    /// </summary>
    public virtual void ChangeColor()
    {
        m_Material.color = Color.red;
        Invoke("ChangeColorBack", invincibilityTime);
    }

    /// <summary>
    /// Resets the colour of the material to it's base visual.
    /// </summary>
    void ChangeColorBack()
    {
        m_Material.color = Color.white;
    }

    /// <summary>
    /// Resets the invincibility bool invoke when damage is taken.
    /// </summary>
    public virtual void EndInvincibility()
    {
        invincibility = false;
    }

}
