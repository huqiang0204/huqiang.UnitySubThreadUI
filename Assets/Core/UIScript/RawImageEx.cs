using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class RawImageEx : RawImage
    {
        static int[] trileft = new int[]{
        0,1,2,2,1,3,
        4,5,6,6,5,7,
        2,7,8,8,7,13,
        8,9,10,10,9,11,
        12,13,14,14,13,15,
        1,16,3,3,16,18,
        16,17,18,18,17,19,
        17,4,19,19,4,6,
        9,12,11,11,12,14,
    };
        /// <summary>
        /// 网格顺序 左下0，左下1（可拉伸），左中（箭头），左上0（可拉伸），左上1，中间（可拉伸），右下，右中（可拉伸），右上
        /// </summary>
        void ComputeLeft()
        {
            Vector3[] v = new Vector3[20];
            for (int i = 0; i < 16; i++)
                v[i] = buff[i];
            float fix = arrow.w - arrow.y;
            float y = size.y - corner0.y - corner1.y - fix + dock;
            v[16].x = buff[0].x;
            v[16].y = y;
            v[17].x = buff[0].x;
            v[17].y = y + fix;
            v[18].x = buff[2].x;
            v[18].y = y;
            v[19].x = buff[2].x;
            v[19].y = y + fix;
            Vertex = v;
        }
        void GetLeftUV()
        {
            if (texture == null)
                return;
            float w = texture.width;
            float h = texture.height;
            float x = corner0.x / w;
            float y = corner0.y / h;
            int index = fix0;

            uv[index].x = 0;
            uv[index].y = 0;
            index++;
            uv[index].x = 0;
            uv[index].y = y;
            index++;
            uv[index].x = x;
            uv[index].y = 0;
            index++;
            uv[index].x = x;
            uv[index].y = y;

            x = corner1.x / w;
            y = corner1.y / h;
            index = fix1;
            uv[index].x = 0;
            uv[index].y = 1;
            index++;
            uv[index].x = x;
            uv[index].y = 1 - y;
            index++;
            uv[index].x = x;
            uv[index].y = 1;
            index -= 3;
            uv[index].x = 0;
            uv[index].y = 1 - y;

            x = corner2.x / w;
            y = corner2.y / h;
            index = fix2;
            uv[index].x = 1;
            uv[index].y = 0;
            index++;
            uv[index].x = 1;
            uv[index].y = y;
            index -= 3;
            uv[index].x = 1 - x;
            uv[index].y = 0;
            index++;
            uv[index].x = 1 - x;
            uv[index].y = y;

            x = corner3.x / w;
            y = corner3.y / h;
            index = fix3;
            uv[index].x = 1;
            uv[index].y = 1;
            index--;
            uv[index].x = 1;
            uv[index].y = 1 - y;
            index--;
            uv[index].x = 1 - x;
            uv[index].y = 1;
            index--;
            uv[index].x = 1 - x;
            uv[index].y = 1 - y;

            x = arrow.x / w;
            y = arrow.y / h;
            float x1 = arrow.z / w;
            float y1 = arrow.w / h;
            uv[16].x = x;
            uv[16].y = y;
            uv[17].x = x;
            uv[17].y = y1;
            uv[18].x = x1;
            uv[18].y = y;
            uv[19].x = x1;
            uv[19].y = y1;
        }
        void Fixation()
        {
            float x = size.x;
            float y = size.y;
            int index = fix0;
            buff[index].x = -x;
            buff[index].y = -y;
            index++;
            buff[index].x = -x;
            buff[index].y = -y + corner0.y;
            index++;
            buff[index].x = -x + corner0.x;
            buff[index].y = -y;
            index++;
            buff[index].x = -x + corner0.x;
            buff[index].y = -y + corner0.y;

            index = fix1;
            buff[index].x = -x;
            buff[index].y = y;
            index++;
            buff[index].x = -x + corner1.x;
            buff[index].y = y - corner1.y;
            index++;
            buff[index].x = -x + corner1.x;
            buff[index].y = y;
            index -= 3;
            buff[index].x = -x;
            buff[index].y = y - corner1.y;

            index = fix2;
            buff[index].x = x;
            buff[index].y = -y;
            index++;
            buff[index].x = x;
            buff[index].y = -y + corner2.y;
            index -= 3;
            buff[index].x = x - corner2.x;
            buff[index].y = -y;
            index++;
            buff[index].x = x - corner2.x;
            buff[index].y = -y + corner2.y;

            index = fix3;
            buff[index].x = x;
            buff[index].y = y;
            index--;
            buff[index].x = x;
            buff[index].y = y - corner3.y;
            index--;
            buff[index].x = x - corner3.x;
            buff[index].y = y;
            index--;
            buff[index].x = x - corner3.x;
            buff[index].y = y - corner3.y;
        }
        Vector3[] buff = new Vector3[16];
        int fix0 = 0, fix1 = 5, fix2 = 10, fix3 = 15;
        public Vector2 corner0, corner1, corner2, corner3;
        public float dock;

        private Vector2 size;

        private const float dockMax = 32f;
        public float docMin = 0;
        /// <summary>
        /// 箭头中心位置
        /// </summary>
        public Vector4 arrow = Vector4.zero;
        Vector3[] Vertex;
        Vector2[] uv = new Vector2[20];
        List<UIVertex> buffer;
        List<int> tri;
        void Refresh()
        {
            size = rectTransform.sizeDelta * 0.5f;
            Fixation();
            ComputeLeft();
            GetLeftUV();
            if (buffer == null)
                buffer = new List<UIVertex>();
            buffer.Clear();
            for (int i = 0; i < 20; i++)
            {
                var v = new UIVertex();
                v.position = Vertex[i];
                v.uv0 = uv[i];
                v.uv1 = uv[i];
                buffer.Add(v);
            }
            if (tri == null)
            {
                tri = new List<int>();
                tri.AddRange(trileft);
            }
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Refresh();
            vh.AddUIVertexStream(buffer, tri);
        }
#if DEBUG
    float tmp;
    private void Update()
    {
        if (tmp != dock)
        {
            if (dock > dockMax)
                dock = dockMax;
            SetVerticesDirty();
            tmp = dock;
        }
    }
#endif
    }
}