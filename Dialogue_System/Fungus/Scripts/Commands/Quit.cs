// fungus-light修改编写

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// 退出应用，在编辑器和webplayer无效
    /// </summary>
    [CommandInfo("Flow", 
                 "退出应用",
                 "退出应用，在编辑器和webplayer无效")]
    [AddComponentMenu("")]
    public class Quit : Command 
    {
        #region Public members

        public override void OnEnter()
        {
            Application.Quit();

            // 不支持的平台会继续下一条
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}