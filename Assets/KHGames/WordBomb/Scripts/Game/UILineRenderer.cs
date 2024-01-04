using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    public Vector2 startPoint = new Vector2(0, 0);
    public Vector2 endPoint = new Vector2(100, 100);
    public float lineThickness = 5f;
    public int segments = 30;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        var perpendicular = new Vector2(startPoint.y - endPoint.y, endPoint.x - startPoint.x).normalized *
                            lineThickness;
        
        var lineVertices = new Vector2[]
        {
            startPoint - perpendicular,
            startPoint + perpendicular,
            endPoint + perpendicular,
            endPoint - perpendicular
        };
        for (int i = 0; i < lineVertices.Length; i++)
        {
            vh.AddVert(lineVertices[i], color, Vector2.zero);
        }

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    public override bool Raycast(Vector2 sp, Camera eventCamera)
    {
        return false;
    }
}