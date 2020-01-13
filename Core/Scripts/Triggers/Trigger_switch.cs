using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Black_Rabbit
{
    [AddComponentMenu("Black-Rabbit/Trigger/with_switch")]
    [RequireComponent(typeof(simple_switch))]
    public class Trigger_switch : Trigger_basic
    {
        public bool CanRevert=true;
        private bool IsSwitched = false;

        private void Update()
        {
            if (isUseful && Input.GetKeyUp(KeyCode.E))
            {
                if (IsSwitched)
                {
                    GetComponent<simple_switch>().Revert_it();
                    IsSwitched = false;
                }
                else
                {
                    GetComponent<simple_switch>().Do_switch();
                    IsSwitched = true;
                }
                
            }
        }
    }
}

