using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Fungus;

[CommandInfo("",
             "播放视频",
             "全屏播放一个视频")]
[AddComponentMenu("")]
public class PlayVideo : Command
{
    public VideoPlayer movie;


    #region Public members

    public override void OnEnter()
    {
        movie.playOnAwake = false;
        movie.Play();
    }

    public override string GetSummary()
    {
        return "将要播放"+movie.name;
    }

    public override Color GetButtonColor()
    {
        return new Color32(220, 220, 220, 255);
    }

    #endregion
}
