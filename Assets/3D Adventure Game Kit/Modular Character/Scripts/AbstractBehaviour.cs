using UnityEngine;
using System.Collections;

/// <summary>
/// This class extends Monobehaviour adding multiple references. Each class on the character can extend AbstractBehaviour to have access to these references.
/// </summary>
public abstract class AbstractBehaviour : MonoBehaviour {

    [HideInInspector] public Rigidbody m_Rigidbody;
    [HideInInspector] public CharacterMotor m_Motor;
    [HideInInspector] public CameraControl m_Camera;
    [HideInInspector] public Animator m_Animator;
    [HideInInspector] public Material m_Material;
    [HideInInspector] public AudioSource m_Audio;
    [HideInInspector] public InputManager m_Input;

    /// <summary>
    /// Method which retrieves all the references for the above variables.
    /// </summary>
    protected virtual void Awake()
    {
        //Assign Camera Control Variable
        if (Camera.main.GetComponent<CameraControl>())
        {
            m_Camera = Camera.main.GetComponent<CameraControl>();
        }
        else
        {
            Debug.LogError(Camera.main.name + " requires component : CameraControl");
        }

        //Assign Input Manager Variable if the character uses user input.
        if (GetComponent<InputManager>())
        {
            m_Input = GetComponent<InputManager>();
        }

        //Assign Input Manager Variable if the character uses user input.
        if (GetComponentInChildren<AudioSource>())
        {
            m_Audio = GetComponentInChildren<AudioSource>();
        }

        //Assign Rigidbody Variable
        if (GetComponent<Rigidbody>())
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        else if (GetComponentInChildren<Rigidbody>())
        {
            m_Rigidbody = GetComponentInChildren<Rigidbody>();
        }
        else if (GetComponentInParent<Rigidbody>())
        {
            m_Rigidbody = GetComponentInParent<Rigidbody>();
        }
        else
        {
            Debug.LogError(transform.name + " requires component : Rigidbody");
        }

        //Assign Character Motor Variable
        if (GetComponent<CharacterMotor>())
        {
            m_Motor = GetComponent<CharacterMotor>();
        }
        else if (GetComponentInChildren<CharacterMotor>())
        {
            m_Motor = GetComponentInChildren<CharacterMotor>();
        }
        else if (GetComponentInParent<CharacterMotor>())
        {
            m_Motor = GetComponentInParent<CharacterMotor>();
        }
        else
        {
            Debug.Log(transform.name + " does not have component : CharacterMotor.");
        }

        //Assign Animator Variable
        if (GetComponent<Animator>())
        {
            m_Animator = GetComponent<Animator>();
        }
        else if (GetComponentInChildren<Animator>())
        {
            m_Animator = GetComponentInChildren<Animator>();
        }
        else if (GetComponentInParent<Animator>())
        {
            m_Animator = GetComponentInParent<Animator>();
        }
        
        //Assign Material Variable
        if (GetComponent<SkinnedMeshRenderer>())
        {
            m_Material = GetComponent<SkinnedMeshRenderer>().material;
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            m_Material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        }
        else if (GetComponent<MeshRenderer>())
        {
            m_Material = GetComponent<MeshRenderer>().material;
        }
        else if (GetComponentInChildren<MeshRenderer>())
        {
            m_Material = GetComponentInChildren<MeshRenderer>().material;
        }
        else
        {
            Debug.Log(transform.name + " does not have a Material assigned to either a Mesh Renderer or a SkinnedMeshRenderer.");
        }
    }

}
