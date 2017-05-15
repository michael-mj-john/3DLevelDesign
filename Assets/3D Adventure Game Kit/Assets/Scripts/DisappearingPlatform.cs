using UnityEngine;
using System.Collections;

/// <summary>
/// Method that handles disappering platforms after they have been stood on.
/// </summary>
public class DisappearingPlatform : MonoBehaviour {

    public float pressureLength = 1f;   //Time before the platform disappears.
    public float returnLength = 5f;     //Time it takes before the platform returns.
    public float blinkInterval = 0.1f;  //The time inbetween each color blink.
    public Color newColor = Color.red;  //Color the platform changes to.

    Material mat;       //Reference to the material of the platform.
    Color baseColor;    //The starting color of the platform.

    /// <summary>
    /// Sets the two private variables Material and Base Color.
    /// </summary>
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        baseColor = mat.color;
    }

    /// <summary>
    /// When the player starts a collision with the platform begin Invoking the Methods which flash and remove the platform.
    /// </summary>
    /// <param name="C"></param>
    void OnCollisionEnter(Collision C)
    {
        if (C.transform.tag == "Player")
        {
            InvokeRepeating("FlashPlatform", blinkInterval, blinkInterval);
            Invoke("RemovePlatform", pressureLength);
        }
    }

    //Change the color of the platform either to baseColor or newColor.
    void FlashPlatform()
    {
        if(mat.color == baseColor)
        {
            mat.color = newColor;
        }
        else
        {
            mat.color = baseColor;
        }
    }

    /// <summary>
    /// Set the gameObject to inActive and Invoke to return the platform using the length variable.
    /// </summary>
    void RemovePlatform()
    {
        gameObject.SetActive(false);
        CancelInvoke();
        Invoke("ReturnPlatform", returnLength);
    }

    /// <summary>
    /// Reset the platform to it's original state.
    /// </summary>
    void ReturnPlatform()
    {
        gameObject.SetActive(true);
        mat.color = baseColor;
    }
}
