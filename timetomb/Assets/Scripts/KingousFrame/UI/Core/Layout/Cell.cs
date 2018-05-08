/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/

using System;
using UnityEngine.EventSystems;
namespace UnityEngine.UI
{
    public class Cell : MonoBehaviour, ICell
    {
        public int index { get; set; }
        public bool selected { get; private set; }
        public AbstractLayoutDirector layoutParent { get; set; }
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform ?? (m_rectTransform = transform as RectTransform); } }
        public object data { get; private set; }
        public virtual void SetData(object o)
        {
            data = o;
        }
        public virtual void SetReset()
        {

        }
        public virtual void SetSelected(bool v)
        {
            selected = v;
        }
        public void PostBehaviourEvent(object param)
        {
            layoutParent.PostBehaviourEvent(this, param);
        }
    }
}