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

        // Update is called once per frame
        void Update()
        {
            if (useTimes <= 0)
            {
                isUseful = false;
                isShow = false;
            }

            if (isUseful && Input.GetKeyUp(KeyCode.E))
            {
                if (useTimes > 0)
                {
                    PD.Play();
                    useTimes--;
                    UI.HideMessage();
                }

            }
        }
    }
}


