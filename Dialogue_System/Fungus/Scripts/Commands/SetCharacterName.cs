using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Narrative",
                 "设置角色名称",
                 "设置角色的名称为某个变量")]
public class SetCharacterName : Command
{

    
    [Tooltip("设置人物名称")]
    [SerializeField] protected Character character;
    

    [VariableProperty(typeof(StringVariable))]
    [SerializeField] protected Variable Name;

    public override void OnEnter()
    {
        character.SetStandardText((Name as StringVariable).Value);
        Continue();
    }

    public override string GetSummary()
    {
        string charactername = "未选择角色";
        if (character!=null)
        {
            charactername = character.name.ToString();
        }
        string namevalue = "未指定变量";
        if (Name!=null)
        {
            namevalue = (Name as StringVariable).Value;
        }

        return charactername+" 的名称将会设置为 "+namevalue;
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }
}
