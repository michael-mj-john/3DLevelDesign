using UnityEngine;
using System.Collections;

public class AdditionalAnimationSupport : AbstractBehaviour {

    float currentWeight = 0;     //Current animation weight used for lerping to desired float.
    public float speed = 10f;   //Speed of the lerp.

    /// <summary>
    /// This is used to lift the players hands when he is colliding with a wall the code checks if the player is in certain situations and lerps the weight of animation layer to blend the animation in and out.
    /// </summary>
	void Update () {

        bool wallState = false; //Bool set in the if statement used to decide state of the animation if the player is colliding with a wall.
        //Gets the layer and changes the weight of the layer depending on the situation of the character.
        if (m_Motor.collision.wallCollision)
        {
            if (m_Motor.collision.grounded)
            {
                if (currentWeight != 1)
                {
                    currentWeight = Mathf.Lerp(currentWeight, 1, speed * Time.deltaTime);
                    m_Animator.SetLayerWeight(1, currentWeight);
                }
            }
            else
            {
                currentWeight = Mathf.Lerp(currentWeight, 0, speed * Time.deltaTime);
                m_Animator.SetLayerWeight(1, currentWeight);
                wallState = true;
            }

        }
        else
        {
            if (m_Motor.collision.grounded)
            {
                if (currentWeight != 0)
                {
                    currentWeight = Mathf.Lerp(currentWeight, 0, speed * Time.deltaTime);
                    m_Animator.SetLayerWeight(1, currentWeight);
                }
            }
            else
            {
                currentWeight = Mathf.Lerp(currentWeight, 0, speed * Time.deltaTime);
                m_Animator.SetLayerWeight(1, currentWeight);
                wallState = false;

            }

        }

        //Changes the animation state.
        m_Animator.SetBool("Wall", wallState);
    }
}
