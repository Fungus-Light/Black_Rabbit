// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Loads a saved value and stores it in a Boolean, Integer, Float or String variable. If the key is not found then the variable is not modified.
    /// </summary>
    [CommandInfo("存档系统",
                 "读取变量", 
                 "读取一个变量")]
    [AddComponentMenu("")]
    public class LoadVariable : Command
    {
        [Tooltip("键值")]
        [SerializeField] protected string key = "";

        [Tooltip("变量选择，仅支持Bool，Int,Float,String")]
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

            // Prepend the current save profile (if any)
            string prefsKey = SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables(key);

            System.Type variableType = variable.GetType();

            if (variableType == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = variable as BooleanVariable;
                if (booleanVariable != null)
                {
                    booleanVariable.Value = SaveSystem.GetBool(prefsKey);
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                if (integerVariable != null)
                {
                    integerVariable.Value = SaveSystem.GetInt(prefsKey);
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    //floatVariable.Value = PlayerPrefs.GetFloat(prefsKey);
                    floatVariable.Value = SaveSystem.GetFloat(prefsKey);
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                if (stringVariable != null)
                {
                    stringVariable.Value = SaveSystem.GetString(prefsKey);
                }
            }
            else if (variableType == typeof(Vector2Variable))
            {
                Vector2Variable vector2Variable = variable as Vector2Variable;
                if (vector2Variable != null)
                {
                    vector2Variable.Value = SaveSystem.GetVector2(prefsKey);
                }
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            if (key.Length == 0)
            {
                return "Error: 未选择键值";
            }
        
            if (variable == null)
            {
                return "Error: 未选择变量";
            }

            return "'" + key + "' 读取到 " + variable.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable in_variable)
        {
            return this.variable == in_variable ||
                base.HasReference(in_variable);
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