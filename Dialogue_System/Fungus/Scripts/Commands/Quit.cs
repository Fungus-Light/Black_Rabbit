// fungus-light�޸ı�д

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// �˳�Ӧ�ã��ڱ༭����webplayer��Ч
    /// </summary>
    [CommandInfo("Flow", 
                 "�˳�Ӧ��",
                 "�˳�Ӧ�ã��ڱ༭����webplayer��Ч")]
    [AddComponentMenu("")]
    public class Quit : Command 
    {
        #region Public members

        public override void OnEnter()
        {
            Application.Quit();

            // ��֧�ֵ�ƽ̨�������һ��
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}