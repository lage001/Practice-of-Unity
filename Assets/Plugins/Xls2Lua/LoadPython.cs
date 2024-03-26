using UnityEngine;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;


public class LoadPython
{
#if UNITY_EDITOR
    [MenuItem("Assets/Xls2Lua")]
    static void Xls2Lua()
    {
        var Select = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(Select);
        if (!string.IsNullOrEmpty(path) && Path.HasExtension(path))
        {
            RunPythonScript(path);
        }
        else
        {
            path = GetSelectedPathOrFallback();
            DirectoryInfo theFolder = new DirectoryInfo(path);
            foreach (var nextFile in theFolder.GetFiles("*.xls")) 
            {
                RunPythonScript(nextFile.FullName);
            }
            foreach (var nextFile in theFolder.GetFiles("*.xlsx")) 
            {
                RunPythonScript(nextFile.FullName);
            }
        }
        AssetDatabase.Refresh();
    }
    
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                //如果是目录获得目录名，如果是文件获得文件所在的目录名
                path = Path.GetDirectoryName(path);
                break;
            }
        }

        return path;
    }

    public static void RunPythonScript(string xls)
    {
        string pyFile = Application.dataPath + "/Plugins/Xls2Lua/xls2lua.py";
        string xlsFile = xls;
        string luaCodePath = Application.dataPath.Replace("Assets", "LuaCode");
        string output = luaCodePath + "/Game/Data/Config/"; 
        
        string sArguments = pyFile + " " + xlsFile + " " + output;
        Debug.Log("sArguments: "+ sArguments);

        var exePath = "";
        #if UNITY_EDITOR_OSX
        exePath = @"/Library/Frameworks/Python.framework/Versions/3.10/bin/python3";
        #else
        exePath = Application.dataPath + "/Plugins/Xls2Lua/WinPython/python-3.10.2.amd64/python.exe";
        #endif
        
        var ps = ProcessHelper.StartProcess(exePath, sArguments);
        ps.WaitForExit();
        
    }

#endif
}