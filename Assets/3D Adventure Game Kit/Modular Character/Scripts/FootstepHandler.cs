using UnityEngine;
using System.Collections;

//Altered version of http://wiki.unity3d.com/index.php/FootstepHandler Jake Bayer's Footstep Handler to be used with the character controller.
[RequireComponent(typeof(AudioSource))]
public class FootstepHandler : AbstractBehaviour
{
    public AudioClip[] sounds;          //An array that stores the footstep sounds.
    public float interval = 0.1f;   //The time between footsteps.
    public float vol = 0.2f;        //Base volume.
    public float volVarience = 0.4f;    //The amount of varience that can effect the footsteps.

    IEnumerator Start()
    {
        while (true)
        {
            float vel = m_Rigidbody.velocity.magnitude;
            //If the character is grounded and currently moving.
            if (m_Motor.collision.grounded && vel > 0.01f)
            {
                //Change the volume and select a sound clip and play the clip, then wait for the interval.
                m_Audio.volume = Random.Range(vol / volVarience, vol * volVarience);
                m_Audio.clip = sounds[Random.Range(0, sounds.Length)];
                m_Audio.Play();
                yield return new WaitForSeconds(interval);
            }
            else
            {
                yield return 0;
            }
        }


    }
}