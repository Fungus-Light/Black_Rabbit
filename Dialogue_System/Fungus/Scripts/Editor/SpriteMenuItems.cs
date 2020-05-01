// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;

namespace Fungus.EditorUtils
{
    public class SpriteMenuItems 
    {
        //[MenuItem("Tools/Black_Rabbit/Fungus/创建/可点击图片", false, 150)]
        static void CreateClickableSprite()
        {
            FlowchartMenuItems.SpawnPrefab("ClickableSprite");
        }

        //[MenuItem("Tools/Black_Rabbit/Fungus/创建/可拖动图片", false, 151)]
        static void CreateDraggableSprite()
        {
            FlowchartMenuItems.SpawnPrefab("DraggableSprite");
        }

        //[MenuItem("Tools/Black_Rabbit/Fungus/创建/拖放目标图片", false, 152)]
        static void CreateDragTargetSprite()
        {
            FlowchartMenuItems.SpawnPrefab("DragTargetSprite");
        }

        //[MenuItem("Tools/Black_Rabbit/Fungus/创建/Parallax Sprite", false, 152)]
        static void CreateParallaxSprite()
        {
            FlowchartMenuItems.SpawnPrefab("ParallaxSprite");
        }
    }
}