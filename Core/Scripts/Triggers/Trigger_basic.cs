using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Black_Rabbit
{

    public enum GameType{
        TPS,FPS
    };

    [AddComponentMenu("Black-Rabbit/Trigger/Basic")]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class Trigger_basic : MonoBehaviour
    {
        void Awake()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
            this.GetComponent<AudioSource>().playOnAwake = false;
            this.GetComponent<AudioSource>().loop = false;
        }
        public AudioSource audio;
        public GameType gameType = GameType.TPS;

        public Transform messagePos;
        public string PosName = "Message_Pos";

        [TextArea(2,10)]
        public string _Name, _Message;
        public MessageBar UI;
        public bool isUseful;
        public bool isTalking = false;

        public bool isShow = true;
        // Start is called before the first frame update
        public virtual void Start()
        {
            UI = FindObjectOfType<MessageBar>();
            isUseful = false;
            isTalking = false;
            messagePos = transform.Find(PosName);
            audio = GetComponent<AudioSource>();
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (gameType==GameType.TPS)
            {
                if (other.tag == "Player")
                {
                    if (isShow)
                    {
                        UI.ShowMessage(_Name, _Message, messagePos);
                    }
                    isUseful = true;
                }
            }
            
        }

        protected void OnTriggerExit(Collider other)
        {
            if (gameType == GameType.TPS)
            {
                if (other.tag == "Player")
                {
                    UI.HideMessage();
                    isUseful = false;
                }
            }
        }


        public void MakeUseful()
        {
            isUseful = true;
        }

        public void MakeUseless()
        {
            isUseful = false;
        }
    }
}


