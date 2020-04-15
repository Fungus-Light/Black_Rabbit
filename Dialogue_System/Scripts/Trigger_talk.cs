using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace Black_Rabbit
{
    [AddComponentMenu("Black-Rabbit/Trigger/Talk")]
    public class Trigger_talk : Trigger_basic
    {
        public Flowchart control;
        public string _message = "doit";
        public string blockName = "flow";
        public KeyCode key = KeyCode.E;
        public bool activeAuto=false;

        override public void Start()
        {
            base.Start();
            control = transform.Find(blockName).GetComponent<Flowchart>();
        }

        private void Update()
        {
            if (control != null)
            {
                if (control.HasExecutingBlocks() == true)
                {
                    isTalking = true;
                }
                else
                {
                    isTalking = false;
                }
            }
            else
            {
                isTalking = false;
            }



            if (isUseful)
            {
                if (activeAuto==true)
                {
                    ActiveFlowchart();
                }
                else
                {
                    if (Input.GetKeyUp(key))
                    {
                        ActiveFlowchart();
                    }
                }
            }
        }

        public void ActiveFlowchart()
        {
            if (control != null)
            {
                control.SendFungusMessage(_message);
                isUseful = false;
                UI.HideMessage();
            }
        }
    }

}

