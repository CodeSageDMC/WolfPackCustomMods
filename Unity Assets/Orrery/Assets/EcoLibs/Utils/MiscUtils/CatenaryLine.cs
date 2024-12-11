// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

///<summary>
///Represents a catenary line that goes from point A to point B.
///A catenary is the curve that an idealized hanging chain or cable assumes under its own weight when supported only at its ends in a uniform gravitational field. More information on: https://en.wikipedia.org/wiki/Catenary
///Used by ropes that attach a boat to a moorage post. Can also be used by wires.
///</summary>
[ExecuteInEditMode]
public class CatenaryLine : MonoBehaviour
{
    public float        DistanceBetweenPoints  = 0.2f;      //Distance between each point of the line in the renderer. The smaller this value is, the better the line will look.
    public float        LineCatenary           = 5f;        //Catanary that will be used for the line. The higher this value is, the more stretched the line will be.

    //Point A and point B represent the start and end points of the line (which one is which does not matter).
    public Transform    PointA { get; set; }
    public Transform    PointB { get; set; }

    Vector2             originalTextureTiling;              //Contains the original material tiling. Will be used to calculate a new tiling according to the line size.
    LineRenderer        lineRenderer;

    void Awake()
    {
        this.lineRenderer          = GetComponent<LineRenderer>();
        this.originalTextureTiling = this.lineRenderer.material.mainTextureScale;
    }

    void Update() => GenerateLine();

    ///<summary>Generates a catenary line between point A and point B.</summary>
    void GenerateLine()
    {
        if (PointA == null || PointB == null) return;

        var lineDistance                = Vector3.Distance(PointA.position, PointB.position);
        var pointCount                  = Mathf.RoundToInt(lineDistance / DistanceBetweenPoints);                       //Number of points between the start and the end of the line.

        //Used to guarantee that all points are at the exact same distance from each other.
        //If we don't use it, and only use distance between points that is set in inspector, if we have e.g. 1 point between start and end, the point may not be at the center of the line.
        var realDistanceBetweenPoints = lineDistance / pointCount;

        var linePoints                  = new Vector3[pointCount + 1];                                                  //Contains all positions that will be used to render the line.
        linePoints[0]                   = PointA.position;
        linePoints[pointCount]          = PointB.position;

        var lineDirection               = (PointB.position - PointA.position).normalized;
        var offset                      = CalculateCatenary(LineCatenary, -lineDistance / 2f);

        for (int i = 1; i < pointCount; i++)                                                                            //Iterate through all points (except for the first and last points, as we already know their positions).
        {
            var linePoint               = PointA.position + i * realDistanceBetweenPoints * lineDirection;              //Calculate the position of the point.

            var x                       = i * realDistanceBetweenPoints - lineDistance / 2f;
            linePoint.y                 = linePoint.y - (offset - CalculateCatenary(LineCatenary, x));

            linePoints[i]               = linePoint;
        }

        this.lineRenderer.positionCount = linePoints.Length;                                                            //Set the point positions on line renderer.

        for (var i = 0; i < linePoints.Length; i++)
            this.lineRenderer.SetPosition(i, linePoints[i]);

        this.lineRenderer.material.mainTextureScale = new Vector2(this.originalTextureTiling.x * lineDistance, 1f);     //Set the tiling for line material.
    }

    float CalculateCatenary(float a, float x) => a * MathUtils.CalculateHyperbolicCosine(x / a);
}
