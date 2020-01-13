using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple active/disactive many things in one time and can revert 
namespace Black_Rabbit
{
    public class simple_switch : MonoBehaviour
    {
        public GameObject[] to_active;
        public GameObject[] to_close;

        public void Do_switch()
        {
            foreach (GameObject obj in to_active)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in to_close)
            {
                obj.SetActive(false);
            }
        }

        public void Revert_it()
        {
            foreach (GameObject obj in to_active)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in to_close)
            {
                obj.SetActive(true);
            }
        }
        
    }

}
