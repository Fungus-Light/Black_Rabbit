using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Black_Rabbit
{
    [AddComponentMenu("Black-Rabbit/Trigger/Basic")]
    [RequireComponent(typeof(BoxCollider))]
    public class Trigger_basic : MonoBehaviour
    {
        void Awake()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        public Transform messagePos;
        public string PosName = "Message_Pos";

        [TextArea(2,10)]
        public string _Name, _Message;
        public MessageBar UI;
        public bool isUseful;
        public bool isTalking = false;

        protected bool isShow = true;
        // Start is called before the first frame update
        public virtual void Start()
        {
            UI = FindObjectOfType<MessageBar>();
            isUseful = false;
            isTalking = false;
            messagePos = transform.Find(PosName);
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && isShow)
            {
                UI.ShowMessage(_Name, _Message, messagePos);
                isUseful = true;
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                UI.HideMessage();
                isUseful = false;
            }
        }

    }
}


