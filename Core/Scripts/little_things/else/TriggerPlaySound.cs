using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Black_Rabbit;

public enum TriggerType
{
    Basic, Switch, Talk, Timeline
};

[RequireComponent(typeof(AudioSource))]
public class TriggerPlaySound : MonoBehaviour
{
    public AudioSource audio;
    public Trigger_basic trigger;

    public TriggerType type;

    public KeyCode key = KeyCode.E;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop=false;
        trigger = GetComponent<Trigger_basic>();
    }

    private void Start()
    {
        if (trigger is Trigger_switch)
        {
            type = TriggerType.Switch;
        }
        else if (trigger is Trigger_talk)
        {
            type = TriggerType.Talk;
        }
        else if (trigger is Trigger_timeline)
        {
            type = TriggerType.Timeline;
        }
        else
        {
            type = TriggerType.Basic;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(key)&&(type!=TriggerType.Basic))
        {
            if (trigger.isUseful==true)
            {
                PlaySound();
            }
        }
    }


    public void PlaySound()
    {
        if (type == TriggerType.Switch)
        {
            audio.Play();
        }else if (type == TriggerType.Talk)
        {
            if ((trigger as Trigger_talk).isTalking==false)
            {
                audio.Play();
            }
        }else if (type==TriggerType.Timeline)
        {
            if ((trigger as Trigger_timeline).useTimes>0||(trigger as Trigger_timeline).isUnlimited==true)
            {
                if ((trigger as Trigger_timeline).PD.state!=UnityEngine.Playables.PlayState.Playing)
                {
                    audio.Play();
                }
                
            }
        }
    }
}
