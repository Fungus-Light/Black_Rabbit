using UnityEngine;
using UnityEngine.Playables;

namespace Black_Rabbit
{
    [AddComponentMenu("Black-Rabbit/Trigger/Timeline")]
    public class Trigger_timeline : Trigger_basic
    {
        [Tooltip("有播放次数限制的timeline")]
        public PlayableDirector PD;
        public int useTimes = 1;
        public KeyCode key = KeyCode.E;
        public bool isUnlimited = false;
        public bool activeAuto = false;

        // Update is called once per frame
        void Update()
        {
            if (useTimes <= 0)
            {
                isUseful = false;
                isShow = false;
            }

            if (isUseful)
            {
                if (activeAuto)
                {
                    PlayTimeLine();
                }
                else
                {
                    if (Input.GetKeyUp(key))
                    {
                        PlayTimeLine();
                    }
                }

                

            }
        }

        public void PlayTimeLine()
        {
            if (useTimes > 0)
            {
                audio.Play();
                PD.Play();
                if (isUnlimited == false)
                {
                    useTimes--;
                }
                UI.HideMessage();
                isUseful = false;
            }
        }

    }
}


