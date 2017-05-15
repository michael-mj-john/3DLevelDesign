using UnityEngine;
using System.Collections;

/// <summary>
/// Class that extends Collectable which handles the coin interaction.
/// </summary>
public class Coin : Collectable {
    
    public GameObject fx;   //Particle Effect Prefab that is used on picked.
    public AudioClip sfx;   //Sound Effect used on pickup.

    /// <summary>
    /// Overidden from Collectable when the object is touched by the player.
    /// </summary>
    /// <param name="player"></param>
    public override void Effect(GameObject player)
    {
        //If there is a specified FX spawn the particle effect.
        if (fx)
        {
            GameObject newFx = Instantiate(fx);
            newFx.transform.position = transform.position;
        }

        //If there is a specified SFX play the clip
        if (sfx != null)
        {
            GameObject.Find("Sound Handler").GetComponent<AudioSource>().clip = sfx;
            GameObject.Find("Sound Handler").GetComponent<AudioSource>().Play();
        }
        //If there is a UI element named CoinText Get the counter class and increase the value.
        if (GameObject.Find("CoinText"))
        {
            GameObject.Find("CoinText").GetComponent<Counter>().Increase();
        }

        //Destroy the collectable on pickup.
        Destroy(transform.parent.gameObject);
    }
}
