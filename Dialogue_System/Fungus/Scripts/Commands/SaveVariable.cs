// fungus-light�޸ĺͱ�д

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// ����һ��Boolean, Integer, Float or String ʹ�ñ��ػ��־û��洢.
    /// </summary>
    [CommandInfo("�浵ϵͳ", 
                 "�������", 
                 "����һ��֧�ֵ����͵ı���")]
    [AddComponentMenu("")]
    public class SaveVariable : Command
    {
        [Tooltip("����ļ�ֵ")]
        [SerializeField] protected string key = "";
        
        [Tooltip("����ѡ�񣬽�֧��Bool��Int,Float,String")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable), 
                          typeof(FloatVariable), 
                          typeof(StringVariable),
                          typeof(Vector2Variable))]
        [SerializeField] protected Variable variable;

        #region Public members

        public override void OnEnter()
        {
            if (key == "" ||
                variable == null)
            {
                Continue();
                return;
            }
            
            var flowchart = GetFlowchart();
            
            string prefsKey = SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables(key);
            
            System.Type variableType = variable.GetType();

            if (variableType == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = variable as BooleanVariable;
                if (booleanVariable != null)
                {
                    //PlayerPrefs.SetInt(prefsKey, booleanVariable.Value ? 1 : 0);
                    SaveSystem.SetBool(prefsKey,booleanVariable.Value);
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                if (integerVariable != null)
                {
                    //PlayerPrefs.SetInt(prefsKey, integerVariable.Value);
                    SaveSystem.SetInt(prefsKey,integerVariable.Value);
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    //PlayerPrefs.SetFloat(prefsKey, floatVariable.Value);
                    SaveSystem.SetFloat(prefsKey,floatVariable.Value);
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                if (stringVariable != null)
                {
                    //PlayerPrefs.SetString(prefsKey, stringVariable.Value);
                    SaveSystem.SetString(prefsKey, stringVariable.Value);
                }
            }
            else if (variableType == typeof(Vector2Variable))
            {
                Vector2Variable vector2Variable = variable as Vector2Variable;
                if (vector2Variable != null)
                {
                    
                    SaveSystem.SetVector2(prefsKey, vector2Variable.Value);
                }
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            if (key.Length == 0)
            {
                return "Error: δѡ���ֵ";
            }
            
            if (variable == null)
            {
                return "Error: û��ѡ�񱣴�����";
            }
            
            return variable.Key + " ���浽 '" + key + "'";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable in_variable)
        {
            return this.variable == in_variable || base.HasReference(in_variable);
        }

        #endregion
        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(key, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}