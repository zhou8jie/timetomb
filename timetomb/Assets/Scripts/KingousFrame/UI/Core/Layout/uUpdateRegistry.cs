using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    internal sealed class uUpdateRegistry
    {
        #region privates
        static uUpdateRegistry instance;

        private static uUpdateRegistry Get()
        {
            if (instance == null)
            {
                instance = new uUpdateRegistry();
                instance.elements.Clear();
                instance.elementsSwitch.Clear();
            }
            return instance;
        }

        private uUpdateRegistry()
        {
            Canvas.willRenderCanvases += PerformUpdate;
        }

        void PerformUpdate()
        {
            HashSet<IUpdateElement> currents = currentElements, nexts = nextElements;
            SwitchList();
            nexts.Clear();
            foreach (IUpdateElement element in currents)
            {
                if ((element as Object) == null) continue;
                if (element.PerformUpdate()) { nexts.Add(element); }
            }
            currents.Clear();
        }

        readonly HashSet<IUpdateElement> elements = new HashSet<IUpdateElement>();
        readonly HashSet<IUpdateElement> elementsSwitch = new HashSet<IUpdateElement>();
        bool useSwitch;

        void InternalRegisterElement(IUpdateElement element)
        {
            currentElements.Add(element);
        }

        void InternalUnregisterElement(IUpdateElement element)
        {
            currentElements.Remove(element);
        }

        void InternalUpdateImmediately()
        {
            PerformUpdate();
        }

        HashSet<IUpdateElement> currentElements { get { return useSwitch ? elementsSwitch : elements; } }

        HashSet<IUpdateElement> nextElements { get { return useSwitch ? elements : elementsSwitch; } }

        void SwitchList() { useSwitch = !useSwitch; }

        #endregion

        public static void RegisterElement(IUpdateElement element)
        {
            Get().InternalRegisterElement(element);
        }

        public static void UnregisterElement(IUpdateElement element)
        {
            Get().InternalUnregisterElement(element);
        }

        public static void UpdateImmediately()
        {
            Get().InternalUpdateImmediately();
        }

        public static bool Contains(IUpdateElement element)
        {
            return Get().currentElements.Contains(element);
        }
    }
}
