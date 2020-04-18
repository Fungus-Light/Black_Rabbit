using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightManager : MonoBehaviour
{
    public FlashLight flashlight;
    public KeyCode key=KeyCode.F;

    void Start()
    {
        flashlight = FindObjectOfType<FlashLight>();
        flashlight.gameObject.SetActive(isOn);
    }

    bool isOn = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(key))
        {
            isOn = !isOn;
            flashlight.gameObject.SetActive(isOn);

        }
    }

    public void FlashOn()
    {
        isOn = true;
        flashlight.gameObject.SetActive(isOn);
    }

    public void FlashOff()
    {
        isOn = false;
        flashlight.gameObject.SetActive(isOn);
    }

}
