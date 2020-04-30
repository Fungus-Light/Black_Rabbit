using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Black_Rabbit
{
    public class MenuItems
    {
        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/character/ThirdPerson") : ("Tools/Black_Rabbit/创建/角色/第三人称角色"), false, 100)]
        static void CreatePlayer()
        {
            CreateGameObject.SpawnPrefab("Players/Player");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/character/FirstPerson") : ("Tools/Black_Rabbit/创建/角色/第一人称角色"), false, 100)]
        static void CreateFPSPlayer()
        {
            CreateGameObject.SpawnPrefab("Players/FPS_Player");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/triggers/basic-trigger") : ("Tools/Black_Rabbit/创建/触发器/基本触发器"), false, 100)]
        private static void CreateTriggerBasic()
        {
            CreateGameObject.SpawnPrefab("Triggers/Trigger_basic");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/triggers/switch-trigger") : ("Tools/Black_Rabbit/创建/触发器/开关触发器"), false, 100)]
        private static void CreateTriggerSwitch()
        {
            CreateGameObject.SpawnPrefab("Triggers/Trigger_active");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/triggers/Timeline-trigger") : ("Tools/Black_Rabbit/创建/触发器/Timeline触发器"), false, 100)]
        static void CreateTriggerTimeline()
        {
            CreateGameObject.SpawnPrefab("Triggers/Trigger_timeline");
        }


        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/triggers/talkbable-trigger") : ("Tools/Black_Rabbit/创建/触发器/对话触发器"), false, 100)]
        static void CreateTriggerTalk()
        {
            CreateGameObject.SpawnPrefab("Triggers/Trigger_fungus");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/cameras/free look") : ("Tools/Black_Rabbit/创建/相机/自由摄像机"), false, 100)]
        static void CreateFreeLookCam()
        {
            CreateGameObject.SpawnPrefab("Camera/LookPlayer");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/create/UI/message ui") : ("Tools/Black_Rabbit/创建/UI/提示信息UI"), false, 100)]
        static void CreateMessageUI()
        {
            CreateGameObject.SpawnPrefab("UIs/MessageUI");
        }

        [MenuItem(Config.isEnglish == true ? ("Tools/Black_Rabbit/Utility/export Black_Rabbit package") : ("Tools/Black_Rabbit/功能/导出Black_Rabbit包"))]
        static void ExportBlackRabbitPackage()
        {
            string filename = "Black_Rabbit" + Config.version;
            string path = EditorUtility.SaveFilePanel(Config.isEnglish == true ? ("export Black_Rabbit package") : ("导出Black_Rabbit"), "", filename, "unitypackage");
            if (path.Length == 0)
            {
                return;
            }

            string[] folders = new string[] { "Assets/Black_Rabbit" };

            AssetDatabase.ExportPackage(folders, path, ExportPackageOptions.Recurse);
        }

    }
}


