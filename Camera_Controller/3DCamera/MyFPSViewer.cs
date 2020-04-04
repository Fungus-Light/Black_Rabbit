using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Black_Rabbit
{
    public class MyFPSViewer : MonoBehaviour
    {
        public float weaponRange = 50f;                                     // Distance in Unity units over which the player can fire
        private Camera fpsCam;                                              // Holds a reference to the first person camera                                           // Float to store the time the player will be allowed to fire again, after firing
        public MessageBar ui;

        private Trigger_basic prevTrigger;

        void Start()
        {
            //初始化摄像机
            fpsCam = GetComponentInParent<Camera>();
            ui = FindObjectOfType<MessageBar>();
            ui.HideMessage();
        }


        void Update()
        {
            // 在屏幕中间创建一个Vecter3
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // 声明射线
            RaycastHit hit;

            // 检查是否碰到任何物体
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {

                // Get a reference to a health script attached to the collider we hit
                Trigger_basic item = hit.collider.GetComponent<Trigger_basic>();

                if (item != null)
                {

                    if (item.isShow)
                    {
                        ui.ShowMessage(item._Name, item._Message, item.messagePos.transform);
                    }
                    

                    CLearPrev();
                    prevTrigger = item;
                    if (item.gameType==GameType.FPS)
                    {
                        item.MakeUseful();
                    }
                    

                }
                else
                {
                    ui.HideMessage();

                    CLearPrev();
                }
            }
            else
            {
                ui.HideMessage();
            }

        }

        public void CLearPrev()
        {
            if (prevTrigger != null)
            {
                prevTrigger.MakeUseless();
                prevTrigger = null;
            }
        }

    }
}

