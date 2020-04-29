using UnityEngine;
using System.Collections;

public class SaveSystemSetup : MonoBehaviour
{

    [SerializeField] private string fileName = "gamedata.save"; // 要保存的文件名
    [SerializeField] private bool dontDestroyOnLoad = false; // 是否跨场景

    void Awake()
    {
        SaveSystem.Initialize(fileName);
        if (dontDestroyOnLoad) DontDestroyOnLoad(transform.gameObject);
    }

    // 退出时保存
    void OnApplicationQuit()
    {
        SaveSystem.SaveToDisk();
    }
}
