using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// �Զ���Player�ű������
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
        EditorGUILayout.LabelField("�����������");

        _Component.ID = EditorGUILayout.IntField("���ID", _Component.ID);
    }
}
