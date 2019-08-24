using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ShareRender:MonoBehaviour
{
    MeshFilter filter;
    MeshRenderer render;
    Mesh mesh;
    public List<ShareElement> elements = new List<ShareElement>();
    public Texture2D texture;
    public string textureName;
    private void Awake()
    {
        filter = GetComponent<MeshFilter>();
        render = GetComponent<MeshRenderer>();
        mesh = filter.mesh;
        render.material = new Material(Shader.Find("UI/Default"));
        if(texture!=null)
        {
            textureName = texture.name;
            render.material.mainTexture = texture;
        }
       
    }
    List<Vector3> Vertex = new List<Vector3>();
    List<Vector2> UV = new List<Vector2>();
    List<Color> Colors = new List<Color>();
    List<int> Tri = new List<int>();
    public Vector3 Position;
    public void VertexCalculation()
    {
        List<Vector3> vert = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> tri = new List<int>();
        for (int i = 0; i < elements.Count; i++)
            elements[i].GetUVInfo(vert,uv,colors, tri, Vector3.zero, Quaternion.identity, Vector3.one);
        Vertex = vert;
        UV = uv;
        Colors = colors;
        Tri = tri;
        Aplly();
    }
    public void AsyncVertexCalculation()
    {
        List<Vector3> vert = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> tri = new List<int>();
        for (int i = 0; i < elements.Count; i++)
            elements[i].GetUVInfo(vert, uv, colors, tri, Vector3.zero, Quaternion.identity, Vector3.one);
        Vertex = vert;
        UV = uv;
        Colors = colors;
        Tri = tri;
    }
    public void Aplly()
    {
        mesh.triangles = null;
        if (Vertex == null)
            return;
        if (Vertex.Count == 0)
            return;
        mesh.vertices = Vertex.ToArray();
        mesh.uv = UV.ToArray();
        mesh.colors = Colors.ToArray();
        mesh.triangles = Tri.ToArray();
    }
    public void SetTexture(Texture2D text)
    {
        texture = text;
        textureName = text.name;
        render.material.mainTexture = texture;
    }
}
public enum CollisionType
{
    None, Circle, Rectangle, Polygon
}
public class ShareElement
{
    public object DataContext;
    public bool activeSelf = true;
    public List<ShareElement> Child = new List<ShareElement>();
    public ShareRender Render;
    public Vector3 localPosition;
    public Vector3 localScale = Vector3.one;
    public Vector2 sizeDelta=new Vector2(1,1);
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
    public Vector3 Position;
    Quaternion Rotate;
    public Quaternion localRotation;
    public Rect rect;
    public Vector2 textureSize;
    public Vector3 Angle { get => localRotation.eulerAngles; set => localRotation = Quaternion.Euler(value); }
    public float fillAmountX = 1;
    public float fillAmountY = 1;
    public Color color = Color.white;
    Vector3[] vertex = new Vector3[4];
    Vector2[] uvs = new Vector2[4];
    public void GetUVInfo(List<Vector3> vertices, List<Vector2> uv,List<Color> colors, List<int> tri, Vector3 position, Quaternion quate, Vector3 scale)
    {
        float w = localScale.x * sizeDelta.x;
        float h = localScale.y * sizeDelta.y;
        var pos = localPosition;
        pos = quate * pos + position;
        float left = -pivot.x * w;
        float right = left + w * fillAmountX;
        float down = -pivot.y * h;
        float top = down + h * fillAmountY;
        Vector3 ls = localScale;
        ls.x *= scale.x;
        ls.y *= scale.y;
        right *= ls.x;
        left *= ls.x;
        down *= ls.y;
        top *= ls.y;
        Position = pos;
        ///注意顺序quate要放前面
        var q = quate * localRotation;
        Rotate = q;
        if (activeSelf)
        {
            vertex[0] = q * new Vector3(left, down) + pos;
            vertex[1] = q * new Vector3(left, top) + pos;
            vertex[2] = q * new Vector3(right, top) + pos;
            vertex[3] = q * new Vector3(right, down) + pos;
            float uw = uvs[2].x - uvs[1].x;
            float ux = uvs[1].x + uw * fillAmountX;
            float uh = uvs[2].y - uvs[1].y;
            float uy = uvs[1].y + uh * fillAmountY;
            uvs[0]= uvs[0];
            uvs[1]= uvs[1];
            uvs[1].y = uy;
            uvs[2].y = uy;
            uvs[2].x = ux;
            uvs[3]= uvs[3];
            uvs[3].x = ux;
            int s = vertices.Count;
            vertices.AddRange(vertex);
            uv.AddRange(uvs);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            tri.Add(s);
            tri.Add(s + 1);
            tri.Add(s + 2);
            tri.Add(s + 2);
            tri.Add(s + 3);
            tri.Add(s);
        }
        if (activeSelf)
            for (int i = 0; i < Child.Count; i++)
                Child[i].GetUVInfo(vertices, uv,colors, tri, pos, q, ls);
    }
    public void SetNactiveSize()
    {
        float w = rect.width;
        float h = rect.height;
        sizeDelta = new Vector2(w, h);
    }
    public void LoadSprite(string texture,string spriteName)
    {
        Vector2 p = pivot;
        ElementAsset.FindSpriteUV(texture, spriteName, ref rect, ref textureSize, ref p);
        pivot = new Vector2(p.x / rect.width, p.y / rect.height);
        ResetUV();
    }
    void ResetUV()
    {
        float tx = textureSize.x;
        float ty = textureSize.y;
        float x = rect.x / tx;
        float y = rect.y / ty;
        float w = rect.width;
        float h = rect.height;
        float r = x + w / tx;
        float t = y + h / ty;
        uvs[0].x = x;
        uvs[0].y = y;
        uvs[1].x = x;
        uvs[1].y = t;
        uvs[2].x = r;
        uvs[2].y = t;
        uvs[3].x = r;
        uvs[3].y = y;
    }
    public ElementCollider collider;
    /// <summary>
    /// 更新碰撞点
    /// </summary>
    public void UpdatePoints()
    {
        if(collider!=null)
        {
            var pos = collider.Position = Render.Position + Position;
            if(collider.LocalPoints!=null)
            {
                int len = collider.LocalPoints.Length;
                if (len != collider.points.Length)
                    collider.points = new Vector2[len];
                for (int i = 0; i < len; i++)
                    collider.points[i] = Rotate * collider.LocalPoints[i] + pos;
            }
        }
    }
    public SpriteRectInfo[] rectInfos;
    public void SetGif(string text,string[] spriteNames)
    {
        rectInfos = ElementAsset.FindSpriteUVs(text,spriteNames);
        ApplyGifSprite(0);
    }
    void ApplyGifSprite(int index)
    {
        textureSize = rectInfos[index].txtSize;
        rect = rectInfos[index].rect;
        pivot = new Vector2(rectInfos[index].pivot.x / rect.width, rectInfos[index].pivot.y / rect.height);
        ResetUV();
    }
    bool Loop;
    bool gif;
    float gifTime;
    float allTime;
    int index = 0;
    public void PlayGif(float time,bool loop = false)
    {
        allTime = time;
        Loop = loop;
        gif = true;
        gifTime = 0;
        index = 0;
    }
    bool scaleAble;
    bool sloop;
    float sTime;
    float sat;
    float sss;
    float sts;
    public float moveX = -0.002f;
    public void Scale(float time,float ts,bool loop=true)
    {
        sss = localScale.x;
        sts = ts;
        sTime = 0;
        sat = time;
        sloop = loop;
    }
    public void Update(float time)
    {
        if(gif)
        {
            if(rectInfos!=null)
                if(rectInfos.Length>1)
                {
                    gifTime += time;
                    if (gifTime > allTime)
                    {
                        if (Loop)
                        {
                            gifTime -= allTime;
                            var r = (int)(gifTime / allTime * rectInfos.Length);
                            if (r != index)
                            {
                                ApplyGifSprite(r);
                                index = r;
                            }
                        }
                        else
                        {
                            if (index != rectInfos.Length - 1)
                            {
                                index = rectInfos.Length - 1;
                                ApplyGifSprite(index);
                            }
                            gif = false;
                        }
                    }
                    else
                    {
                        var r = (int)(gifTime / allTime * rectInfos.Length);
                        if (r != index)
                        {
                            ApplyGifSprite(r);
                            index = r;
                        }
                    }
                }
        }
        if(scaleAble)
        {
            sTime += time;
            if(sTime>sat)
            {
                if(sloop)
                {
                    sTime -= sat;
                    float r = sTime / sat;
                    r = (sts - sss) * r + sss;
                    localScale.x = r;
                    localScale.y = r;
                    localScale.z = r;
                }
                else
                {
                    scaleAble = false;
                    localScale.x = sts;
                    localScale.y = sts;
                    localScale.z = sts;
                }
            }
            else
            {
                float r = sTime / sat;
                r = (sts - sss) * r + sss;
                localScale.x = r;
                localScale.y = r;
                localScale.z = r;
            }
        }
        float s = moveX * time;
        localPosition.x += s;
        if (localPosition.x < -12)
            activeSelf = false;
        else if (localPosition.x > 12)
            activeSelf = false;
        else activeSelf = true;
    }
    public void Reset()
    {
        scaleAble = false;
        gif = false;
    }
}
public class ElementCollider
{
    public ShareElement Target;
    public Vector3 Position;
    public CollisionType collisionType;
    public float Radius;
    public Vector2[] LocalPoints;
    public Vector2[] points;
    public Action<ElementCollider> OnCollision;
    public void CheckCollision(ElementCollider tar)
    {
        switch (collisionType)
        {
            case CollisionType.None:
                break;
            case CollisionType.Circle:
                CircleCollision(this,tar);
                break;
            case CollisionType.Rectangle:
            case CollisionType.Polygon:
                PolygonCollision(this,tar);
                break;
        }
    }
    static void CircleCollision(ElementCollider collider,ElementCollider tar)
    {
        if (collider.Radius == 0)
            return;
        switch(tar.collisionType)
        {
            case CollisionType.None:
                break;
            case CollisionType.Circle:
                if (huqiang.Physics2D.CircleToCircle(collider.Position, tar.Position, collider.Radius, tar.Radius))
                    if (tar.OnCollision != null)
                        tar.OnCollision(collider);
                break;
            case CollisionType.Polygon:
                if (tar.points != null)
                    if (huqiang.Physics2D.CircleToPolygon(collider.Position, collider.Radius, tar.points))
                        if (tar.OnCollision != null)
                            tar.OnCollision(collider);
                break;
        }
    }
    static void PolygonCollision(ElementCollider collider,ElementCollider tar)
    {
        if (collider.points == null)
            return;
        switch(tar.collisionType)
        {
            case CollisionType.None:
                break;
            case CollisionType.Circle:
                if (collider.points != null)
                    if (huqiang.Physics2D.CircleToPolygon(tar.Position, tar.Radius, collider.points))
                        if (tar.OnCollision != null)
                            tar.OnCollision(collider);
                break;
            case CollisionType.Polygon:
                if (collider.points != null)
                    if (tar.points != null)
                        if (huqiang.Physics2D.PToP2(tar.points, collider.points))
                            if (tar.OnCollision != null)
                                tar.OnCollision(collider);
                break;
        }
    }
}