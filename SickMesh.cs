using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//requires mesh
[RequireComponent(typeof(Mesh))]
public class SickMesh : MonoBehaviour
{
    

    Mesh mesh;
    Vector3[] vertices;

    int[] trinagleRenPnts;

    public bool drawShapes = true;

    public int height = 10;
    public int width = 5;
    public int fillPointCount = 4;


    int[] rendTrinagleRenPnts;

    int rendHeight;
    int rendWidth;
    float rendMultiplyer;

    Vector3[] rendVertices;


    public Terrainnnn testTerrain;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendHeight = height + (height * fillPointCount);
        rendWidth = width + (width * fillPointCount);
        rendMultiplyer = 1f / (fillPointCount + 1);

        //StartCoroutine(CreateVerticiesShapes());
        //CreateVerticies();
        CreateRendVerticies();
        CreateRendShapes();
        //CreateShapes();


    }

    private void Update()
    {
        rendUpdateMesh();
        //UpdateMesh();
    }


    void CreateRendVerticies()
    {
        rendVertices = new Vector3[(rendHeight + 1) * (rendWidth + 1)];

        for (int i = 0, vertIndex = 0; i <= rendHeight; i++)
        {
            for (int k = 0; k <= rendWidth; k++)
            {
                //vertices[i * width + k] = new Vector3(i, 0, k);
                rendVertices[vertIndex] = new Vector3(k, 0, (rendHeight - i - 1)) * rendMultiplyer;
                vertIndex++;

            }


        }
    }

    void CreateVerticies()
    {
        vertices = new Vector3[(height + 1) * (width + 1)];

        for (int i = 0, vertIndex = 0; i <= height; i++)
        {
            for (int k = 0; k <= width; k++)
            {
                //vertices[i * width + k] = new Vector3(i, 0, k);
                vertices[vertIndex] = new Vector3(k, 0, (height - i -1));
                vertIndex++;

            }


        }

    }

    void CreateRendShapes()
    {
        rendTrinagleRenPnts = new int[rendHeight * rendWidth * 6];
        int vert = 0;
        int triIndex = 0;

        for (int i = 0; i < rendHeight; i++)
        {
            for (int k = 0; k < rendWidth; k++)
            {
                //verticies are from right to left, and moves a row down

                //draws from bottom left to top right, clockwise
                rendTrinagleRenPnts[triIndex] = vert;
                rendTrinagleRenPnts[triIndex + 1] = vert + 1;
                //need to add 2 because moved over a column (one extra vertice per column)
                rendTrinagleRenPnts[triIndex + 2] = vert + rendWidth + 2;

                //draws from top right to bottom left, clockwise
                rendTrinagleRenPnts[triIndex + 3] = vert + rendWidth + 2;
                rendTrinagleRenPnts[triIndex + 4] = vert + rendWidth + 1;
                rendTrinagleRenPnts[triIndex + 5] = vert;
                vert++;
                triIndex += 6;

            }
            vert++;
        }
    }

    void CreateShapes()
    {

        trinagleRenPnts = new int[height * width * 6];
        int vert = 0;
        int triIndex = 0;

        for (int i = 0; i < height; i++)
        {
            for (int k = 0; k < width; k++)
            {
                //verticies are from right to left, and moves a row down

                //draws from bottom left to top right, clockwise
                trinagleRenPnts[triIndex] = vert;
                trinagleRenPnts[triIndex + 1] = vert + 1;
                //need to add 2 because moved over a column (one extra vertice per column)
                trinagleRenPnts[triIndex + 2] = vert + width + 2; 

                //draws from top right to bottom left, clockwise
                trinagleRenPnts[triIndex + 3] = vert + width + 2;
                trinagleRenPnts[triIndex + 4] = vert + width + 1;
                trinagleRenPnts[triIndex + 5] = vert;
                vert++;
                triIndex += 6;

            }
            vert++;
        }
        


    }

    private void OnDrawGizmos()
    {
        //oldGizomsShit();
        GizmosShit();
    }


    void GizmosShit()
    {
        if (rendVertices == null) { return; }

        for (int i = 0; i < rendVertices.Length; i++)
        {
            Gizmos.DrawSphere(rendVertices[i], 0.05f);
        }
    }

    void oldGizomsShit()
    {
        if (vertices == null) { return; }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }


    void rendUpdateMesh()
    {

        
       

        //mesh.RecalculateNormals();

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

                rendVertices[vertIndex] = offSetTerrainData[terrainIndex];
            }
            offset++;
        }

        mesh.vertices = rendVertices;
        if (drawShapes) { mesh.triangles = rendTrinagleRenPnts; }

        CreateRendShapes();
        mesh.RecalculateNormals();
    }




    void UpdateMesh()
    {
        mesh.vertices = vertices;
        if (drawShapes) { mesh.triangles = trinagleRenPnts; }
        //for recalculating normals with lighting
        mesh.RecalculateNormals();

        if (testTerrain == null) { return; }

        if (testTerrain.terrainData.Length == 0) { return; }


        int offset = 0;
        Vector3[] offSetTerrainData = testTerrain.offSetData(height - testTerrain.height);

        for (int i = 0; i < testTerrain.height; i++)
        {

            for (int k = 0; k < testTerrain.width; k++)
            {
                int vertIndex = i * width + k + offset;
                int terrainIndex = i * testTerrain.width + k;

                vertices[vertIndex] = offSetTerrainData[terrainIndex];
            }
            offset++;
        }

        CreateShapes();
        mesh.RecalculateNormals();
    }
}


