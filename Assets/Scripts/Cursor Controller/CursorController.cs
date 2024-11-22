using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    
    void Start()
    {
    }

   
    void Update()
    {
        ControlCursor();
    }


    void ControlCursor()
    {

       
        if(Input.GetKeyDown (KeyCode.Tab))
        { 
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
