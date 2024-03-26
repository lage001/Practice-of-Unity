using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos;
using System;
using Unity.VisualScripting.FullSerializer;

[Serializable]
public class ThemeTempInfo
{
    [TableColumnWidth(120, Resizable = false)]
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    [LabelText("缩略图")]
    //[OnClickMethod("OnIconClick")]
    public GameObject icon;
        
    [ReadOnly]
    [LabelText("资源名")]
    public string name;

    private Action<string> _onClickListener;

    public ThemeTempInfo(GameObject obj, Action<string> onClickListener)
    {
        icon = obj;
        name = obj.name;
        _onClickListener = onClickListener;
    }

    private void OnIconClick()
    {
        Debug.Log("我被点击了");
    }
}
