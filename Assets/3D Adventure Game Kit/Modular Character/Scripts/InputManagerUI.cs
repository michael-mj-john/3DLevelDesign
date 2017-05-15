using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputManagerUI : InputManager {

    Text inputText;

    public override void Start()
    {
        base.Start();
        if (GameObject.Find("InputText"))
        {
            inputText = GameObject.Find("InputText").GetComponent<Text>();
            inputText.enabled = false;
        }
    }

    public override void Visual()
    {

        if (GameObject.Find("InputText"))
        {
            base.Visual();
            CancelInvoke();
            inputText.enabled = true;
            inputText.text = "Input: " + current.name;
            Invoke("Disappear", 4f);
        }
        else
        {
            Debug.Log("Cannot locate InputText Objet for InputManagerUI");
        }
    }

    void Disappear()
    {
        inputText.enabled = false;
    }
}
