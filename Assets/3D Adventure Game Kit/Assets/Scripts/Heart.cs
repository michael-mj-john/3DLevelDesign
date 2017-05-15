using UnityEngine;
using System.Collections;

/// <summary>
/// Method extends collectable and changes the Effect.
/// </summary>
public class Heart : Collectable {

    public GameObject fx;   //Prefab of Particle Effect.
    public AudioClip sfx;   //Sound Effect.
    public int value = 1;   //How much the heart heals.

    public override void Effect(GameObject player)
    {
        //Makes sure the player isn't at full health
        if (player.GetComponent<Health>().currentHealth < player.GetComponent<Health>().healthMax)
        {
            //If the script has a defined FX then it is spawned when the heart is picked up.
            if (fx)
            {
                GameObject newFx = Instantiate(fx);
                newFx.transform.position = transform.position;

            }

            //If the script has a defined SFX then it is played when the heart is picked up.
            if (sfx != null)
            {
                GameObject.Find("Sound Handler").GetComponent<AudioSource>().clip = sfx;
                GameObject.Find("Sound Handler").GetComponent<AudioSource>().Play();
            }

            //Heal the player for the defined value.
            player.GetComponent<Health>().Heal(value);

            //Destroy the heart.
            Destroy(transform.parent.gameObject);
        }
    }
}
