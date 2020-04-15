using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Black_Rabbit;

//一个简单的切换移动的小补丁，不建议使用
public class MoveSwitch : MonoBehaviour
{
    public CharacterMovement movement;
    
    void Start()
    {
        movement = GetComponent<CharacterMovement>();
    }

    public void StopMove()
    {
        movement.enabled = false;
    }

    public void ActiveMove()
    {
        movement.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
