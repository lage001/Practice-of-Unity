using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 自定义Player脚本的面板
[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private Player _Component;

    private void OnEnable()
    {
        _Component = target as Player;

    }

    private void OnDisable()
    {
        _Component = null;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUILayout.LabelField("人物相关属性");

        _Component.ID = EditorGUILayout.IntField("玩家ID", _Component.ID);
    }
}
