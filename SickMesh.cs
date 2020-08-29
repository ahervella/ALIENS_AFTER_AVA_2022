using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//requires mesh
[RequireComponent(typeof(Mesh))]
public class SickMesh : MonoBehaviour
{
    

    Mesh mesh;

    public bool drawShapes = true;

    //control point height and width
    public int height = 10;
    public int width = 5;
    //number of points to fill between control points (can be zero too)
    public int fillPointCount = 4;


    int[] triangleRenIndices;

    //actual render height and width after applying fill points
    int rendHeight;
    int rendWidth;

    //length between two control points (fill points will adjust to this)
    public float heightUnit = 1;
    public float widthUnit = 1;

    //distance between rendered vertecies
    float xRendVertDist;
    float yRendVerDist;

    Vector3[] rendVertices;


    public Terrainnnn testTerrain;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendHeight = height + (height * fillPointCount);
        rendWidth = width + (width * fillPointCount);
        xRendVertDist = widthUnit / (fillPointCount + 1);
        yRendVerDist = heightUnit / (fillPointCount + 1);

        CreateRendVerticies();
        CreateRendShapes();
    }

    private void Update()
    {
        rendUpdateMesh();
    }


    void CreateRendVerticies()
    {
        rendVertices = new Vector3[(rendHeight + 1) * (rendWidth + 1)];

        for (int i = 0, vertIndex = 0; i <= rendHeight; i++)
        {
            for (int k = 0; k <= rendWidth; k++)
            {
                //need to do - i - 1 so that it goes top down
                rendVertices[vertIndex] = new Vector3(k * xRendVertDist, 0, (rendHeight - i - 1) * yRendVerDist);
                vertIndex++;

            }


        }
    }


    void CreateRendShapes()
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

    

    private void OnDrawGizmos()
    {
        if (rendVertices == null) { return; }

        for (int i = 0; i < rendVertices.Length; i++)
        {
            Gizmos.DrawSphere(rendVertices[i], 0.05f);
        }
    }


    void rendUpdateMesh()
    {

        

        if (testTerrain == null) { return; }

        if (testTerrain.terrainData.Length == 0) { return; }

        testTerrain.reCalcVertecies(fillPointCount);

        int offset = 0;
        float positionOffset = fillPointCount == 0 ? 0 : 1f / (float)(fillPointCount+1) * fillPointCount;

        Vector3[] offSetTerrainData = testTerrain.offSetData(height - testTerrain.height + positionOffset);

        for (int i = 0; i < testTerrain.rendHeight; i++)
        {

            for (int k = 0; k < testTerrain.rendWidth; k++)
            {
                int vertIndex = i * rendWidth + k + offset;
                int terrainIndex = i * testTerrain.rendWidth + k;

                Vector3 nonUnitAdjusted = offSetTerrainData[terrainIndex];
                rendVertices[vertIndex] = new Vector3(widthUnit * nonUnitAdjusted.x, nonUnitAdjusted.y, heightUnit * nonUnitAdjusted.z);
            }
            offset++;
        }

        mesh.vertices = rendVertices;
        if (drawShapes) { mesh.triangles = triangleRenIndices; }

        CreateRendShapes();
        mesh.RecalculateNormals();
    }




}


