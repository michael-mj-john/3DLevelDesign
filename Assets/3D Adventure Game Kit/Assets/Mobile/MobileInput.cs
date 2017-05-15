using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MobileInput : MonoBehaviour {

    public InputManager input;  //Reference to the main InputManager.
    public Vector2 joystickValue;   //Current value of the joystick.

    /// <summary>
    /// Joystick communicates with this method to store it's current value;
    /// </summary>
    /// <param name="value"></param>
    public void StoreJoystick(Vector2 value)
    {
        joystickValue = value;
    }

    /// <summary>
    /// The Mobile Canvas Jump button communicates with this method to make the character jump.
    /// </summary>
    public void Jump()
    {
        input.GetComponent<Jump>().OnJump();
    }

    /// <summary>
    /// On Awake sets up a new control scheme for Mobile input.
    /// </summary>
    void Awake()
    {
        if (input == null)
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>();

        InputManager.Control newControl = new InputManager.Control();
        newControl.mobileInput = this;
        newControl.name = "Mobile";
        newControl.type = InputManager.ControlType.Mobile;
        input.AddInput(newControl);
    }

    /// <summary>
    /// Update checks if the current input type is mobile and uses the values retrieved by the mobile canvas in the current input control scheme.
    /// </summary>
    void Update()
    {
        if (input.current.type == InputManager.ControlType.Mobile)
        {
            input.current.moveH = joystickValue.x;
            input.current.moveV = joystickValue.y;

            //Checks touch count and deciphers how the player is currently touching the screen and how that transfers into input.
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).deltaPosition.magnitude > 0.05)
                {
                    //If the joystick isn't being used the touch refers to the camera control else the second touch refers to the camera control.
                    if (joystickValue.magnitude == 0)
                    {

                        input.current.cameraH = Input.GetTouch(0).deltaPosition.x;
                        input.current.cameraV = Input.GetTouch(0).deltaPosition.y;
                    }
                    else
                    {
                        input.current.cameraH = Input.GetTouch(1).deltaPosition.x;
                        input.current.cameraV = Input.GetTouch(1).deltaPosition.y;
                    }
                }
                else
                {
                    input.current.cameraH = 0;
                    input.current.cameraV = 0;
                }
            }
        }
    }
}
