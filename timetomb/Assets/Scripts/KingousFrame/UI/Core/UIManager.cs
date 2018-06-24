using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private static UIManager inst;
    public static UIManager get()
    {
        return inst ?? (inst = new UIManager());
    }

    public UIRoot root { get { return _root; } }

    private const int ORDER_STEP = 10;

    private List<Layer> _layers = new List<Layer>();
    private Dictionary<string, UILayerId> _name2layerDic = new Dictionary<string, UILayerId>();
    private UIRoot _root;

    private UIManager()
    {
        var rootObj = GameObject.Find("UIRoot");
        if (rootObj == null)
        {
            rootObj = Object.Instantiate(Resources.Load<GameObject>("UI/Root"));
            var trans = rootObj.transform;
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }
        _root = rootObj.GetComponent<UIRoot>();
        _root.gameObject.name = "UIRoot";

        _layers.Add(new Layer() { id = UILayerId.Hud, order = 0 });
        _layers.Add(new Layer() { id = UILayerId.Panel, order = 10000 });
        _layers.Add(new Layer() { id = UILayerId.Pop, order = 20000 });
        _layers.Add(new Layer() { id = UILayerId.Top, order = 30000 });

        for (int i = 0, len = _layers.Count; i < len; i++)
        {
            var layer = _layers[i];
            var go = new GameObject("layer-" + layer.id.ToString());

            var trans = go.AddComponent<RectTransform>();
            trans.SetParent(_root.canvas.transform, false);
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;

            trans.anchoredPosition = Vector2.zero;
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.sizeDelta = Vector2.zero;

            layer.root = go;
        }
    }

    private Layer getLayer(UILayerId id)
    {
        return _layers[(int)id];
    }

    public void init()
    {
    }

    public UIBase add(UIBase ui)
    {
        var layer = getLayer(ui.layerId);
        var cur = layer.find(ui.name);
        if (cur != null)
        {
            Debug.LogError("invalid ui instance found: " + ui);
            return null;
        }
        var go = ui.gameObject;
        var trans = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        trans.SetParent(layer.root.transform, false);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;

        trans.anchoredPosition = Vector2.zero;
        trans.anchorMin = Vector2.zero;
        trans.anchorMax = Vector2.one;
        trans.sizeDelta = Vector2.zero;

        ui.initialize();

        var curCount = layer.list.Count;
        var last = curCount == 0 ? null : layer.list[curCount - 1];

        var canvas = ui.canvas;
        canvas.overrideSorting = true;
        canvas.sortingOrder = last == null ? layer.order : (last.canvas.sortingOrder + ORDER_STEP);

        var raycaster = ui.raycaster;

        layer.list.Add(ui);

        return ui;
    }

    public void remove(UIBase ui)
    {
        var layer = getLayer(ui.layerId);
        layer.list.Remove(ui);
        ui.gameObject.SetActive(false);
    }

    public UIBase find(string name)
    {
        UILayerId id = UILayerId.Hud;
        if (!_name2layerDic.TryGetValue(name, out id))
            return null;
        var layer = _layers[(int)id];
        return layer.find(name);
    }

    public T find<T>(string name) where T : UIBase
    {
        return find(name) as T;
    }

    public UIBase addHUD(string name, Vector3 worldPos)
    {
        var prefab = Resources.Load<GameObject>("UI/" + name);
        var go = Object.Instantiate(prefab);
        var ui = go.GetComponent<UIBase>();

        go.name = name;
        ui.show(false);

        var layer = getLayer(ui.layerId);
        var trans = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        trans.SetParent(layer.root.transform, false);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;

        trans.anchoredPosition = Vector2.zero;
        trans.anchorMin = Vector2.zero;
        trans.anchorMax = Vector2.one;
        trans.sizeDelta = Vector2.zero;

        Camera cam = CameraController.Instance().cam;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        var uiCame = UIManager.get().root.camera;
        Vector3 uiPos = uiCame.ScreenToWorldPoint(screenPos);

        trans.position = uiPos;

        ui.initialize();
        return ui;
    }
    
    public UIBase show(string name, float during=0)
    {
        var cur = find(name);
        if (cur != null)
        {
            if (!cur.isShow)
                cur.show();
            return cur;
        }
        var prefab = Resources.Load<GameObject>("UI/" + name);
        var go = Object.Instantiate(prefab);
        var ui = go.GetComponent<UIBase>();

//        var layer = getLayer(ui.layerId);

        go.name = name;
        _name2layerDic[name] = ui.layerId;
//        ui.show();
        if (during > 0)
            ui.fadeIn(during);
        else
            ui.show();

        return ui;
    }

    public T show<T>(string name) where T : UIBase
    {
        return show(name) as T;
    }

    public void hide(string name, float during=0)
    {
        Debug.LogError(name);
        var cur = find(name);
        if (cur != null)
        {
            //cur.hide();
            if (during > 0)
                cur.fadeOut(during);
            else
                cur.hide();
        }
        else
            Debug.LogError("cant find ui named : " + name);
    }

    public void hideAll(UILayerId layerId)
    {
        var layer = getLayer(layerId);
        for (int i = layer.list.Count - 1; i >= 0; i--)
        {
            var ui = layer.list[i];
            ui.hide();
        }
    }

    private class Layer
    {
        public UILayerId id;
        public int order;
        public GameObject root;
        public List<UIBase> list = new List<UIBase>();

        public UIBase find(string name)
        {
            return list.Find(ui => name == ui.name);
        }

    }
}
