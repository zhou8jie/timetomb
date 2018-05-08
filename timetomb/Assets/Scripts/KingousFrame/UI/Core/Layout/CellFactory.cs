/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    internal class CellFactory : IUpdateElement
    {
        private AbstractLayoutDirector m_layoutParent;
        private RectTransform m_container;
        private ICell m_prefab;
        private string m_prefabName;
        public string prefabName { get { return m_prefabName; } }
        private readonly Queue<ICell> m_cached = new Queue<ICell>();
        private float m_updateTime;
        private int m_preparedCount;
        private int m_createdCount;

        public CellFactory(AbstractLayoutDirector layoutGroup, RectTransform container, ICell prefab, int precompredCount)
        {
            m_layoutParent = layoutGroup;
            m_container = container;
            m_prefab = prefab;
            m_prefabName = m_prefab.gameObject.name + "({0})";
            m_preparedCount = precompredCount;
            if (m_prefab != null)
            {
                m_prefab.gameObject.SetActive(false);
            }

#if !UNITY_EDITOR
            PrecomparePopularItems(precompredCount);
#endif
        }

        private void AddChild(RectTransform container, RectTransform children)
        {
            children.SetParent(container);
            children.localScale = Vector3.one;
            children.localPosition = Vector3.zero;
        }

        protected void PrecomparePopularItems(int count)
        {
            int i = 0;
            ICell item = null;
            while (i < count)
            {
                item = GameObject.Instantiate(m_prefab as Component).GetComponent<ICell>();
                AddChild(m_container, item.rectTransform);
                item.gameObject.SetActive(false);
                m_cached.Enqueue(item);
                i++;
            }
        }

        public ICell Get(object data, int index)
        {
            ICell result = null;
            if (m_cached.Count > 0)
            {
                result = m_cached.Dequeue();
            }
            else
            {
                result = GameObject.Instantiate(m_prefab as Component).GetComponent<ICell>();
                AddChild(m_container, result.rectTransform);
            }
            result.layoutParent = m_layoutParent;
            result.rectTransform.localScale = Vector3.one;
            result.index = index;
            result.gameObject.name = GetBehaviourName(index);
            result.gameObject.SetActive(true);
            result.SetData(data);
            m_createdCount++;
            return result;
        }

        public string GetBehaviourName(int index)
        {
            return string.Format(m_prefabName, index);
        }

        public void Release(ICell behaviour)
        {
            m_cached.Enqueue(behaviour);
            behaviour.gameObject.SetActive(false);
            m_createdCount--;
            DelayDestroyPooledItems();
        }

        protected void DelayDestroyPooledItems()
        {
            if (m_layoutParent.IsActive())
            {
                m_updateTime = Time.realtimeSinceStartup + 5;
                uUpdateRegistry.RegisterElement(this);
            }
        }

        public bool PerformUpdate()
        {
            if (Time.realtimeSinceStartup > m_updateTime)
            {
                if (m_cached.Count == 0) return false;
                GameObject go = null;
                while (m_cached.Count > 0 && m_cached.Count + m_createdCount > m_preparedCount)
                {
                    ICell b = m_cached.Dequeue();
                    if ((b as MonoBehaviour) == null) { continue; }
                    go = b.gameObject;
                    if (go != null)
                        Object.Destroy(go);
                }
                return false;
            }
            return true;
        }
    }
}
