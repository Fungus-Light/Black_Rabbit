
//-----------------------------------------------------------------------
namespace TiaanDotCom.Unity3D.EditorTools
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Implements menu items for the Unity Editor to perform
    /// end-of-line conversion and fix issues such as for the
    /// following: "There are inconsistent line endings in the
    /// 'Assets/.../*.cs' script. Some are Mac OS X (UNIX) and
    /// some are Windows. This might lead to incorrect line
    /// numbers in stacktraces and compiler errors."
    /// </summary>
    public static class LineEndingsEditMenu
    {
//        [MenuItem("Tools/Fungus/Utilities/EOL Conversion (Windows)")]
//        public static void ConvertLineEndingsToWindowsFormat()
//        {
//            ConvertLineEndings(false);
//        }

        [MenuItem("Tools/Black_Rabbit/功能/转换到Mac的行尾格式")]
        public static void ConvertLineEndingsToMacFormat()
        {
            ConvertLineEndings(true);
        }

        private static void ConvertLineEndings(bool isUnixFormat)
        {
            string title = string.Format(
                "转换到 {0} 格式",
                (isUnixFormat ? "UNIX" : "Windows"));
            if (!EditorUtility.DisplayDialog(
                title,
                "这个操作可能改变你项目中许多文件，" +
                "希望你做好备份！！ " +
                "要继续吗？",
                "Yes",
                "No"))
            {
                Debug.Log("操作取消");
                return;
            }

            var fileTypes = new string[]
            {
                "*.txt",
                "*.cs",
                "*.js",
                "*.boo",
                "*.compute",
                "*.shader",
                "*.cginc",
                "*.glsl",
                "*.xml",
                "*.xaml",
                "*.json",
                "*.inc",
                "*.css",
                "*.htm",
                "*.html",
            };

            string projectAssetsPath = Application.dataPath;
            int totalFileCount = 0;
            var changedFiles = new List<string>();
            var regex = new Regex(@"(?<!\r)\n");
            const string LineEnd = "\r\n";
            var comparisonType = System.StringComparison.Ordinal;
            foreach (string fileType in fileTypes)
            {
                string[] filenames = Directory.GetFiles(
                    projectAssetsPath,
                    fileType,
                    SearchOption.AllDirectories);
                totalFileCount += filenames.Length;
                foreach (string filename in filenames)
                {
                    string originalText = File.ReadAllText(filename);
                    string changedText;
                    changedText = regex.Replace(originalText, LineEnd);
                    if (isUnixFormat)
                    {
                        changedText =
                            changedText.Replace(LineEnd, "\n");
                    }

                    bool isTextIdentical = string.Equals(
                        changedText, originalText, comparisonType);
                    if (!isTextIdentical)
                    {
                        changedFiles.Add(filename);
                        File.WriteAllText(
                            filename,
                            changedText,
                            System.Text.Encoding.UTF8);
                    }
                }
            }

            int changedFileCount = changedFiles.Count;
            int skippedFileCount = (totalFileCount - changedFileCount);
            string message = string.Format(
                "跳过 {0} 个文件 " +
                "修改了 {1} 文件",
                skippedFileCount,
                changedFileCount);
            if (changedFileCount <= 0)
            {
                message += ".";
            }
            else
            {
                message += (":" + LineEnd);
                message += string.Join(LineEnd, changedFiles.ToArray());
            }

            Debug.Log(message);
            if (changedFileCount > 0)
            {
                // Recompile modified scripts.
                AssetDatabase.Refresh();
            }
        }
    }
}
