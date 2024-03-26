using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using System.Collections.Generic;

public class MenuWindow : OdinMenuEditorWindow
{
    //打开窗口
    public static void OpenWindow()
    {
        var window = GetWindow<MenuWindow>();
        window.titleContent = new GUIContent("菜单类型窗口");
    }
    
    //设置菜单栏
    
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree()
        {
            { "第一栏", BaseType.Instance },
            { "第二栏", ButtonType.Instance },
            { "第三栏", TitleType.Instance },
            { "第四栏", Everything.Instance },

        };
        return tree;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

public class EditorWindow : OdinEditorWindow
{
    public static void OpenWindow()
    {
        var window = GetWindow<EditorWindow>();
        window.titleContent = new GUIContent("一般类型窗口");
    }
    
    //枚举类型，按钮显示，boxGroup分区
    [EnumToggleButtons, BoxGroup("Settings")]
    public ScaleMode ScaleMode;

    //字符串类型，boxGroup分区
    [FolderPath(RequireExistingPath = true), BoxGroup("Settings")]
    public string OutputPath;

    //列表 水平分区占一半
    [HorizontalGroup(0.5f)]
    public List<Texture> InputTextures;

    //纹理 水平分区占剩下的部分
    [HorizontalGroup, InlineEditor(InlineEditorModes.LargePreview)]
    public Texture Preview;

    //按钮
    [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
    public void PerformSomeAction()
    {

    }
}