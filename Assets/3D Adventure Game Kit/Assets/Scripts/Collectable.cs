using UnityEngine;
using System.Collections;

/// <summary>
/// Class that handles the base logic for collectable objects such as coins and hearts handles collision and movement.
/// </summary>
public class Collectable : MonoBehaviour
{

    public Vector3 rotationVector;  //Axis the object rotates around.
    public float spinSpeed;         //Rotation speed.

    /// <summary>
    /// Rotates the object based on the inspector variables
    /// </summary>
    void Update()
    {
        transform.Rotate(rotationVector * (spinSpeed * Time.deltaTime));
    }

    /// <summary>
    /// Detects when the object is picked up and calls the Effect Void.
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerEnter(Collider c)
    {

        if (c.tag == "Player")
        {
            Effect(c.gameObject);

        }
    }
    /// <summary>
    /// Open virtual void used for overriding an effect on pickup.
    /// </summary>
    /// <param name="player"></param>
    public virtual void Effect(GameObject player)
    {

    }
}