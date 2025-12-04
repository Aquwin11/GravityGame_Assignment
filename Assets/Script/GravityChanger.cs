using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    public Vector3 gravityScale;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Physics.gravity = gravityScale;
        }
    }
}
