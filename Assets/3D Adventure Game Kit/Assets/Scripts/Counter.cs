using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A basic counter script can be used for multiple purposes however is used display the amount of coins that have been collected.
/// </summary>
public class Counter : MonoBehaviour {

    int value = 0;  //Int on the current count of the counter.
    Text text;      //Visual display of the counter.

    /// <summary>
    /// Finds the Text component on the same GameObject.
    /// </summary>
    void Start()
    {
        text = GetComponent<Text>();
    }

    /// <summary>
    /// Is called when the value is increased and is show on the stored text variable.
    /// </summary>
    public void Increase()
    {
        value++;
        text.text = "x" + value.ToString();
    }
}
