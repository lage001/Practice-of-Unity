﻿using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(FTail_Animator2D))]
    [CanEditMultipleObjects]
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    public class FTail_Editor_2D : FTail_Editor_UI 
    {
        protected override void ConvertToV2(FTail_AnimatorBase oldTail, TailAnimator2 n)
        {
            base.ConvertToV2(oldTail, n);
            n.Axis2D = 3;
        }
    }

    [CustomEditor(typeof(FTail_AnimatorUI))]
    [CanEditMultipleObjects]
    public class FTail_Editor_UI : FTailAnimator_Editor
    {
        protected SerializedProperty sp_lock;

        protected override void ConvertToV2(FTail_AnimatorBase oldTail, TailAnimator2 n)
        {
            base.ConvertToV2(oldTail, n);
            n.Axis2D = 3;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            sp_lock = serializedObject.FindProperty("Lock2D");
        }

        public override void OnInspectorGUI()
        {
            drawAutoFixOption = false;
            base.OnInspectorGUI();
        }

        protected override void DrawTuningParameters(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 130;

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal(FGUI_Resources.BGInBoxStyle);
            drawTuningParams = EditorGUILayout.Foldout(drawTuningParams, "Tuning Parameters", true);

            FTail_AnimatorUI uiTail = tail as FTail_AnimatorUI;

            if (uiTail)
            {
                GUILayout.FlexibleSpace();
                EditorGUIUtility.labelWidth = 70;
                EditorGUILayout.PropertyField(sp_lock);
            }

            GUILayout.EndHorizontal();

            if (drawTuningParams)
            {
                GUILayout.Space(8f);
                DrawTuningParametersGUI(tail);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
            GUILayout.Space(1f);

            EditorGUIUtility.labelWidth = 0;
        }
    }
}