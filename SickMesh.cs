using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//requires mesh
[RequireComponent(typeof(Mesh))]
public class SickMesh : MonoBehaviour
{


    Mesh mesh;

    public bool drawShapes = true;
    public bool drawPoints = true;
    public bool drawOnlyControlPoints = false;

    //control point height and width
    public int height = 10;
    public int width = 5;
    //number of points to fill between control points (can be zero too)
    public int fillPointCount = 4;

    public int topMargin = 5;
    public int bottomMargin = 5;
    public int leftMargin = 0;
    public int rightMargin = 0;

    public float treadmillSpeed = 0.05f;
    public float horizontalWrapSpeed = 0.05f;

    int[] triangleRenIndices;


    //actual render height and width with repsect to a unit
    //of 1 fill shape length or width
    int rendHeight;
    int rendWidth;

    //length between two control points (fill points will adjust to this)
    //public float heightUnit = 1;
    //public float widthUnit = 1;

    //distance between rendered vertecies
    //float xRendVertDist;
    //float yRendVerDist;


    public Vector3[] vertices;

    public Vector3[] rendVertices;


    public Terrainnnn[] terrains;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendHeight = height + (height * fillPointCount);
        rendWidth = width + (width * fillPointCount);
        //xRendVertDist = widthUnit / (fillPointCount + 1);
        //yRendVerDist = heightUnit / (fillPointCount + 1);

        for(int i = 0; i < terrains.Length; i++)
        {
            terrains[i] = Instantiate(terrains[i]);
        }

        createControlVertices();
        refereshRenderedPoints();
    }

    void createControlVertices()
    {
        vertices = new Vector3[(height + 1) * (width + 1)];

        for (int i = 0, vertIndex = 0; i <= height; i++)
        {
            for (int k = 0; k <= width; k++)
            {
                vertices[vertIndex] = new Vector3(k, 0, height - i);
                vertIndex++;
            }
        }

    }



    void refereshRenderedPoints()
    {
        int rvHeight = height + fillPointCount * (height);
        int rvWidth = width + fillPointCount * (width);

        Vector3[] renderedVerts = new Vector3[(rvHeight + 1) * (rvWidth + 1)];

        //build list with full rows where control rows would be
        for (int i = 0, rowOffset = 0; i <= height; i++)
        {
            for (int k = 0, colOffset = 0; k <= width; k++)
            {
                int rvStartIndex = (i + rowOffset) * (rvWidth + 1) + k + colOffset;

                Vector3 firstRefPoint = vertices[i * (width + 1) + k];

                renderedVerts[rvStartIndex] = firstRefPoint;

                //no in between points on last vertecie
                if (k == width) { continue; }

                //fill in between points
                Vector3 secondRefPoint = vertices[i * (width + 1) + k + 1];

                for (int j = 0; j < fillPointCount; j++)
                {
                    renderedVerts[rvStartIndex + j + 1] = smoothERP(firstRefPoint, secondRefPoint, j);
                }


                colOffset += fillPointCount;
            }

            rowOffset += fillPointCount;
        }

        //now fill the rest of the empty rows
        for (int g = 0, rowOffset = 0; g < height; g++)
        {

            for (int w = 0; w <= rvWidth; w++)
            {
                int rvStartIndex = (g + rowOffset + 1) * (rvWidth + 1) + w;
                Vector3 firstRefPoint = renderedVerts[(g + rowOffset) * (rvWidth + 1) + w];
                Vector3 secondRefPoint = renderedVerts[(g + rowOffset + fillPointCount + 1) * (rvWidth + 1) + w];

                for (int q = 0; q < fillPointCount; q++)
                {
                    int index = rvStartIndex + q * (rvWidth + 1);
                    renderedVerts[index] = smoothERP(firstRefPoint, secondRefPoint, q);
                }

            }

            rowOffset += fillPointCount;
        }


        rendVertices = (Vector3[])renderedVerts.Clone();
    }




    Vector3 smoothERP(Vector3 firstRefPoint, Vector3 secondRefPoint, int index)
    {
        Vector3 diff = secondRefPoint - firstRefPoint;
        float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
        float theta = sinRadAng * (index + 1);

        float curveMultiplyer = Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;
        float lerpMultiplyer = (float)(index + 1) / (float)(fillPointCount + 1);

        return (new Vector3(diff.x * lerpMultiplyer, diff.y * curveMultiplyer, diff.z * lerpMultiplyer) + firstRefPoint);

    }





    private void OnDrawGizmos()
    {
        if (rendVertices == null || !drawPoints) { return; }

        Vector3[] points = drawOnlyControlPoints ? vertices : rendVertices;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i];
            if (point.z <= height - topMargin
                && point.z >= bottomMargin
                && point.x <= width - rightMargin
                && point.x >= leftMargin)
            {
                Gizmos.DrawSphere(point, 0.05f);
            }
        }
    }



    private void Update()
    {
        generateTerrain();
        moveMesh();
        updateMesh();
    }


    void generateTerrain()
    {
        int terrainSelector = Mathf.FloorToInt(Random.Range(0, terrains.Length - 1));
        float likelihoodSelector = Random.Range(0.00001f, 1f);

        Terrainnnn selectedTerrain = terrains[terrainSelector];

        if (likelihoodSelector <= selectedTerrain.appearanceLikelihood)
        {
            addTerrain(selectedTerrain);
        }
    }



    void addTerrain(Terrainnnn terrain)
    {
        int offset = 0;

        Vector3 posOffset = new Vector3(vertices[0].x, 0, vertices[0].z - terrain.vertHeight + 1);

        //here not <= because terrain heights are the vertecie count
        for (int i = 0; i < terrain.vertHeight; i++)
        {
            for (int k = 0; k < terrain.vertWidth; k++)
            {
                int vertIndex = i * width + k + offset;
                int terrainIndex = i * terrain.vertWidth + k;
                float actualElevation = Mathf.Max(vertices[vertIndex].y, (terrain.terrainData[terrainIndex]).y);
                vertices[vertIndex] = (terrain.terrainData[terrainIndex] + posOffset);
                vertices[vertIndex] = new Vector3(vertices[vertIndex].x, actualElevation, vertices[vertIndex].z);
            }
            offset++;
        }

        refereshRenderedPoints();
    }



    void moveMesh()
    {

        for (int i = 0; i < rendVertices.Length; i++)
        {
            rendVertices[i] += new Vector3(horizontalWrapSpeed, 0, -treadmillSpeed);
        }

        for (int k = 0; k < vertices.Length; k++)
        {
            vertices[k] += new Vector3(horizontalWrapSpeed, 0, -treadmillSpeed);
        }

        //incase points move so much that need to move rows more than once
        //fire safety here too lol
        int safety = 0;
        while (applyTreadmillEffect()) { safety++; if (safety > 10) { break; } }
        safety = 0;
        while (applyHorizontalWrap()) { safety++; if (safety > 10) { break; } }
        refereshRenderedPoints();
    }



    bool applyTreadmillEffect()
    {

        if (vertices[0].z < (height - 1))
        {
            Vector3[] shortenedList = new Vector3[vertices.Length - (width + 1)];

            for (int i = 0; i < shortenedList.Length; i++)
            {
                shortenedList[i] = vertices[i];
            }

            for (int k = 0; k < vertices.Length; k++)
            {
                if (k <= width)
                {
                    vertices[k] = new Vector3(shortenedList[0].x + k, 0, shortenedList[0].z + 1);
                }
                else
                {
                    vertices[k] = shortenedList[k - width - 1];
                }
            }
            return true;
        }

        return false;

    }


    bool applyHorizontalWrap()
    {
        //return false;
        if (vertices[0].x > 1 || vertices[0].x < -1)
        {
            bool movingRight = vertices[0].x > 1;
            int modifier = movingRight ? 1 : 0;
            int invModifier = movingRight ? 0 : 1;
            int dirModifier = movingRight ? 1 : -1;

            Vector3[] shortenedList = new Vector3[vertices.Length - (height + 1)];


            for (int i = 0, offset = 0; i < vertices.Length; i++)
            {
                //anything on the right most column for moving right, left col for moving left
                
                if ((i + modifier) % (width + 1) == 0) { offset++; continue; }

                shortenedList[i - offset] = vertices[i];
               
            }


            Vector3[] vertCopy = (Vector3[])vertices.Clone();

            for (int k = 0, offset = 0; k < vertices.Length; k++)
            {
                //if moving right, want to catch when on the left most col
                //moving left, catch the right most col
                if ((k+ invModifier) % (width + 1) == 0)
                {
                    //subtract or add the index depending on col on
                    Vector3 newPnt = vertCopy[k + width * dirModifier];

                    //move x by one control unit left or right depending on dir
                    vertices[k] = new Vector3(vertCopy[k].x - dirModifier, newPnt.y, newPnt.z);
                    offset++;
                }
                else
                {
                    vertices[k] = shortenedList[k - offset];
                }
            }

            return true;
        }

        return false;
    }



    void updateMesh()
    {
        mesh.vertices = rendVertices;

        if (drawShapes)
        {
            createRendShapes();
            mesh.triangles = triangleRenIndices;
            mesh.RecalculateNormals();
        }
    }



    void createRendShapes()
    {
        triangleRenIndices = new int[rendHeight * rendWidth * 6];
        int vert = 0;
        int triIndex = 0;

        for (int i = 0; i < rendHeight; i++)
        {
            for (int k = 0; k < rendWidth; k++)
            {
                //verticies are from right to left, and moves a row down

                //draws from top left to bottom right, clockwise
                triangleRenIndices[triIndex] = vert;
                triangleRenIndices[triIndex + 1] = vert + 1;
                //need to add 2 because moved over a column (one extra vertice per column)
                triangleRenIndices[triIndex + 2] = vert + rendWidth + 2;


                //draws from bottom right to top left, clockwise
                triangleRenIndices[triIndex + 3] = vert + rendWidth + 2;
                triangleRenIndices[triIndex + 4] = vert + rendWidth + 1;
                triangleRenIndices[triIndex + 5] = vert;
                vert++;
                triIndex += 6;

            }
            vert++;
        }
    }

}


