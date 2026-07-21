using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class HUDShieldGraphic : MaskableGraphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); Rect r=GetPixelAdjustedRect(); Vector2 c=r.center; float w=r.width*.38f,h=r.height*.43f;
        Vector2[] outer={c+new Vector2(-w,h),c+new Vector2(w,h),c+new Vector2(w*.88f,-h*.2f),c+new Vector2(0,-h),c+new Vector2(-w*.88f,-h*.2f)};
        Vector2[] inner={c+new Vector2(-w*.68f,h*.68f),c+new Vector2(w*.68f,h*.68f),c+new Vector2(w*.58f,-h*.1f),c+new Vector2(0,-h*.68f),c+new Vector2(-w*.58f,-h*.1f)};
        AddPolygon(vh,outer,new Color(.02f,.72f,1f,1f)); AddPolygon(vh,inner,new Color(.05f,.22f,.75f,1f));
        AddPolygon(vh,new[]{inner[0],c+new Vector2(0,h*.68f),c+new Vector2(0,-h*.68f),inner[3],inner[4]},new Color(.12f,.62f,1f,.9f));
        AddLoop(vh,outer,3f,Color.white);
    }
    static void AddPolygon(VertexHelper vh,Vector2[] p,Color color){int s=vh.currentVertCount;foreach(var v in p)vh.AddVert(v,color,Vector2.zero);for(int i=1;i<p.Length-1;i++)vh.AddTriangle(s,s+i,s+i+1);}
    static void AddLoop(VertexHelper vh,Vector2[] p,float width,Color color){for(int i=0;i<p.Length;i++)AddLine(vh,p[i],p[(i+1)%p.Length],width,color);}
    static void AddLine(VertexHelper vh,Vector2 a,Vector2 b,float width,Color color){Vector2 n=new Vector2(-(b.y-a.y),b.x-a.x).normalized*width*.5f;int s=vh.currentVertCount;vh.AddVert(a-n,color,Vector2.zero);vh.AddVert(a+n,color,Vector2.zero);vh.AddVert(b+n,color,Vector2.zero);vh.AddVert(b-n,color,Vector2.zero);vh.AddTriangle(s,s+1,s+2);vh.AddTriangle(s,s+2,s+3);}
}
