// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    // The prefab names are prefixed with Fungus to avoid clashes with any other prefabs in the project
    public class NarrativeMenuItems 
    {

        [MenuItem("Tools/Fungus/Create/角色", false, 50)]
        static void CreateCharacter()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Character");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Fungus/Create/对话框", false, 51)]
        static void CreateSayDialog()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("SayDialog");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Fungus/Create/选项组", false, 52)]
        static void CreateMenuDialog()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("MenuDialog");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Fungus/Create/标签", false, 53)]
        static void CreateTag()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Tag");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Fungus/Create/Audio Tag", false, 54)]
        static void CreateAudioTag()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("AudioTag");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Fungus/Create/舞台", false, 55)]
        static void CreateStage()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Stage");
            go.transform.position = Vector3.zero;
        }
        
        [MenuItem("Tools/Fungus/Create/舞台位置", false, 56)]
        static void CreateStagePosition()
        {
            FlowchartMenuItems.SpawnPrefab("StagePosition");
        }

        [MenuItem("Tools/Fungus/Create/本地化物体", false, 57)]
        static void CreateLocalization()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Localization");
            go.transform.position = Vector3.zero;
        }
    }
}