using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Black_Rabbit
{
    [AddComponentMenu("Black-Rabbit/Tools/TimeScale")]
    public class TimeMachine : MonoBehaviour
    {
        // Start is called before the first frame update
        
        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ContinueGame()
        {
            Time.timeScale = 1;
        }

        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }
        
    }
}

