using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    public class CameraMenuItems 
    {
        [MenuItem("Tools/Black_Rabbit/创建/对话/摄像机视图", false, 100)]
        static void CreateView()
        {
            FlowchartMenuItems.SpawnPrefab("View");
        }
    }
}