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
        public KeyCode key = KeyCode.E;
        public bool activeAuto=false;

        private void Update()
        {
            if (isUseful )
            {
                if (activeAuto==true)
                {
                    ActiveSwitch();
                }
                else if(Input.GetKeyUp(key))
                {
                    ActiveSwitch();
                }
            }
        }

        public void ActiveSwitch()
        {
            if (IsSwitched)
            {
                if (CanRevert)
                {
                    GetComponent<simple_switch>().Revert_it();
                    IsSwitched = false;
                }
            }
            else
            {
                GetComponent<simple_switch>().Do_switch();
                IsSwitched = true;
            }
        }

    }
}

