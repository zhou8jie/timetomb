/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(DynamicGridLayoutGroup), true)]
    public class DynamicGridLayoutGroupEditor : Editor
    {
        SerializedProperty m_Padding;
        SerializedProperty m_CellSize;
        SerializedProperty m_Spacing;
        SerializedProperty m_StartCorner;
        SerializedProperty m_StartAxis;
        SerializedProperty m_ChildAlignment;
        SerializedProperty m_Constraint;
        SerializedProperty m_ConstraintCount;
        SerializedProperty m_PrefabObject;
        SerializedProperty m_PrecreateCount;
        SerializedProperty m_FitSizeOfChildren;
        SerializedProperty m_FitVisibleLength;
        SerializedProperty m_DeactivateInvisibles;

        protected virtual void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_CellSize = serializedObject.FindProperty("m_CellSize");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_Constraint = serializedObject.FindProperty("m_Constraint");
            m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");
            m_FitSizeOfChildren = serializedObject.FindProperty("m_FitSizeOfChildren");
            m_FitVisibleLength = serializedObject.FindProperty("m_FitVisibleLength");
            m_DeactivateInvisibles = serializedObject.FindProperty("m_DeactivateInvisibles");
            m_PrefabObject = serializedObject.FindProperty("m_prefabObject");
            m_PrecreateCount = serializedObject.FindProperty("m_prepreparedCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_CellSize, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            EditorGUILayout.PropertyField(m_Constraint, true);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_ConstraintCount, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(m_FitSizeOfChildren, true);
            EditorGUILayout.PropertyField(m_FitVisibleLength, true);
            EditorGUILayout.PropertyField(m_DeactivateInvisibles, true);
            EditorGUILayout.PropertyField(m_PrefabObject, true);
            EditorGUILayout.PropertyField(m_PrecreateCount, true);

            if (serializedObject.ApplyModifiedProperties())
            {
                (target as DataDrivenLayoutGroup).SetLayoutDirty();
            }

            if (EditorApplication.isPlaying)
            {
                DataDrivenLayoutGroup layoutGroup = target as DataDrivenLayoutGroup;
                if (GUILayout.Button("Test"))
                {
                    List<object> list = new List<object>();
                    int length = Mathf.CeilToInt(Random.Range(100, 200));
                    for (int i = 0; i < length; i++)
                    {
                        list.Add(null);
                    }
                    layoutGroup.layoutDatas = list;
                }
            }
        }
    }
}
