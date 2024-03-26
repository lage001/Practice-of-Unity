using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;

public class BaseType : GlobalConfig<BaseType>
{
    [LabelText("文本")] public string StringType = "";

    [LabelText("数值")] public float FloatType = -1;

    [LabelText("矩阵")] public float2x2 Float2X2Type;

    [LabelText("布尔值")] public bool BootType;
}

public class ButtonType : GlobalConfig<ButtonType>
{
    [Button("一个按钮")]
    public void OnClickButton()
    {
        Debug.Log("Do something");
    }
}

public class TitleType : GlobalConfig<TitleType>
{
    [Title("一个标题")] public string wenben;
    [Title("一个对象")] public GameObject obj;
}
public class Everything : GlobalConfig<Everything>
{
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
    
    // 特殊类列表
    [TableList]
    [LabelText("主题模板列表")]
    public List<ThemeTempInfo> TemplateList;
    
    //按钮
    [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
    public void PerformSomeAction()
    {

    }
}
