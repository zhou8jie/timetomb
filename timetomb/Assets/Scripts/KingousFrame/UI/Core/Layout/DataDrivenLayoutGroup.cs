/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    public abstract class DataDrivenLayoutGroup : AbstractLayoutDirector, ILayoutElement, ILayoutGroup
    {
        [SerializeField]
        protected RectOffset m_Padding = new RectOffset();
        public RectOffset padding { get { return m_Padding; } set { SetProperty(ref m_Padding, value); } }

        [FormerlySerializedAs("m_Alignment")]
        [SerializeField]
        protected TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;
        public TextAnchor childAlignment { get { return m_ChildAlignment; } set { SetProperty(ref m_ChildAlignment, value); } }

        [SerializeField]
        private FitSizeAxis m_FitSizeOfChildren = FitSizeAxis.Both;
        public FitSizeAxis fitSizeOfChildren { get { return m_FitSizeOfChildren; } set { SetProperty(ref m_FitSizeOfChildren, value); } }

        public enum FitSizeAxis { None = 0, HorizonalOnly = 1, VerticalOnly = 2, Both = 3 }

        [SerializeField]
        protected bool m_FitVisibleLength;

        [SerializeField]
        protected bool m_DeactivateInvisibles;

        private Vector2 m_ScrollRectPosition = new Vector2(0, 1);
        public Vector2 scrollRectPosition { get { return m_ScrollRectPosition; } set { m_ScrollRectPosition = value; m_ScrollRectPosDirty = true; } }
        private bool m_ScrollRectPosDirty = true;

        protected DrivenRectTransformTracker m_Tracker;

        private Vector2 m_TotalMinSize = Vector2.zero;
        private Vector2 m_TotalPreferredSize = Vector2.zero;
        private Vector2 m_TotalFlexibleSize = Vector2.zero;

        protected bool m_PropertyDirty = false;
        protected bool m_ScrollRectDirty = false;

        protected override void Awake()
        {
            base.Awake();

            if (scrollRect != null)
            {
                m_ScrollRectDirty = maskAvailable;
            }

            SetLayoutDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            }
        }

        protected virtual void OnScrollRectValueChanged(Vector2 arg0)
        {
            m_ScrollRectDirty = true;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            SetLayoutDirty();
        }

        // ILayoutElement Interface
        public virtual void CalculateLayoutInputHorizontal()
        {
            m_LayoutChildren.Clear();
            m_Tracker.Clear();
        }

        public abstract void CalculateLayoutInputVertical();
        public virtual float minWidth { get { return GetTotalMinSize(0); } }
        public virtual float preferredWidth { get { return GetTotalPreferredSize(0); } }
        public virtual float flexibleWidth { get { return GetTotalFlexibleSize(0); } }
        public virtual float minHeight { get { return GetTotalMinSize(1); } }
        public virtual float preferredHeight { get { return GetTotalPreferredSize(1); } }
        public virtual float flexibleHeight { get { return GetTotalFlexibleSize(1); } }
        public virtual int layoutPriority { get { return 0; } }

        public virtual void SetLayoutHorizontal() { }
        public virtual void SetLayoutVertical() { }

        protected DataDrivenLayoutGroup()
        {
            if (m_Padding == null)
                m_Padding = new RectOffset();
        }

        #region Unity Lifetime calls

        protected override void OnDisable()
        {
            base.OnDisable();

            if (scrollRect != null)
            {
                scrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
            }
            SetLayoutDirty();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();

            SetLayoutDirty();
        }

        #endregion

        protected override void ClearAll()
        {
            m_Tracker.Clear();
            base.ClearAll();
        }

        protected float GetTotalMinSize(int axis)
        {
            return m_TotalMinSize[axis];
        }

        protected float GetTotalPreferredSize(int axis)
        {
            return m_TotalPreferredSize[axis];
        }

        protected float GetTotalFlexibleSize(int axis)
        {
            return m_TotalFlexibleSize[axis];
        }

        protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);
            float availableSpace = rectTransform.rect.size[axis];
            float surplusSpace = availableSpace - requiredSpace;
            float alignmentOnAxis = 0;
            if (axis == 0)
                alignmentOnAxis = ((int)childAlignment % 3) * 0.5f;
            else
                alignmentOnAxis = ((int)childAlignment / 3) * 0.5f;
            return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
        }

        protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
        {
            m_TotalMinSize[axis] = totalMin;
            m_TotalPreferredSize[axis] = totalPreferred;
            m_TotalFlexibleSize[axis] = totalFlexible;
        }

        protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
        {
            if (rect == null)
                return;

            m_Tracker.Add(this, rect,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.SizeDelta);

            rect.SetInsetAndSizeFromParentEdge(axis == 0 ? RectTransform.Edge.Left : RectTransform.Edge.Top, pos, size);
        }

        private bool isRootLayoutGroup
        {
            get
            {
                Transform parent = transform.parent;
                if (parent == null)
                    return true;
                return transform.parent.GetComponent(typeof(ILayoutGroup)) == null;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (isRootLayoutGroup)
                SetLayoutDirty();
        }

        protected virtual void FitSizeOfChildren()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
            FitSizeAxis axis = m_FitSizeOfChildren;
            if (((int)axis & 1) != 0)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
            if (((int)axis & 2) != 0)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
        }

        public override void SetLayoutDirty()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
            if (!IsActive()) return;

            base.SetLayoutDirty();

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }


        protected override void SetDatas()
        {
            if (!IsActive()) return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetLayoutDirty();
        }
#endif
    }
}
