using UnityEngine;
using System.Collections;

/// <summary>
/// A trigger that damages the player on collision.
/// </summary>
public class TriggerDamage : MonoBehaviour {

    public int damage = 5;  //Amount of damage
    public bool instaKill = false;  //Bool which changes the damage to equal the current health.

    /// <summary>
    /// When the trigger is entered by the player, it deals damage to that player based on the damage variable.
    /// </summary>
    /// <param name="C"></param>
    void OnTriggerEnter(Collider C)
    {
        if (C.transform.tag == "Player")
        {
            int dmg = damage;
            //Changes the value if the instaKill variable is true.
            if (instaKill)
            {
                dmg = C.GetComponent<Health>().currentHealth;
            }
            //Damages the player
            C.GetComponent<Health>().Damage(dmg, null);
        }
    }
}
