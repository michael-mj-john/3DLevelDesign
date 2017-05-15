using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour
{

    private void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().duration);
    }

}