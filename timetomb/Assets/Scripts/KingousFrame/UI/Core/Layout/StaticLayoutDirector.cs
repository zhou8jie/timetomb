/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Static Layout Director", 152)]
    public class StaticLayoutDirector : AbstractLayoutDirector
    {
        [Range(1, 10)]
        [SerializeField]
        private int m_searchDepth = 1;

        protected override void Awake()
        {
            base.Awake();

            m_LayoutChildren.Clear();
            SearchLayoutBehaviours(transform, 1, 0, m_LayoutChildren);
        }

        private int SearchLayoutBehaviours(Transform root, int depth, int index, List<ICell> result)
        {
            if (depth > m_searchDepth) return index;
            Transform child = null;
            int childCount = root.childCount;
            for (int i = 0; i < childCount; i++)
            {
                child = root.GetChild(i);
                ICell behaviour = child.GetComponent<ICell>();
                if (behaviour != null)
                {
                    m_LayoutChildren.Add(behaviour);
                    behaviour.index = index;
                    behaviour.layoutParent = this;
                    index++;
                }
                index = SearchLayoutBehaviours(child, depth + 1, index, result);
            }
            return index;
        }

        protected override void SetDatas()
        {
            m_DataDirty = false;

            int childCount = m_LayoutChildren.Count;
            int dataCount = layoutDatas.Count;
            ICell behaviour = null;
            object data = null;
            for (int i = 0; i < childCount; i++)
            {
                behaviour = m_LayoutChildren[i];
                behaviour.index = i;
                if (i >= 0 && i < dataCount)
                {
                    data = layoutDatas[i];
                    behaviour.SetData(data);
                }
                else
                {
                    behaviour.SetData(null);
                }
            }
        }
    }
}