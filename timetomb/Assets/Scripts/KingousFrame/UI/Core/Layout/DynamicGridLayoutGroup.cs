/*
Copyright (c) 2014-2016 The uguiex Authors. All rights reserved.
Use of this source code is governed by a BSD-style license that can be
found in the LICENSE file.
*/
using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Dynamic Grid Layout Group", 152)]
    public class DynamicGridLayoutGroup : DataDrivenLayoutGroup
    {
        public enum Corner { UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3 }
        public enum Axis { Horizontal = 0, Vertical = 1 }
        public enum Constraint { FixedColumnCount = 0, FixedRowCount = 1 }

        [SerializeField]
        protected Corner m_StartCorner = Corner.UpperLeft;
        public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

        [SerializeField]
        protected Axis m_StartAxis = Axis.Horizontal;
        public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

        [SerializeField]
        protected Vector2 m_CellSize = new Vector2(100, 100);
        public Vector2 cellSize { get { return m_CellSize; } set { SetProperty(ref m_CellSize, value); } }

        [SerializeField]
        protected Vector2 m_Spacing = Vector2.zero;
        public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        [SerializeField]
        protected Constraint m_Constraint = Constraint.FixedColumnCount;
        public Constraint constraint { get { return m_Constraint; } set { SetProperty(ref m_Constraint, value); } }

        [SerializeField]
        protected int m_ConstraintCount = 2;
        public int constraintCount { get { return m_ConstraintCount; } set { SetProperty(ref m_ConstraintCount, value); } }

        protected int m_dynamicFirstVisibleNext = -1;
        private int m_dynamicFirstVisible = -1;
        protected int m_dynamicVisibleLengthNext = -1;
        private int m_dynamicVisibleLength = -1;//-1 - no limited

        private Bounds m_maskBounds = new Bounds();

        public int dynamicFirstVisible { set { m_dynamicFirstVisibleNext = Mathf.Max(0, Mathf.Min(layoutDatas.Count - 1, value - m_ConstraintCount)); SetLayoutDirty(); } get { return m_dynamicFirstVisibleNext; } }
        public int dynamicVisibleLength { set { m_dynamicVisibleLengthNext = value; SetLayoutDirty(); } get { return m_dynamicVisibleLengthNext; } }

        private Vector2 m_LastScrollPosition;
        private Vector2 m_LastScrollVelocity;
        private UpdateExecutor m_UpdateExecutor;

        protected DynamicGridLayoutGroup() { m_UpdateExecutor = new UpdateExecutor(DelaySetSells); }

        protected override void Start()
        {
            base.Start();
        }

        protected override void ClearAll()
        {
            base.ClearAll();

            m_dynamicFirstVisible = -1;
            m_dynamicFirstVisibleNext = -1;
            m_dynamicVisibleLength = -1;

            m_UpdateExecutor.Cancel();
        }

        public override void CalculateLayoutInputHorizontal()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            if (m_LayoutDirty)
            {
                if (scrollRect != null)
                {
                    m_LastScrollPosition = scrollRect.normalizedPosition;
                    m_LastScrollVelocity = scrollRect.velocity;
                }

                int minColumns = 0;
                int preferredColumns = 0;
                if (m_Constraint == Constraint.FixedColumnCount)
                {
                    minColumns = preferredColumns = m_ConstraintCount;
                }
                else if (m_Constraint == Constraint.FixedRowCount)
                {
                    minColumns = preferredColumns = Mathf.CeilToInt(layoutDatas.Count / (float)m_ConstraintCount - 0.001f);
                }
                else
                {
                    minColumns = 1;
                    preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(layoutDatas.Count));
                }

                if (maskAvailable)
                {
                    RectTransform scrollRectTransform = scrollRect.transform as RectTransform;
                    Vector3[] corners = new Vector3[4];
                    scrollRectTransform.GetWorldCorners(corners);
                    int count = corners.Length;
                    for (int i = 0; i < count; i++) { corners[i] = transform.InverseTransformPoint(corners[i]); }

                    Vector3 size = new Vector3(corners[2].x - corners[0].x, corners[2].y - corners[0].y, corners[2].z - corners[0].z);
                    m_maskBounds.center = new Vector3(corners[0].x + size.x * 0.5f, corners[0].y + size.y * 0.5f, corners[0].z + size.z * 0.5f);
                    m_maskBounds.size = size;

                    SetLayoutInputForCustomAxis(
                    Mathf.Max(padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x, m_maskBounds.size.x),
                    Mathf.Max(padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x, m_maskBounds.size.x),
                    -1, 0);
                }
                else
                {
                    SetLayoutInputForCustomAxis(
                    padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
                    padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
                    -1, 0);
                }
            }
        }

        public override void CalculateLayoutInputVertical()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            if (m_LayoutDirty)
            {
                int minRows = 0;
                if (m_Constraint == Constraint.FixedColumnCount)
                {
                    minRows = Mathf.CeilToInt(layoutDatas.Count / (float)m_ConstraintCount - 0.001f);
                }
                else if (m_Constraint == Constraint.FixedRowCount)
                {
                    minRows = m_ConstraintCount;
                }
                else
                {
                    float width = rectTransform.rect.size.x;
                    int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
                    minRows = Mathf.CeilToInt(layoutDatas.Count / (float)cellCountX);
                }

                float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
                if (maskAvailable) minSpace = Mathf.Max(m_maskBounds.size.y, minSpace);
                SetLayoutInputForCustomAxis(minSpace, minSpace, -1, 1);

                Vector2 size = rectTransform.rect.size;
                FitSizeOfChildren();

                if (scrollRect != null)
                {
                    if (size != rectTransform.rect.size)
                        scrollRect.normalizedPosition = m_LastScrollPosition;
                    scrollRect.velocity = m_LastScrollVelocity;
                }
            }
            m_LayoutDirty = false;
        }

        protected virtual void SetLayoutInputForCustomAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
        {
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
        }

        public override void SetLayoutHorizontal()
        {

        }

        public override void SetLayoutVertical()
        {
            m_UpdateExecutor.DelayUpdate();
        }

        protected override void OnScrollRectValueChanged(Vector2 arg0)
        {
            base.OnScrollRectValueChanged(arg0);

            SetCells();
        }

        bool DelaySetSells()
        {
            if (!IsActive()) return false;
            SetCells();
            return false;
        }

        protected override void SetDatas()
        {
            if (!IsActive()) return;
            if (this == null || gameObject == null) return;///TODO??
            if (!isActiveAndEnabled) return;
            SetLayoutDirty();
            CalculateLayoutInputHorizontal();
            CalculateLayoutInputVertical();
            SetCells();
            base.SetDatas();
            m_ScrollRectDirty = true;
            m_UpdateExecutor.Cancel();
        }

        private void SetCells()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;

            int cellCountX = 1;
            int cellCountY = 1;
            if (m_Constraint == Constraint.FixedColumnCount)
            {
                cellCountX = m_ConstraintCount;
                cellCountY = Mathf.CeilToInt(layoutDatas.Count / (float)cellCountX - 0.001f);
            }
            else if (m_Constraint == Constraint.FixedRowCount)
            {
                cellCountY = m_ConstraintCount;
                cellCountX = Mathf.CeilToInt(layoutDatas.Count / (float)cellCountY - 0.001f);
            }
            else
            {
                cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
                cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
            }

            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            if (startAxis == Axis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, layoutDatas.Count);
                actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(layoutDatas.Count / (float)cellsPerMainAxis));
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, layoutDatas.Count);
                actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(layoutDatas.Count / (float)cellsPerMainAxis));
            }

            Vector2 requiredSpace = new Vector2(
                    actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
                    actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
                    );
            Vector2 startOffset = new Vector2(
                    GetStartOffset(0, requiredSpace.x),
                    GetStartOffset(1, requiredSpace.y)
                    );

            if (m_ScrollRectDirty)
            {
                if (maskAvailable)
                {
                    RectTransform maskRectTrans = scrollRect.viewport.transform as RectTransform;

                    Vector3[] corners = new Vector3[4];
                    maskRectTrans.GetWorldCorners(corners);
                    int count = corners.Length;
                    for (int i = 0; i < count; i++) { corners[i] = transform.InverseTransformPoint(corners[i]); }

                    Vector3 size = new Vector3(corners[2].x - corners[0].x, corners[2].y - corners[0].y, corners[2].z - corners[0].z);
                    m_maskBounds.center = new Vector3(corners[0].x + size.x * 0.5f, corners[0].y + size.y * 0.5f, corners[0].z + size.z * 0.5f);
                    m_maskBounds.size = size;

                    if (m_FitVisibleLength)
                    {
                        bool fallback = false;
                        rectTransform.GetLocalCorners(corners);
                        switch (m_Constraint)
                        {
                            case Constraint.FixedColumnCount:
                                if (m_StartAxis != Axis.Horizontal) { fallback = true; break; }
                                m_dynamicFirstVisibleNext = Mathf.FloorToInt((corners[1].y - m_maskBounds.max.y - m_Padding.top) / (cellSize.y + spacing.y)) * constraintCount;
                                m_dynamicVisibleLengthNext = Mathf.CeilToInt(m_maskBounds.size.y / (cellSize.y + spacing.y) + 1) * constraintCount;
                                CheckVisibleNext(1);
                                break;
                            case Constraint.FixedRowCount:
                                if (m_StartAxis != Axis.Vertical) { fallback = true; break; }
                                m_dynamicFirstVisibleNext = Mathf.FloorToInt((m_maskBounds.min.x - corners[0].x - m_Padding.left) / (cellSize.x + spacing.x)) * constraintCount;
                                m_dynamicVisibleLengthNext = Mathf.CeilToInt(m_maskBounds.size.x / (cellSize.x + spacing.x) + 1) * constraintCount;
                                CheckVisibleNext(0);
                                break;
                        }
                        if (fallback)
                        {
                            m_dynamicVisibleLengthNext = -1;
                            m_dynamicFirstVisibleNext = -1;
                        }
                    }
                }
                m_ScrollRectDirty = false;
            }

            if (prefabItem != null)
            {
                if (m_DataDirty || ((Mathf.Abs(m_dynamicFirstVisible - m_dynamicFirstVisibleNext) > m_dynamicVisibleLength) && m_dynamicVisibleLength > 0))
                {
                    int childrenCount = m_LayoutChildren.Count;
                    for (int i = 0; i < childrenCount; i++) DestroyBehaviour(m_LayoutChildren[i]);
                    m_LayoutChildren.Clear();

                    m_dynamicFirstVisible = -1;
                }

                //RectTransform prefabSize = prefabItem.rectTransform;
                if (m_dynamicFirstVisible < 0 || m_dynamicFirstVisible != m_dynamicFirstVisibleNext || m_dynamicVisibleLengthNext != m_dynamicFirstVisible)
                {
                    m_dynamicVisibleLength = m_dynamicVisibleLengthNext;

                    //create ILayoutBehaviours
                    int toRemoveIndex = 0;
                    int lastVisible = Mathf.Max(0, Mathf.Min(m_dynamicFirstVisibleNext + (m_dynamicVisibleLength < 0 ? (layoutDatas.Count + m_ConstraintCount) : m_dynamicVisibleLength), layoutDatas.Count));
                    if (m_LayoutChildren.Count > 0)
                    {
                        while (m_dynamicFirstVisible >= 0 && m_dynamicFirstVisible < m_dynamicFirstVisibleNext)
                        {
                            m_dynamicFirstVisible++;
                            toRemoveIndex = 0;
                            DestroyBehaviour(m_LayoutChildren[toRemoveIndex]);
                            m_LayoutChildren.RemoveAt(toRemoveIndex);
                            if (m_LayoutChildren.Count == 0) m_dynamicFirstVisible = -1;
                        }
                    }

                    if (m_LayoutChildren.Count > 0)
                    {
                        while (m_dynamicFirstVisible >= 0 && (m_dynamicFirstVisible + m_LayoutChildren.Count) > lastVisible)
                        {
                            toRemoveIndex = m_LayoutChildren.Count - 1;
                            if (toRemoveIndex < 0) break;
                            DestroyBehaviour(m_LayoutChildren[toRemoveIndex]);
                            m_LayoutChildren.RemoveAt(toRemoveIndex);
                        }
                    }

                    ICell item = null;
                    List<object> datas = layoutDatas;

                    if (m_dynamicFirstVisible < 0)
                    {
                        m_dynamicFirstVisible = m_dynamicFirstVisibleNext;
                        if (m_dynamicFirstVisible < 0) { m_dynamicFirstVisible = 0; m_dynamicFirstVisibleNext = 0; }
                        int maxToAdd = Mathf.Min(datas.Count, lastVisible);

                        for (int i = m_dynamicFirstVisible; i < maxToAdd; i++)
                        {
                            item = CreateBehaviour(datas[i], i);
                            m_LayoutChildren.Add(item);
                            item.rectTransform.SetAsLastSibling();
                        }
                    }
                    else
                    {
                        while (m_dynamicFirstVisible > m_dynamicFirstVisibleNext && m_dynamicFirstVisible > 0)
                        {
                            m_dynamicFirstVisible--;
                            if (m_dynamicFirstVisible >= 0 && m_dynamicFirstVisible < datas.Count)
                            {
                                item = CreateBehaviour(datas[m_dynamicFirstVisible], m_dynamicFirstVisible);
                                m_LayoutChildren.Insert(0, item);
                                item.rectTransform.SetAsFirstSibling();
                            }
                        }

                        while (m_dynamicFirstVisible + m_LayoutChildren.Count < lastVisible)
                        {
                            item = CreateBehaviour(datas[m_dynamicFirstVisible + m_LayoutChildren.Count], m_dynamicFirstVisible + m_LayoutChildren.Count);
                            m_LayoutChildren.Add(item);
                            item.rectTransform.SetAsLastSibling();
                        }
                    }
                }
            }

            ICell child = null;
            MonoBehaviour childBehaviour = null;
            Vector3 pos = new Vector3();
            Bounds childBounds = new Bounds(Vector3.zero, new Vector3(cellSize.x + spacing.x, cellSize.y + spacing.y, 1));
            int childIndex = 0;

            for (int i = 0; i < m_LayoutChildren.Count; i++)
            {
                child = m_LayoutChildren[i];
                childBehaviour = child as MonoBehaviour;

                RectTransform rect = childBehaviour.transform as RectTransform;

                if (rect == null) { continue; }

                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = cellSize;

                if (childBehaviour == null || childBehaviour.gameObject == null) { continue; }

                childIndex = child.index;

                int positionX;
                int positionY;
                if (startAxis == Axis.Horizontal)
                {
                    positionX = childIndex % cellsPerMainAxis;
                    positionY = childIndex / cellsPerMainAxis;
                }
                else
                {
                    positionX = childIndex / cellsPerMainAxis;
                    positionY = childIndex % cellsPerMainAxis;
                }

                if (cornerX == 1)
                    positionX = actualCellCountX - 1 - positionX;
                if (cornerY == 1)
                    positionY = actualCellCountY - 1 - positionY;

                pos.x = startOffset.x + (cellSize[0] + spacing[0]) * positionX;
                pos.y = startOffset.y + (cellSize[1] + spacing[1]) * positionY;

                SetChildAlongCustomAxis(childIndex, 0, child.rectTransform, pos.x, cellSize[0]);
                SetChildAlongCustomAxis(childIndex, 1, child.rectTransform, pos.y, cellSize[1]);

                if (m_DeactivateInvisibles)
                {
                    childBounds.center = child.rectTransform.localPosition;
                    if (m_maskBounds.Intersects(childBounds))
                    {
                        if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (child.gameObject.activeSelf) child.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!child.gameObject.activeSelf)
                        child.gameObject.SetActive(true);
                }
            }

            m_DataDirty = false;
        }

        protected virtual void SetChildAlongCustomAxis(int index, int axis, RectTransform rectTransform, float position, float cellSize)
        {
            SetChildAlongAxis(rectTransform, axis, position, cellSize);
        }

        protected virtual void CheckVisibleNext(int axis)
        {

        }

        class UpdateExecutor : IUpdateElement
        {
            private System.Func<bool> _callback;

            public UpdateExecutor(System.Func<bool> callback) { _callback = callback; }

            public void DelayUpdate()
            {
                uUpdateRegistry.RegisterElement(this);
            }

            public void Cancel()
            {
                uUpdateRegistry.UnregisterElement(this);
            }

            public bool PerformUpdate()
            {
                return _callback();
            }
        }
    }
}
