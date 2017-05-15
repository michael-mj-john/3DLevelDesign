using UnityEngine;
using System.Collections;

/// <summary>
/// This method resets the rotations of a rigidbody platform when the player leaves the platform.
/// </summary>
public class ResetRotation : MonoBehaviour {

    public float speed = 1f;        //Speed the platform resets.
    
    public bool on = true;          //Bool whether the player is currently on the platform.

    Quaternion initialRotation;     //Set on start and is the rotation the platform returns to.

    /// <summary>
    /// Set the initial rotation.
    /// </summary>
    void Start () {
        initialRotation = transform.rotation;
	}
	
    /// <summary>
    /// If the player is not on the platform reset the rotation back to the initial rotation.
    /// </summary>
	void Update () {
        if (!on)
        {
            if(transform.rotation != initialRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, speed * Time.deltaTime);
        }
	}

    /// <summary>
    /// If the Player is colliding with the platform on is set to true.
    /// </summary>
    /// <param name="C"></param>
    void OnCollisionEnter(Collision C)
    {
        if(C.transform.tag == "Player")
        {
            on = true;
        }
    }
    
    /// <summary>
    /// If the Player stops colliding with the platform on is set to false.
    /// </summary>
    /// <param name="C"></param>
    void OnCollisionExit(Collision C)
    {
        if (C.transform.tag == "Player")
        {
            on = false;
        }
    }
}
