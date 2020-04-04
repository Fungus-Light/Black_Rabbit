using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Black_Rabbit
{
    public static class CreateGameObject
    {
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

