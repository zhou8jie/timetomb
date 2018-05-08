/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using System;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Dynamic Layout Director", 152)]
    public class DynamicLayoutDirector : AbstractLayoutDirector
    {
        protected override void SetDatas()
        {
            m_DataDirty = false;

            ClearAll();

            int length = layoutDatas.Count;
            for (int i = 0; i < length; i++)
            {
                ICell child = CreateBehaviour(layoutDatas[i], i);
                m_LayoutChildren.Add(child);
                child.rectTransform.SetAsLastSibling();
            }
        }
    }
}
