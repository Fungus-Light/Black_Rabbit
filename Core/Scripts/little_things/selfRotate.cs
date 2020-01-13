using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Black-Rabbit/Tools/SelfRotate")]
public class selfRotate : MonoBehaviour
{
    public Vector3 v;

    void Start()
    {

    }

    void Update()
    {
        if (v!=null)
        {

        }
        transform.Rotate(v);
        
    }
}
