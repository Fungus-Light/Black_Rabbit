using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Black_Rabbit;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Flowchart))]
    public class FlowchartEditor : Editor 
    {
        protected SerializedProperty descriptionProp;
        protected SerializedProperty colorCommandsProp;
        protected SerializedProperty hideComponentsProp;
        protected SerializedProperty stepPauseProp;
        protected SerializedProperty saveSelectionProp;
        protected SerializedProperty localizationIdProp;
        protected SerializedProperty variablesProp;
        protected SerializedProperty showLineNumbersProp;
        protected SerializedProperty hideCommandsProp;
        

        protected Texture2D addTexture;

        protected VariableListAdaptor variableListAdaptor;


        public static bool FlowchartDataStale { get; set; }

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            descriptionProp = serializedObject.FindProperty("description");
            colorCommandsProp = serializedObject.FindProperty("colorCommands");
            hideComponentsProp = serializedObject.FindProperty("hideComponents");
            stepPauseProp = serializedObject.FindProperty("stepPause");
            saveSelectionProp = serializedObject.FindProperty("saveSelection");
            localizationIdProp = serializedObject.FindProperty("localizationId");
            variablesProp = serializedObject.FindProperty("variables");
            showLineNumbersProp = serializedObject.FindProperty("showLineNumbers");
            hideCommandsProp = serializedObject.FindProperty("hideCommands");
            

            addTexture = FungusEditorResources.AddSmall;

            variableListAdaptor = new VariableListAdaptor(variablesProp, target as Flowchart);
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            var flowchart = target as Flowchart;

            flowchart.UpdateHideFlags();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(descriptionProp);
            EditorGUILayout.PropertyField(colorCommandsProp);
            EditorGUILayout.PropertyField(hideComponentsProp);
            EditorGUILayout.PropertyField(stepPauseProp);
            EditorGUILayout.PropertyField(saveSelectionProp);
            EditorGUILayout.PropertyField(localizationIdProp);
            EditorGUILayout.PropertyField(showLineNumbersProp);
            

            // Show list of commands to hide in Add Command menu
            //ReorderableListGUI.Title(new GUIContent(hideCommandsProp.displayName, hideCommandsProp.tooltip));
            //ReorderableListGUI.ListField(hideCommandsProp);
            EditorGUILayout.PropertyField(hideCommandsProp, new GUIContent(hideCommandsProp.displayName, hideCommandsProp.tooltip), true);

            if(EditorGUI.EndChangeCheck())
            {
                FlowchartDataStale = true;
            }


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(Config.isEnglish?"Open Flowchart Window":"打开流程块编辑器", Config.isEnglish ? "Opens the Flowchart Window":"打开流程对话块编辑器")))
            {
                EditorWindow.GetWindow(typeof(FlowchartWindow), false, "Flowchart");
            }


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            //Show the variables in the flowchart inspector
            GUILayout.Space(20);

            DrawVariablesGUI(false, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth) - VariableListAdaptor.ReorderListSkirts);

        }

        public virtual void DrawVariablesGUI(bool showVariableToggleButton, int w)
        {
            var t = target as Flowchart;

            if(t == null)
            {
                return;
            }

            serializedObject.Update();


            if (t.Variables.Count == 0)
            {
                t.VariablesExpanded = true;
                //showVariableToggleButton = true;
            }

            if (showVariableToggleButton && !t.VariablesExpanded)
            {
                if (GUILayout.Button ("Variables (" + t.Variables.Count + ")", GUILayout.Height(24)))
                {
                    t.VariablesExpanded = true;
                }

                // Draw disclosure triangle
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.x += 5;
                lastRect.y += 5;
                EditorGUI.Foldout(lastRect, false, "");
            }
            else
            {
                // Remove any null variables from the list
                // Can sometimes happen when upgrading to a new version of Fungus (if .meta GUID changes for a variable class)
                for (int i = t.Variables.Count - 1; i >= 0; i--)
                {
                    if (t.Variables[i] == null)
                    {
                        t.Variables.RemoveAt(i);
                    }
                }

                variableListAdaptor.DrawVarList(w);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static List<System.Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }
        
        public static List<System.Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                    .Where(t =>
                           t != derivedType &&
                           derivedType.IsAssignableFrom(t)
                           ).ToList();
            
        }

        /// <summary>
        /// When modifying custom editor code you can occasionally end up with orphaned editor instances.
        /// When this happens, you'll get a null exception error every time the scene serializes / deserialized.
        /// Once this situation occurs, the only way to fix it is to restart the Unity editor.
        /// As a workaround, this function detects if this editor is an orphan and deletes it. 
        /// </summary>
        protected virtual bool NullTargetCheck()
        {
            try
            {
                // The serializedObject accessor creates a new SerializedObject if needed.
                // However, this will fail with a null exception if the target object no longer exists.
                #pragma warning disable 0219
                SerializedObject so = serializedObject;
            }
            catch (System.NullReferenceException)
            {
                DestroyImmediate(this);
                return true;
            }
            
            return false;
        }
    }
}