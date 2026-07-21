using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class HUDHealthBarGraphic : MaskableGraphic
{
    [SerializeField,Range(0f,1f)] private float fill=1f;
    public void SetFill(float value){fill=Mathf.Clamp01(value);SetVerticesDirty();}
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); Rect r=GetPixelAdjustedRect(); AddQuad(vh,r,new Color(.01f,.04f,.1f,.98f));
        Rect inner=new(r.xMin+4,r.yMin+4,r.width-8,r.height-8); float filled=inner.width*fill;
        int segments=20; float gap=2f; float segmentWidth=(inner.width-gap*(segments-1))/segments;
        for(int i=0;i<segments;i++){float x=inner.xMin+i*(segmentWidth+gap);if(x>=inner.xMin+filled)break;float shown=Mathf.Min(segmentWidth,inner.xMin+filled-x);float t=i/(float)(segments-1);Color col=Color.Lerp(new Color(.04f,.65f,1f),new Color(1f,.16f,.22f),t);AddQuad(vh,new Rect(x,inner.yMin,shown,inner.height),col);AddLine(vh,new Vector2(x+2,inner.yMin),new Vector2(x+shown-2,inner.yMax),1.5f,new Color(1,1,1,.25f));}
        AddLoop(vh,r,2f,new Color(.12f,.78f,1f,.9f));
    }
    static void AddQuad(VertexHelper vh,Rect r,Color c){int s=vh.currentVertCount;vh.AddVert(new Vector2(r.xMin,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMin,r.yMax),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMax),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMin),c,Vector2.zero);vh.AddTriangle(s,s+1,s+2);vh.AddTriangle(s,s+2,s+3);}
    static void AddLoop(VertexHelper vh,Rect r,float w,Color c){AddLine(vh,new(r.xMin,r.yMin),new(r.xMax,r.yMin),w,c);AddLine(vh,new(r.xMax,r.yMin),new(r.xMax,r.yMax),w,c);AddLine(vh,new(r.xMax,r.yMax),new(r.xMin,r.yMax),w,c);AddLine(vh,new(r.xMin,r.yMax),new(r.xMin,r.yMin),w,c);}
    static void AddLine(VertexHelper vh,Vector2 a,Vector2 b,float w,Color c){Vector2 n=new Vector2(-(b.y-a.y),b.x-a.x).normalized*w*.5f;int s=vh.currentVertCount;vh.AddVert(a-n,c,Vector2.zero);vh.AddVert(a+n,c,Vector2.zero);vh.AddVert(b+n,c,Vector2.zero);vh.AddVert(b-n,c,Vector2.zero);vh.AddTriangle(s,s+1,s+2);vh.AddTriangle(s,s+2,s+3);}
}
