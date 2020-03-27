// Fungus-Light修改和编写

﻿using UnityEngine;
using UnityEditor;
using Black_Rabbit;

namespace Fungus.EditorUtils
{
    public class FlowchartMenuItems
    {
        [MenuItem("Tools/Fungus/Create/对话流", false, 0)]
        static void CreateFlowchart()
        {
            GameObject go = SpawnPrefab("Flowchart");
            go.transform.position = Vector3.zero;

            // This is the latest version of Flowchart, so no need to update.
            var flowchart = go.GetComponent<Flowchart>();
            if (flowchart != null)
            {
                flowchart.Version = FungusConstants.CurrentVersion;
            }

            // Only the first created Flowchart in the scene should have a default GameStarted block
            if (GameObject.FindObjectsOfType<Flowchart>().Length > 1)
            {
                var block = go.GetComponent<Block>();
                GameObject.DestroyImmediate(block._EventHandler);
                block._EventHandler = null;
            }
        }

        //[MenuItem("Tools/Fungus/Create/Fungus Logo", false, 1000)]
        //static void CreateFungusLogo()
        //{
        //    SpawnPrefab("FungusLogo");
        //}

        [MenuItem("Tools/Fungus/Utilities/导出Black_Rabbit包")]
        static void ExportFungusPackage()
        {
            string filename = "Black_Rabbit"+Config.version;
            string path = EditorUtility.SaveFilePanel("导出Black_Rabbit", "",filename , "unitypackage");           
            if(path.Length == 0) 
            {
                return;
            }

            string[] folders = new string[] { "Assets/Black_Rabbit" };

            AssetDatabase.ExportPackage(folders, path, ExportPackageOptions.Recurse);
        }
            
        public static GameObject SpawnPrefab(string prefabName)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
            if (prefab == null)
            {
                return null;
            }

            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = prefab.name;

            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                Camera sceneCam = view.camera;
                Vector3 pos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                pos.z = 0f;
                go.transform.position = pos;
            }

            Selection.activeGameObject = go;
            
            Undo.RegisterCreatedObjectUndo(go, "Create Object");

            return go;
        }
    }
}