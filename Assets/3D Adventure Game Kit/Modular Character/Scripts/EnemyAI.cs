using UnityEngine;
using System.Collections;
/// <summary>
/// Variables for how the enemy attacks. Stats for the attacks.
/// </summary>
[System.Serializable]
public class Attack
{
    public float range = 1.5f;  //Attack range.
    public float coolDown = 2f; //Cooldown before the enemy attacks again.
    public float animationTime = 1f;    //The time when the attack hurts the target.
    public int damage = 1;  //Amount of damage the attack deals
    [HideInInspector] public float timer = 2f;    //Internal Timer for attacking
    public bool attacking = false;  //Bool to determine if the enemy is currently attacking.
}
/// <summary>
/// Method for EnemyAI extends AbstractBehaviour
/// </summary>
public class EnemyAI : AbstractBehaviour {

    public enum State   //Enum for different states the enemy can be in.
    {
        Chase,
        Idle,
        Attack,
        Damaged,
        Dead
    }

    public State state; //Current Enemy state.

    public Transform target;    //Current enemy target.

    public Attack attack = new Attack();    //Attack variables class.
    //Gives each enemy a variance in movement and rotation speeds to split them apart when they chase you.
    public float movementVariance;       
    public float rotationVariance;
    public float bounceFactor = 10f; //How high the player bounces off the enemys head.

    /// <summary>
    /// Set up the movementVariance to effect movementSpeed of the m_Motor.
    /// </summary>
    void Start()
    {
        if (movementVariance > 0)
            m_Motor.movement.movementSpeed += Random.Range(-movementVariance, movementVariance);


        if (rotationVariance > 0)
            m_Motor.rotation.rotationSpeed += Random.Range(-rotationVariance, rotationVariance);

    }

    /// <summary>
    /// Update handles movement animation, updates the attack timer and calls the enemy state logic.
    /// </summary>
    void Update()
    {
        //Movement Animation.
        if (m_Animator)
        {
            m_Animator.SetFloat("Speed", m_Rigidbody.velocity.magnitude);
        }
        
        //Timer control.
        attack.timer -= Time.deltaTime;

        //Update enemy actions.
        StateLogic();
    }
    
    /// <summary>
    /// This decides the current state of the enemy and what the enemy does in that state.
    /// </summary>
    void StateLogic()
    {
        switch (state)
        {
            case State.Idle:
                if (attack.attacking)
                    return;

                //If the enemy is in Idle if it is in idle, if the enemy has a target change to chase state.
                if (target)
                    state = State.Chase;
                break;
            case State.Chase:
                if (attack.attacking)
                    return;

                //if the enemy has lost the target while chasing returns to idle.
                if (!target)
                {
                    state = State.Idle;
                }
                //Checks distance between enemy and target.
                else
                {
                    Vector3 pos = transform.position;
                    Vector3 vec = target.transform.position;
                    Vector3 direction = vec - pos;
                    if (Vector3.Dot(direction, transform.forward) > .8f)
                    {
                        //If within attack range changes state to attack.
                        if (Vector3.Distance(transform.position, target.transform.position) <= attack.range)
                            state = State.Attack;
                    }
                }
                //Activates the chase logic within movement.
                Movement();
                break;
            case State.Attack:
                if (attack.attacking)
                    return;
                if (target)
                {
                    //Makes sure target is still attackable from current location
                    Vector3 pos = transform.position;
                    Vector3 vec = target.transform.position;
                    Vector3 direction = vec - pos;
                    if (Vector3.Dot(direction, transform.forward) < .8f)
                        state = State.Chase;

                    if (Vector3.Distance(transform.position, target.transform.position) > attack.range)
                        state = State.Chase;
                    //if the timer is up and the enemy can attack again it performs a melee.
                    if (attack.timer <= 0f)
                        Melee();
                }
                break;
            case State.Dead:
                //Dead animation
                m_Material.SetFloat("_SliceAmount", Mathf.Lerp(m_Material.GetFloat("_SliceAmount"), 1, .5f * Time.deltaTime));
                break;
        }

    }
    /// <summary>
    /// Used in the chase state to move the enemy towards the target.
    /// </summary>
    void Movement()
    {
        Vector3 moveVector = Vector3.zero;

        if (state != State.Dead && state != State.Attack)
        {
            if (target)
            {
                moveVector = target.transform.position - transform.position;
                moveVector = moveVector.normalized;
            }


        }

        m_Motor.movement.movementDirection = moveVector;

    }
    //Calls the attack Ienumerator and sets the enemy to attacking.
    void Melee()
    {
        if (state != State.Dead)
        {
            attack.attacking = true;
            StartCoroutine(Attack());
        }
    }

    /// <summary>
    /// Handles Animation, timing and damaging the target.
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        //Animation
        if (m_Animator != null)
        {
            m_Animator.SetTrigger("Attack");

        }

        //Waits for damage timing
        yield return new WaitForSeconds(attack.animationTime);

        //If all the sitatuons allow for the attack to go through
        if (state != State.Dead)
        {
            if (target != null)
            {
                Vector3 pos = transform.position;
                Vector3 vec = target.transform.position;
                Vector3 direction = vec - pos;
                if (Vector3.Dot(direction, transform.forward) > .5f)
                {
                    if (Vector3.Distance(pos, target.transform.position) < attack.range)
                    {
                        //Damage the target
                        target.GetComponent<Health>().Damage(attack.damage, transform.position);
                    }
                }
            }
            //Reset state.
            state = State.Idle;
            attack.attacking = false;
            //Reset timer.
            attack.timer = attack.coolDown;
        }
    }

    /// <summary>
    /// Vision area is using a trigger sphere collider and sets the player to the target on enter.
    /// </summary>
    /// <param name="C"></param>
    void OnTriggerStay(Collider C)
    {
        if (state != State.Dead)
        {
            if (C.transform.tag == "Player")
            {
                target = C.transform;
            }
        }

    }

    /// <summary>
    /// Vision area is using a trigger sphere collider and sets the target to null when the player leaves the trigger sphere.
    /// </summary>
    /// <param name="C"></param>
    void OnTriggerExit(Collider C)
    {
        if (state != State.Dead)
        {
            if (C.transform.tag == "Player")
            {
                target = null;
            }
        }
    }

    /// <summary>
    /// This handles the ability for the player to damage the enemy.
    /// </summary>
    /// <param name="C"></param>
    void OnCollisionEnter(Collision C)
    {
        if (C.gameObject.tag == "Player")
        {
            Vector3 direction = target.transform.position - transform.position;
            //Checks the direction of the target to the enemy.
            if (direction.y >= .7f)
            {
                //If the target is moving down.
                if (C.transform.GetComponent<CharacterMotor>().m_Rigidbody.velocity.y < 0.2) { 
                    //Damage the enemy.
                    GetComponent<Health>().Damage(1, transform.position);
                    //Bounce the player off the enemy's head.
                    C.transform.GetComponent<CharacterMotor>().m_Rigidbody.velocity = Vector3.zero;
                    Vector3 newVelocity = m_Motor.movement.movementDirection * bounceFactor / 20;
                    newVelocity.y = bounceFactor;
                    C.transform.GetComponent<CharacterMotor>().ChangeVelocity(newVelocity);
                }
            }

        }

    }
}
