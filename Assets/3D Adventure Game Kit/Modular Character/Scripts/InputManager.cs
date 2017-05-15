using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is used to handle multi forms of input and allow for other scripts to change how they function based on the type of input.
/// </summary>
public class InputManager : MonoBehaviour {

    public Control current;     //The currently designated control scheme that is being used.
    
    public List<Control> controlSettings = new List<Control>(); //A List of all possible control schemes

    public bool inverseCameraV, inverseCameraH;     //Settings to reverse the inputs for the camera directions

    public bool lockControlScheme;      //Locks the current control scheme so new input won't change the input. (Better Performance Wise when locked)

    public enum ControlType     //Enum which defines the type of control used in the Control Class, this can be used to change the logic elsewhere based on the type of control.
    {
        Keyboard,
        Gamepad,
        Mobile
    }

    /// <summary>
    /// This method is used to set the current control scheme to the first option in the list of controls.
    /// </summary>
    public virtual void Start()
    {
        current = controlSettings[0];
    }

    /// <summary>
    /// This method is used in InputManagerUI to display on screen the current control scheme when it is changed.
    /// </summary>
    public virtual void Visual()
    {
        //For Override UI
    }

    /// <summary>
    /// This method handles Detecting Input and Recieving the values of the current input.
    /// </summary>
    public void Update() {
    //This checks if the control scheme is locked if it's not it will go through the possible controls and see if it detects Input, if it does the input is changed.
        if(!lockControlScheme){
            foreach (Control setting in controlSettings)
            {
                if (setting != current)
                {
                    if (setting.DetectInput())
                    {
                        current = setting;
                        Visual();
                    }
                }
            }
        }

        //Using the variables in the InputSetting class we retrieve the input and apply it the the corresponding float.

        if (current.type != ControlType.Mobile)
        {
            current.moveV = Input.GetAxis(current.moveVName);
            current.moveH = Input.GetAxis(current.moveHName);
            current.cameraV = Input.GetAxis(current.cameraVName);
            current.cameraV = (!inverseCameraV) ? current.cameraV : -current.cameraV;
            current.cameraH = Input.GetAxis(current.cameraHName);
            current.cameraH = (!inverseCameraH) ? current.cameraH : -current.cameraH;
            current.jumpBtn = (Input.GetButtonDown(current.jumpBtnName))? 1: 0;
            current.cameraReset = Input.GetAxis(current.cameraResetName);
            if (current.scrollInputName != "")
                current.scrollInput = Input.GetAxis(current.scrollInputName);

        }
    }
    public void AddInput(Control newControl)
    {
        controlSettings.Add(newControl);
        current = newControl;
    }

    /// <summary>
    /// This class handles all the variables which a control configuration handles and checks for input by them.
    /// </summary>
    [System.Serializable]
    public class Control
    {
        public string name; //Name for identification purposes used in the InputManagerUI

        public InputManager.ControlType type;   //Defines the type of control it is.

        public string moveVName, moveHName, cameraVName, cameraHName, jumpBtnName, scrollInputName, cameraResetName;    //Names for the inputs used to retrieve the values
        
        public float moveV, moveH, cameraV, cameraH, jumpBtn, scrollInput, cameraReset; //Floats used to store the values retrieved using the names above.

        [HideInInspector]
        public MobileInput mobileInput; //This is set in MobileInput as it creates it's own control scheme this can be left alone.


        /// <summary>
        /// Checks if one of the input names is current active and returns true or false.
        /// </summary>
        /// <returns>If the control scheme currently has a input being detected</returns>
        public bool DetectInput()
        {
            if (mobileInput == null)
            {
                //Checks Button Names Inputs
                if (Input.GetAxis(moveVName) != 0 || Input.GetAxis(moveHName) != 0 || Input.GetButtonDown(jumpBtnName) != false || Input.GetAxis(cameraResetName) != 0)
                {
                    return true;
                }

                //Checks Scroll Input
                if (scrollInputName != "")
                {
                    if (Input.GetAxis(scrollInputName) != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                //Checks Mobile Input
                if(mobileInput.joystickValue.magnitude != 0)
                {
                    return true;
                }
            }

            return false;
        }

    }

}
