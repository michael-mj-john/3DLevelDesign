using UnityEngine;
using System.Collections;

/// <summary>
/// Method Handles the mouse being locked and hidden when active in game.
/// </summary>
public class CursorControl : MonoBehaviour {

    /// <summary>
    /// If the mouse button is down the cursor gets locked and hidden, if esc is pressed the mouse is freed and the cursor is visible.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
    
}
