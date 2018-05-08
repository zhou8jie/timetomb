/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/

using UnityEngine.EventSystems;
namespace UnityEngine.UI
{
    public interface ICell
    {
        object data { get; }
        bool selected { get; }
        int index { get; set; }
        AbstractLayoutDirector layoutParent { get; set; }
        GameObject gameObject { get; }
        RectTransform rectTransform { get; }
        void SetData(object o);
        void SetSelected(bool v);
        void PostBehaviourEvent(object param);
    }
}