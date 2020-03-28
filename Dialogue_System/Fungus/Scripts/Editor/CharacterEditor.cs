// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Character))]
    public class CharacterEditor : Editor
    {
        protected SerializedProperty nameTextProp;
        protected SerializedProperty nameColorProp;
        protected SerializedProperty soundEffectProp;
        protected SerializedProperty portraitsProp;
        protected SerializedProperty portraitsFaceProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty setSayDialogProp;

        protected virtual void OnEnable()
        {
            nameTextProp = serializedObject.FindProperty ("nameText");
            nameColorProp = serializedObject.FindProperty ("nameColor");
            soundEffectProp = serializedObject.FindProperty ("soundEffect");
            portraitsProp = serializedObject.FindProperty ("portraits");
            portraitsFaceProp = serializedObject.FindProperty ("portraitsFace");
            descriptionProp = serializedObject.FindProperty ("description");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            Character t = target as Character;

            EditorGUILayout.PropertyField(nameTextProp, new GUIContent("名字", "显示在对话框上的名字"));
            EditorGUILayout.PropertyField(nameColorProp, new GUIContent("名字颜色", "对话框上名字颜色"));
            EditorGUILayout.PropertyField(soundEffectProp, new GUIContent("声音", "角色说话时默认声音 ，在对话中可以被覆盖"));
            EditorGUILayout.PropertyField(setSayDialogProp, new GUIContent("对话框", "角色说话时默认对话框，在对话中可以被覆盖"));
            EditorGUILayout.PropertyField(descriptionProp, new GUIContent("描述", "角色的描述备注等"));

            if (t.Portraits != null &&
                t.Portraits.Count > 0)
            {
                t.ProfileSprite = t.Portraits[0];
            }
            else
            {
                t.ProfileSprite = null;
            }
            
            if (t.ProfileSprite != null)
            {
                Texture2D characterTexture = t.ProfileSprite.texture;
                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                if (characterTexture != null)
                    GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
            }

            EditorGUILayout.PropertyField(portraitsProp, new GUIContent("立绘", "对话时显示的立绘"), true);

            EditorGUILayout.HelpBox("所有立绘都应该使用相同分辨率", MessageType.Info);

            EditorGUILayout.Separator();

            string[] facingArrows = new string[]
            {
                "FRONT",
                "<--",
                "-->",
            };
            portraitsFaceProp.enumValueIndex = EditorGUILayout.Popup("立绘朝向", (int)portraitsFaceProp.enumValueIndex, facingArrows);

            EditorGUILayout.Separator();

            EditorUtility.SetDirty(t);

            serializedObject.ApplyModifiedProperties();
        }

    }
}