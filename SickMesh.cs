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

    bool blah = false;

    Vector3[] vertices;

    public Vector3[] rendVertices;


    public Terrainnnn testTerrain;

    public Terrainnnn[] terrains;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendHeight = height + (height * fillPointCount);
        rendWidth = width + (width * fillPointCount);
        xRendVertDist = widthUnit / (fillPointCount + 1);
        yRendVerDist = heightUnit / (fillPointCount + 1);

        CreateRendVerticies();
        CreateVetices();
        CreateRendShapes();

        refereshRenderedPoints();
        //applyTestTerrain();

        //applyTerrain2(testTerrain);


        //applyTerrain(testTerrain);
    }

    private void Update()
    {
        generateTerrain();
        moveMesh();
        rendUpdateMesh();
    }



    void CreateVetices()
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

    void CreateRendVerticies()
    {
        rendVertices = new Vector3[(rendHeight + 1) * (rendWidth + 1)];

        for (int i = 0, vertIndex = 0; i <= rendHeight; i++)
        {
            for (int k = 0; k <= rendWidth; k++)
            {
                //need to do - i so that it goes top down
                rendVertices[vertIndex] = new Vector3(k * xRendVertDist, 0, (rendHeight - i) * yRendVerDist);
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
        if (rendVertices == null || !drawPoints) { return; }


        for (int i = 0; i < rendVertices.Length; i++)
        {
            Vector3 point = rendVertices[i];
            if (point.z <= height - topMargin
                && point.z >= bottomMargin
                && point.x <= width - rightMargin
                && point.x >= leftMargin)
            {
                Gizmos.DrawSphere(point, 0.05f);
            }
            
        }


        return;
        for (int k = 0; k < vertices.Length; k++)
        {
            Gizmos.DrawSphere(vertices[k], 0.05f);
        }
        
    }

    void applyTestTerrain()
    {
        if (testTerrain == null) { return; }

        if (testTerrain.terrainData.Length == 0) { return; }

        testTerrain.reCalcVertecies(fillPointCount);

        int offset = 0;
        float positionOffset = fillPointCount == 0 ? 0 : 1f / (float)(fillPointCount + 1) * fillPointCount;

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
    }


    void applyTerrain2(Terrainnnn terrain)
    {
        int offset = 0;

        Vector3 heightOffset = new Vector3(0, 0, vertices[0].z - terrain.height + 1);

        //here not <= because terrain heights are the vertecie count
        for (int i = 0; i < terrain.height; i++)
        {
            for (int k = 0; k < terrain.width; k++)
            {
                int vertIndex = i * width + k + offset;
                int terrainIndex = i * terrain.width + k;
                float actualElevation = Mathf.Max(vertices[vertIndex].y, (terrain.terrainData[terrainIndex] + heightOffset).y);
                vertices[vertIndex] = terrain.terrainData[terrainIndex] + heightOffset;
                vertices[vertIndex] = new Vector3(vertices[vertIndex].x, actualElevation, vertices[vertIndex].z);
            }
            offset++;
        }

        refereshRenderedPoints();
    }

    void applyTerrain(Terrainnnn terrain)
    {
        terrain.reCalcVertecies(fillPointCount);

        int offset = 0;
        float positionOffset = fillPointCount == 0 ? 0 : 1f / (float)(fillPointCount + 1) * fillPointCount;
        //positionOffset = 

        Vector3[] offSetTerrainData = terrain.offSetData(height - terrain.height + positionOffset);

        for (int i = 0; i < terrain.rendHeight; i++)
        {

            for (int k = 0; k < terrain.rendWidth; k++)
            {
                int vertIndex = i * rendWidth + k + offset;
                int terrainIndex = i * terrain.rendWidth + k;

                Vector3 nonUnitAdjusted = offSetTerrainData[terrainIndex];
                rendVertices[vertIndex] = new Vector3(widthUnit * nonUnitAdjusted.x, nonUnitAdjusted.y, heightUnit * nonUnitAdjusted.z);
            }
            offset++;
        }
    }


    void rendUpdateMesh()
    {

       

        mesh.vertices = rendVertices;
        if (drawShapes) { mesh.triangles = triangleRenIndices; }

        CreateRendShapes();
        mesh.RecalculateNormals();
    }


    void generateTerrain()
    {
        if (blah) { return; }
        int terrainSelector = Mathf.FloorToInt(Random.Range(0, terrains.Length-1));
        float likelihoodSelector = Random.Range(0.00001f, 1f);

        Terrainnnn selectedTerrain = terrains[terrainSelector];

        if (likelihoodSelector <= selectedTerrain.appearanceLikelihood)
        {
            //applyTerrain(selectedTerrain);
            applyTerrain2(selectedTerrain);
        }
    }


    void moveMesh()
    {

        for (int i = 0; i < rendVertices.Length; i++)
        {
            rendVertices[i] += new Vector3(0, 0, -treadmillSpeed);
        }

        for (int k = 0; k < vertices.Length; k++)
        {
            vertices[k] += new Vector3(0, 0, -treadmillSpeed);
        }

        //incase points move so much that need to move rows more than once
        while (applyTreadmillEffect()) { }
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
                    vertices[k] = new Vector3(k, 0, shortenedList[0].z + 1);
                }
                else
                {
                    vertices[k] = shortenedList[k - width - 1];
                }
            }
            return true;
        }

        return false;




        if (rendVertices[0].z < (rendHeight-1) * heightUnit/(fillPointCount+1))
        {

            Vector3[] shortenedList = new Vector3[rendVertices.Length - (rendWidth + 1)];
            for (int i = 0; i < shortenedList.Length; i++)
            {
                shortenedList[i] = rendVertices[i];
            }


           for (int k = 0; k < rendVertices.Length; k++)
            {
                //if first row of points
                if (k <= rendWidth)
                {
                    //need to be placed properly behind first row, so use shortenedList[0].z + yRendVerDist
                    rendVertices[k] = new Vector3(k * xRendVertDist, 0, shortenedList[0].z + yRendVerDist);
                    
                }
                else
                {
                    rendVertices[k] = shortenedList[k - rendWidth - 1];
                }
                
            }

            return true;
        }

        return false;
    }






    void refereshRenderedPoints()
    {
        int rvHeight = height + fillPointCount * (height);
        int rvWidth = width + fillPointCount * (width);

        Vector3[] renderedVerts = new Vector3[(rvHeight+1) * (rvWidth+1)];

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


}


