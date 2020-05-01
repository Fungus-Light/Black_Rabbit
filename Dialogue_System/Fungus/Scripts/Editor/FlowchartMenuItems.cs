// Fungus-Light修改和编写

using UnityEngine;
using UnityEditor;
using Black_Rabbit;

namespace Fungus.EditorUtils
{
    public class FlowchartMenuItems
    {
        [UnityEditor.MenuItem(Config.isEnglish == true ? "Tools/Black_Rabbit/create/control/flowchart" : "Tools/Black_Rabbit/创建/控制/对话流", false, 0)]
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

        [MenuItem(Config.isEnglish == true ? "Tools/Black_Rabbit/create/BlackRabbit-Logo" : "Tools/Black_Rabbit/创建/BlackRabbit-Logo", false, 1000)]
        static void CreateMyLogo()
        {
            SpawnPrefab("Logo");
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