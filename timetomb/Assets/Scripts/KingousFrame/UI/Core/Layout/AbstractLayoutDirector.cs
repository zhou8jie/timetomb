/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class AbstractLayoutDirector : UIBehaviour
    {
        public delegate void EventDelegate(ICell behaviour, object param);

        /// <summary>
        /// Invoked when method PostBehaviorEvent of a child object is called
        /// </summary>
        public EventDelegate onBehaviourEvent;

        #region backing fields
        /// <summary>
        /// GameObject that will used as prefab for children instantiating.
        /// </summary>
        [SerializeField]
        private GameObject m_prefabObject;
        private ICell m_prefab;

        /// <summary>
        /// Decide how many children gameObject will be pre-created when awake.
        /// </summary>
        [SerializeField]
        private int m_prepreparedCount = 5;

        /// <summary>
        /// Factory for object pooling, will also delay destroying instances after a period of time.
        /// </summary>
        CellFactory m_behaviourFactory;

        [NonSerialized]
        private RectTransform m_Rect;
        [NonSerialized]
        private ScrollRect m_ScrollRect = null;
        [NonSerialized]
        private bool m_ScrollRectInited = false;
        [NonSerialized]
        private Mask m_Mask = null;
        [NonSerialized]
        private RectMask2D m_RectMask = null;
        [NonSerialized]
        private bool m_MaskAvailable = false;

        [NonSerialized]
        private int m_selectedIndex = -1;
        [NonSerialized]
        private ICell m_selectedBehaviour;

        protected readonly List<object> m_LayoutDatas = new List<object>();
        protected readonly List<ICell> m_LayoutChildren = new List<ICell>();
        #endregion

        #region properties
        public ICell prefabItem { get { if (m_prefab == null && m_prefabObject != null) m_prefab = m_prefabObject.GetComponent<ICell>(); return m_prefab; } }
        public RectTransform rectTransform { get { if (m_Rect == null) m_Rect = GetComponent<RectTransform>(); return m_Rect; } }

        public bool maskAvailable { get { return m_MaskAvailable; } }

        public ScrollRect scrollRect
        {
            get
            {
                if (!m_ScrollRectInited)
                {
                    m_ScrollRect = GetComponentInParent<ScrollRect>();
                    if (m_ScrollRect != null)
                    {
                        m_Mask = m_ScrollRect.viewport.GetComponent<Mask>();
                        m_RectMask = m_ScrollRect.viewport.GetComponent<RectMask2D>();
                    }
                    m_MaskAvailable = m_Mask != null || m_RectMask != null;
                    m_ScrollRectInited = true;
                }
                return m_ScrollRect;
            }
        }

        public List<ICell> layoutChildren { get { return m_LayoutChildren; } }

        public int selectedIndex
        {
            get
            {
                if (m_selectedBehaviour != null && m_selectedBehaviour.index != m_selectedIndex)
                    m_selectedIndex = m_selectedBehaviour.index;
                return m_selectedIndex;
            }
            set
            {
                if (m_selectedIndex != value)
                {
                    if (m_selectedBehaviour != null) { m_selectedBehaviour.SetSelected(false); }
                    m_selectedIndex = value;
                    m_selectedBehaviour = GetBehaviourByIndex(m_selectedIndex);
                    if (m_selectedBehaviour != null) { m_selectedBehaviour.SetSelected(true); }
                }
            }
        }

        public ICell selectedItem
        {
            get
            {
                return m_selectedBehaviour;
            }
            set
            {
                if (value != null && m_LayoutChildren.Contains(value))
                {
                    m_selectedBehaviour = value;
                    m_selectedIndex = value.index;
                }
                else
                {
                    m_selectedBehaviour = null;
                    m_selectedIndex = -1;
                }
            }
        }

        private CellFactory behaviourFactory { get { if (m_behaviourFactory == null) InitBehaviourFactory(); return m_behaviourFactory; } }
        #endregion

        protected bool m_DataDirty = true;
        protected bool m_LayoutDirty = true;

        public List<object> layoutDatas
        {
            get { return m_LayoutDatas; }
            set
            {
                m_LayoutDatas.Clear();
                if (value != null)
                    m_LayoutDatas.AddRange(value);
                m_DataDirty = true;
                SetDatas();
            }
        }

        /// <summary>
        /// Destroy all existing children.
        /// </summary>
        protected virtual void ClearAll()
        {
            int childrenCount = m_LayoutChildren.Count;
            for (int i = 0; i < childrenCount; i++)
            {
                try
                {
                    ICell layoutBehaviour = m_LayoutChildren[i];
                    if (layoutBehaviour != null && layoutBehaviour.gameObject != null)
                    {
                        DestroyBehaviour(layoutBehaviour);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
            m_LayoutChildren.Clear();
        }

        /// <summary>
        /// Force rebuild all.
        /// </summary>
        public void Refresh()
        {
            layoutDatas = layoutDatas;
        }

        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            SetLayoutDirty();
        }

        public virtual void SetLayoutDirty()
        {
            m_LayoutDirty = true;
        }

        protected virtual void SetDatas()
        {

        }

        protected override void Awake()
        {
            base.Awake();

            if (prefabItem != null)
            {
                if (m_behaviourFactory == null)
                    InitBehaviourFactory();
            }

            m_ScrollRectInited = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetLayoutDirty();
        }

        public void PostBehaviourEvent(ICell behaviour, object param)
        {
            if (onBehaviourEvent != null)
                onBehaviourEvent(behaviour, param);
        }

        /// <summary>
        /// Search for ILayoutBehaviour by data that filled. May return incorrect result when type of data is struct value or repeated reference is added.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ICell GetBehaviour(object data)
        {
            return m_LayoutChildren.Find((ICell behaviour) => behaviour.data == data && data != null);
        }

        /// <summary>
        /// Search for ILayoutBehaviour by index, may return null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ICell GetBehaviourByIndex(int index)
        {
            return m_LayoutChildren.Find((ICell behaviour) => behaviour.index == index && index >= 0);
        }

        #region factory
        private void InitBehaviourFactory()
        {
            m_behaviourFactory = new CellFactory(this, this.rectTransform, prefabItem, m_prepreparedCount);
        }

        protected ICell CreateBehaviour(object data, int index)
        {
            ICell result = behaviourFactory.Get(data, index);
            if (index == m_selectedIndex)
            {
                m_selectedBehaviour = result;
                m_selectedBehaviour.SetSelected(true);
            }
            return result;
        }

        protected void DestroyBehaviour(ICell behaviour)
        {
            behaviourFactory.Release(behaviour);

            if (behaviour.selected)
            {
                if (m_selectedBehaviour != null && m_selectedBehaviour.index != m_selectedIndex)
                {
                    m_selectedIndex = m_selectedBehaviour.index;
                    m_selectedBehaviour.SetSelected(false);
                    m_selectedBehaviour = null;
                }
                behaviour.SetSelected(false);
                behaviour = null;
            }
        }

        protected string GetBehaviourName(int index)
        {
            return behaviourFactory.GetBehaviourName(index);
        }
        #endregion
    }
}
