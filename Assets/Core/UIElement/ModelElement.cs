using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UI
{
    public unsafe struct ElementData
    {
        public Int64 type;
        public Int32 childCount;
        public Int32 name;
        public Int32 tag;
        public Quaternion localRotation;
        public Vector3 localPosition;
        public Vector3 localScale;
        public Vector2 anchoredPosition;
        public Vector3 anchoredPosition3D;
        public Vector2 anchorMax;
        public Vector2 anchorMin;
        public Vector2 offsetMax;
        public Vector2 offsetMin;
        public Vector2 pivot;
        public Vector2 sizeDelta;
        public bool SizeScale;
        public ScaleType scaleType;
        public AnchorType anchorType;
        public MarginType marginType;
        public Vector2 anchorOffset;
        public AnchorPointType anchorpointType;
        public ParentType parentType;
        public Margin margin;
        public Vector2 DesignSize;
        /// <summary>
        /// int32数组,高16位为索引,低16位为类型
        /// </summary>
        public Int32 coms;
        /// <summary>
        /// int16数组
        /// </summary>
        public Int32 child;
        public Int32 ex;
        public static int Size = sizeof(ElementData);
        public static int ElementSize = Size / 4;
    }
    public interface UpdateInterface
    {
        void Update();
    }
    public class ModelElement : DataConversion,UITransform
    {
        public static Coordinates GetGlobaInfo(ModelElement rect, bool Includeroot = true)
        {
            ModelElement[] buff = new ModelElement[32];
            buff[0] = rect;
            var parent = rect.parent;
            int max = 1;
            if (parent != null)
                for (; max < 32; max++)
                {
                    buff[max] = parent;
                    parent = parent.parent;
                    if (parent == null)
                        break;
                }
            Vector3 pos, scale;
            Quaternion quate;
            if (Includeroot)
            {
                var p = buff[max];
                pos = p.data.localPosition;
                scale = p.data.localScale;
                quate = p.data.localRotation;
                max--;
            }
            else
            {
                pos = Vector3.zero;
                scale = Vector3.one;
                quate = Quaternion.identity;
                max--;
            }
            for (; max >= 0; max--)
            {
                var rt = buff[max];
                Vector3 p = rt.data.localPosition;
                Vector3 o = Vector3.zero;
                o.x = p.x * scale.x;
                o.y = p.y * scale.y;
                o.z = p.z * scale.z;
                pos +=quate * o;
                quate *= rt.data.localRotation;
                Vector3 s = rt.data.localScale;
                scale.x *= s.x;
                scale.y *= s.y;
            }
            Coordinates coord = new Coordinates();
            coord.Postion = pos;
            coord.quaternion = quate;
            coord.Scale = scale;
            return coord;
        }
        public static ModelElement CreateNew(string name)
        {
            var mod = new ModelElement();
            mod.name = name;
       
            return mod;
        }
        public ModelElement()
        {
            data.localScale = Vector3.one;
            data.localRotation = Quaternion.identity;
            data.type = ModelManagerUI.GetTypeIndex(this);
            data.anchorMax = data.anchorMin = data.pivot = new Vector2(0.5f, 0.5f);
        }
        public Coordinates coordinates { get { return GetGlobaInfo(this); } }
        public Vector3 ScreenToLocal(Vector3 v)
        {
            var g = GetGlobaInfo(this);
            v -= g.Postion;
            if (g.Scale.x != 0)
                v.x /= g.Scale.x;
            else v.x = 0;
            if (g.Scale.y != 0)
                v.y /= g.Scale.y;
            else v.y = 0;
            if (g.Scale.z != 0)
                v.z /= g.Scale.z;
            else v.z = 0;
            var q = Quaternion.Inverse(g.quaternion);
            v = q * v;
            return v;
        }
        public RectTransform Context;
        public GameObject Main;
        public Coloring ColorController;
        public UpdateInterface updating;
        public int regIndex;
        public ElementData data;
        public string name;
        public string tag;
        public virtual Color color { get {
                if (ColorController == null)
                    return Color.white;
                return ColorController.color;
            } set {
                if (ColorController != null)
                    ColorController.color = value;
            } }
        public ModelElement parent { get; private set; }
        public List<DataConversion> components = new List<DataConversion>();
        public List<ModelElement> child = new List<ModelElement>();
        bool parentChanged;
        public virtual void SetParent(ModelElement element)
        {
            if (element == this)
                return;
            if (parent != null)
                parent.child.Remove(this);
            if (element != null)
                element.child.Add(this);
            parent = element;
            parentChanged = true;
        }
        public DataConversion GetComponent(string type)
        {
            for (int i = 0; i < components.Count; i++)
                if (type == components[i].type)
                    return components[i];
            return null;
        }
        public T GetComponent<T>() where T : DataConversion
        {
            for (int i = 0; i < components.Count; i++)
            {
                T t = components[i] as T;
                if (t != null)
                    return t;
            }
            return null;
        }
        public T AddComponent<T>()where T:DataConversion,new()
        {
            T t = new T();
            data.type |= ModelManagerUI.GetTypeIndex(t);
            components.Add(t);
            t.model = this;
            t.Reset();
            return t;
        }
        public FakeStruct ModData;
        public unsafe static FakeStruct FindChild(FakeStruct fake, string name)
        {
            var data = (ElementData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = buff.GetData(data->child) as Int16[];
            if (chi != null)
            {
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var son = (ElementData*)fs.ip;
                        var n = buff.GetData(son->name) as string;
                        if (n == name)
                            return fs;
                    }
                }
            }
            return null;
        }
        unsafe public override void Load(FakeStruct fake)
        {
            data = *(ElementData*)fake.ip;
            ModData = fake;
            var buff = fake.buffer;
            Int16[] coms = buff.GetData(data.coms) as Int16[];
            if (coms != null)
            {
                for (int i = 0; i < coms.Length; i++)
                {
                    int index = coms[i];
                    i++;
                    int type = coms[i];
                    var fs = buff.GetData(index) as FakeStruct;
                    var dc = ModelManagerUI.Load(type);
                    if (dc != null)
                    {
                        dc.model = this;
                        if (fs != null)
                            dc.Load(fs);
                        components.Add(dc);
                    }
                }
            }
            Int16[] chi = fake.buffer.GetData(data.child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        ModelElement model = new ModelElement();
                        model.Load(fs);
                        child.Add(model);
                        model.parent = this;
                    }
                }
            name = buff.GetData(data.name) as string;
            tag = buff.GetData(data.tag) as string;
        }
        public override void LoadToObject(Component com)
        {
            Main = com.gameObject;
            for (int i = 0; i < components.Count; i++)
                if (components[i] != null)
                    components[i].LoadToObject(com);
            Context = com as RectTransform;
            if (parent != null)
            {
                Context.SetParent(parent.Context);
                Context.SetSiblingIndex(parent.child.IndexOf(this));
            }
            LoadToObject(Context, ref data, this);
        }
        public static void LoadToObject(RectTransform com, ref ElementData data, ModelElement ui)
        {
            var trans = com as RectTransform;
            trans.anchoredPosition = data.anchoredPosition;
            trans.anchoredPosition3D = data.anchoredPosition3D;
            trans.anchorMax = data.anchorMax;
            trans.anchorMin = data.anchorMin;
            trans.offsetMax = data.offsetMax;
            trans.offsetMin = data.offsetMin;
            trans.pivot = data.pivot;
            trans.sizeDelta = data.sizeDelta;
            trans.localRotation = data.localRotation;
            trans.localPosition = data.localPosition;
            trans.localScale = data.localScale;
            trans.name = ui.name;
            if (ui.tag != null)
                trans.tag = ui.tag;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var trans = com as RectTransform;
            if (trans == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, ElementData.ElementSize);
            ElementData* ed = (ElementData*)fake.ip;
            ed->localRotation = trans.localRotation;
            ed->localPosition = trans.localPosition;
            ed->localScale = trans.localScale;
            ed->anchoredPosition = trans.anchoredPosition;
            ed->anchoredPosition3D = trans.anchoredPosition3D;
            ed->anchorMax = trans.anchorMax;
            ed->anchorMin = trans.anchorMin;
            ed->offsetMax = trans.offsetMax;
            ed->offsetMin = trans.offsetMin;
            ed->pivot = trans.pivot;
            ed->sizeDelta = trans.sizeDelta;
            ed->name = buffer.AddData(trans.name);
            ed->tag = buffer.AddData(trans.tag);
            var coms = com.GetComponents<Component>();
            ed->type = ModelManagerUI.GetTypeIndex(coms);
            List<Int16> tmp = new List<short>();
            for (int i = 0; i < coms.Length; i++)
            {
                var rect = coms[i] as RectTransform;
                if (rect == null)
                {
                    var ss = coms[i] as SizeScaling;
                    if (ss != null)
                    {
                        ed->SizeScale = true;
                        ed->scaleType = ss.scaleType;
                        ed->anchorType = ss.anchorType;
                        ed->anchorpointType = ss.anchorPointType;
                        ed->anchorOffset = ss.anchorOffset;
                        ed->marginType = ss.marginType;
                        ed->parentType = ss.parentType;
                        ed->margin = ss.margin;
                        ed->DesignSize = ss.DesignSize;
                    }
                    else if (coms[i] is UICompositeHelp)
                    {
                        ed->ex = buffer.AddData((coms[i] as UICompositeHelp).ToBufferData(buffer));
                    }
                    else if (!(coms[i] is CanvasRenderer))
                    {
                        Int16 type = 0;
                        var fs = ModelManagerUI.LoadFromObject(coms[i], buffer, ref type);
                        if (type > 0)
                        {
                            tmp.Add((Int16)buffer.AddData(fs));
                            tmp.Add(type);
                        }
#if UNITY_EDITOR
                        else Debug.Log(com.name);
#endif
                    }
                }
            }
            if (tmp.Count > 0)
                ed->coms = buffer.AddData(tmp.ToArray());
            int c = trans.childCount;
            if (c > 0)
            {
                Int16[] buf = new short[c];
                for (int i = 0; i < c; i++)
                {
                    var fs = LoadFromObject(trans.GetChild(i), buffer);
                    buf[i] = (Int16)buffer.AddData(fs);
                }
                ed->child = buffer.AddData(buf);
            }
            return fake;
        }
        public ModelElement Find(string name)
        {
            for (int i = 0; i < child.Count; i++)
                if (child[i].name == name)
                    return child[i];
            return null;
        }
#if UNITY_EDITOR
        public void AddSizeScale()
        {
            if (data.SizeScale)
            {
                if (Main != null)
                {
                    var scale = Main.GetComponent<SizeScaleEx>();
                    if (scale == null)
                        scale = Main.AddComponent<SizeScaleEx>();
                    scale.scaleType = data.scaleType;
                    scale.anchorType = data.anchorType;
                    scale.anchorPointType = data.anchorpointType;
                    scale.marginType = data.marginType;
                    scale.parentType = data.parentType;
                    scale.margin = data.margin;
                    scale.DesignSize = data.DesignSize;
                }
            }
        }
#endif

        #region 尺寸自适应
        public static Vector2[] Anchors = new[] { new Vector2(0.5f, 0.5f), new Vector2(0, 0.5f),new Vector2(1, 0.5f),
        new Vector2(0.5f, 1),new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)};
        public static void Scaling(ModelElement rect, ScaleType dock, Vector2 pSize, Vector2 ds)
        {
            switch (dock)
            {
                case ScaleType.None:
                    break;
                case ScaleType.FillX:
                    float sx = pSize.x / ds.x;
                    rect.data.localScale = new Vector3(sx, sx, sx);
                    break;
                case ScaleType.FillY:
                    float sy = pSize.y / ds.y;
                    rect.data.localScale = new Vector3(sy, sy, sy);
                    break;
                case ScaleType.FillXY:
                    sx = pSize.x / ds.x;
                    sy = pSize.y / ds.y;
                    if (sx < sy)
                        rect.data.localScale = new Vector3(sx, sx, sx);
                    else rect.data.localScale = new Vector3(sy, sy, sy);
                    break;
                case ScaleType.Cover:
                    sx = pSize.x / ds.x;
                    sy = pSize.y / ds.y;
                    if (sx < sy)
                        rect.data.localScale = new Vector3(sy, sy, sy);
                    else rect.data.localScale = new Vector3(sx, sx, sx);
                    break;
            }
        }
        public static void Anchoring(ModelElement ele,AnchorType type,ref Vector2 p,ref Vector2 psize)
        {
            switch(type)
            {
                case AnchorType.Anchor:
                    AnchorEx(ele, ele.data.anchorpointType, ele.data.anchorOffset, p, psize);
                    break;
                case AnchorType.Alignment:
                    AlignmentEx(ele, ele.data.anchorpointType, ele.data.anchorOffset, p, psize);
                    break;
            }
        }
        public static void AnchorEx(ModelElement rect, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
        {
            var pivot = Anchors[(int)type];
            float ox = (p.x - 1) * psize.x;//原点x
            float oy = (p.y - 1) * psize.y;//原点y
            float tx = ox + pivot.x * psize.x;//锚点x
            float ty = oy + pivot.y * psize.y;//锚点y
            offset.x += tx;//偏移点x
            offset.y += ty;//偏移点y
            rect.data.localPosition = new Vector3(offset.x, offset.y, 0);
        }
        public static void AlignmentEx(ModelElement rect, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
        {
            Vector2 pivot = Anchors[(int)type];
            float ox = (p.x - 1) * psize.x;//原点x
            float oy = (p.y - 1) * psize.y;//原点y
            float tx = ox + pivot.x * psize.x;//锚点x
            float ty = oy + pivot.y * psize.y;//锚点y
            offset.x += tx;//偏移点x
            offset.y += ty;//偏移点y
            switch (type)
            {
                case AnchorPointType.Left:
                    offset.x += rect.data.sizeDelta.x * 0.5f;
                    break;
                case AnchorPointType.Right:
                    offset.x -= rect.data.sizeDelta.x * 0.5f;
                    break;
                case AnchorPointType.Top:
                    offset.y -= rect.data.sizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.Down:
                    offset.y += rect.data.sizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.LeftDown:
                    offset.x += rect.data.sizeDelta.x * 0.5f;
                    offset.y += rect.data.sizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.LeftTop:
                    offset.x += rect.data.sizeDelta.x * 0.5f;
                    offset.y -= rect.data.sizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.RightDown:
                    offset.x -= rect.data.sizeDelta.x * 0.5f;
                    offset.y += rect.data.sizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.RightTop:
                    offset.x -= rect.data.sizeDelta.x * 0.5f;
                    offset.y -= rect.data.sizeDelta.y * 0.5f;
                    break;
            }
            rect.data.localPosition = new Vector3(offset.x, offset.y, 0);
        }
        public static void MarginEx(ModelElement rect, Margin margin, Vector2 parentPivot, Vector2 parentSize)
        {
            float w = parentSize.x - margin.left - margin.right;
            float h = parentSize.y - margin.top - margin.down;
            var m_pivot = rect.data.pivot;
            float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
            float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
            float sx = rect.data.localScale.x;
            float sy = rect.data.localScale.y;
            rect.data.sizeDelta = new Vector2(w / sx, h / sy);
            rect.data.localPosition = new Vector3(ox, oy, 0);
        }
        public static void MarginX(ModelElement rect, Margin margin, Vector2 parentPivot, Vector2 parentSize)
        {
            float w = parentSize.x - margin.left - margin.right;
            var m_pivot = rect.data.pivot;
            float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
            float sx = rect.data.localScale.x;
            rect.data.sizeDelta.x = w / sx;
            rect.data.localPosition.x = ox;
        }
        public static void MarginY(ModelElement rect, Margin margin, Vector2 parentPivot, Vector2 parentSize)
        {
            float h = parentSize.y - margin.top - margin.down;
            var m_pivot = rect.data.pivot;
            float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
            float sy = rect.data.localScale.y;
            rect.data.sizeDelta.y = h / sy;
            rect.data.localPosition.y = oy;
        }
        static void Resize(ModelElement ele)
        {
            if (ele == null)
                return;
            Vector2 psize;
            Vector2 p = Anchors[0];
            if (ele.data.parentType == ParentType.Tranfrom)
            {
                var t = ele.parent;
                psize = t.data.sizeDelta;
                p = t.data.pivot;
            }
            else
            {
                psize = new Vector2(Scale.LayoutWidth, Scale.LayoutHeight);
            }
            Scaling(ele, ele.data.scaleType, psize, ele.data.DesignSize);
            Anchoring(ele,ele.data.anchorType,ref p,ref psize);
            var st = ele.data.marginType;
            switch (st)
            {
                case MarginType.Margin:
                    var mar = ele.data.margin;
                    if (ele.data.parentType == ParentType.BangsScreen)
                        if (Scale.LayoutHeight / Scale.LayoutWidth > 2f)
                            mar.top += 88;
                    MarginEx(ele, mar, p, psize);
                    break;
                case MarginType.MarginRatio:
                    mar = new Margin();
                    mar.left = ele.data.margin.left * psize.x;
                    mar.right = ele.data.margin.right * psize.x;
                    mar.top = ele.data.margin.top * psize.y;
                    mar.down = ele.data.margin.down * psize.y;
                    if (ele.data.parentType == ParentType.BangsScreen)
                        if (Scale.LayoutHeight / Scale.LayoutWidth > 2f)
                            mar.top += 88;
                    MarginEx(ele, mar, p, psize);
                    break;
                case MarginType.MarginX:
                    mar = ele.data.margin;
                    if (ele.data.parentType == ParentType.BangsScreen)
                        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                            mar.top += 88;
                    MarginX(ele, mar, p, psize);
                    break;
                case MarginType.MarginY:
                    mar = ele.data.margin;
                    if (ele.data.parentType == ParentType.BangsScreen)
                        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                            mar.top += 88;
                    MarginY(ele, mar, p, psize);
                    break;
            }
        }
        public static void ScaleSize(ModelElement element)
        {
            if (element.data.SizeScale)
            {
                Resize(element);
                if (element.SizeChanged != null)
                    element.SizeChanged(element);
                element.IsChanged = true;
            }
            var child = element.child;
            for (int i = 0; i < child.Count; i++)
            {
                ScaleSize(child[i]);
            }
        }
        public static void ScaleSize(ModelElement element,int level)
        {
            if (element.data.SizeScale)
            {
                Resize(element);
                if (element.SizeChanged != null)
                    element.SizeChanged(element);
                element.IsChanged = true;
            }
            level--;
            if (level > 0)
            {
                var child = element.child;
                for (int i = 0; i < child.Count; i++)
                    ScaleSize(child[i], level);
            }
        }
        #endregion
        /// <summary>
        /// 当此物体为非实体时,不予创建GameObject
        /// </summary>
        public bool Entity = true;
        /// <summary>
        /// 自动回收
        /// </summary>
        public bool AutoRecycle = true;
        protected bool _active = true;
        public bool activeSelf { get { return _active; } set { if (_active == value) return; IsChanged = true; _active = value; } }

        public Quaternion LocalRotate { get => data.localRotation; set { data.localRotation = value; IsChanged = true; } }
        public Vector2 SizeDelta { get => data.sizeDelta; set { data.sizeDelta = value; IsChanged = true; } }
        public Vector3 LocalPosition { get => data.localPosition; set { data.localPosition = value; IsChanged = true; } }
        public Vector2 LocalScale { get => data.localScale; set { data.localScale = value; IsChanged = true; } }
        public Vector3 GlobalPosition { get => GetGlobaInfo(this).Postion; }
        public Vector3 GlobalScale { get => GetGlobaInfo(this).Scale; }
        public Quaternion GlobalRotate { get => GetGlobaInfo(this).quaternion; }

        public EventCallBack baseEvent;
        public void RegEvent<T>() where T : EventCallBack, new()
        {
            if (baseEvent != null)
            {
                var t = baseEvent as T;
                if (t != null)
                {
                    EventCallBack.RegEvent<T>(t);
                    return;
                }
            }
            baseEvent = EventCallBack.RegEvent<T>(this);
        }
        public void Update()
        {
            if(_active)
            {
                if (updating != null)
                    updating.Update();
                for (int i = 0; i < child.Count; i++)
                    child[i].Update();
            }
        }
        public override void Apply()
        {
            if(Entity)
            {
                if (activeSelf)
                {
                    if (Context == null)
                    {
                        var obj = ModelManagerUI.CreateNew(data.type);
                        if (obj != null)
                            LoadToObject(obj.transform);
                    }
                    else
                    {
                        if (parentChanged)
                        {
                            if (parent != null)
                            {
                                Context.SetParent(parent.Context);
                                Context.SetSiblingIndex(parent.child.IndexOf(this));
                            }
                            else Context.SetParent(null);
                        }
                        if (IsChanged)
                        {
                            IsChanged = false;
                            LoadToObject(Context, ref data, this);
                        }
                    }
                    if (mIndex > -1)
                    {
                        if (mIndex > Context.childCount)
                            Context.SetAsLastSibling();
                        else Context.SetSiblingIndex(mIndex);
                        mIndex = -1;
                    }
                    for (int i = 0; i < components.Count; i++)
                    {
                        var com = components[i];
                        if (com != null)
                        {
                            com.Apply();
                        }
                    }
                    for (int i = 0; i < child.Count; i++)
                        child[i].Apply();
                    Context.gameObject.SetActive(true);
                }
                else
                {
                    if (Context != null)
                    {
                        Context.gameObject.SetActive(false);
                    }
                }
            }
        }
        public T ComponentReflection<T>() where T : class, new()
        {
            T t = new T();
            ModelManagerUI.ComponentReflection(this, t);
            return t;
        }
        int mIndex = -1;
        public void SetSiblingIndex(int index)
        {
            mIndex = index;
        }
        public int GetSiblingIndex()
        {
            var p = parent;
            if (p == null)
                return -1;
            return p.child.IndexOf(this);
        }
        public object GetExtand()
        {
            if (ModData != null)
                return ModData.buffer.GetData(data.ex);
            return null;
        }
        public Action<ModelElement> SizeChanged;
        public void SetSize(Vector2 size)
        {
            data.sizeDelta = size;
            if (SizeChanged != null)
                SizeChanged(this);
            IsChanged = true;
        }
    }
}
