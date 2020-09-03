﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//requires mesh
[RequireComponent(typeof(Mesh))]
public class SickMesh : MonoBehaviour
{

    const float FPS = 60f;

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
    public float heightUnit = 1;
    public float widthUnit = 1;


    bool canGenerateOthers = true;

    int changeLaneDir = 0;
    public float changeLaneTime = 1.5f;
    float totalFrameCounter = 0f;
    float framesForLaneChange;
    float changeLaneStep;
    float xLaneChangePositionProgress = 0f;


    public Vector3[] vertices;

    public Vector3[] rendVertices;

    public Terrainnnn[] terrains;
    public List<TerrObject> terrObjects;

    //list for value because could generate more than one instance before control point
    //index applies treadmill effect
    public Dictionary<int, List<TerrObject>> currTerrObjsDict = new Dictionary<int, List<TerrObject>>();

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendHeight = height + (height * fillPointCount);
        rendWidth = width + (width * fillPointCount);

        framesForLaneChange = changeLaneTime * FPS;

        changeLaneStep = 1f / framesForLaneChange;

        for (int i = 0; i < terrains.Length; i++)
        {
            terrains[i] = Instantiate(terrains[i]);
        }

        createControlVertices();
        refreshRenderedPoints();
    }

    void createControlVertices()
    {
        vertices = new Vector3[(height + 1) * (width + 1)];

        for (int i = 0, vertIndex = 0; i <= height; i++)
        {
            for (int k = 0; k <= width; k++)
            {
                vertices[vertIndex] = new Vector3(k * widthUnit, 0, (height - i) * heightUnit);
                vertIndex++;
            }
        }

    }



    void refreshRenderedPoints()
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

        float curveMultiplyer = easingFunction(theta);
        float lerpMultiplyer = (float)(index + 1) / (float)(fillPointCount + 1);

        return (new Vector3(diff.x * lerpMultiplyer, diff.y * curveMultiplyer, diff.z * lerpMultiplyer) + firstRefPoint);

    }

    float easingFunction(float theta)
    {
        return Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;
    }




    private void OnDrawGizmos()
    {
        if (rendVertices == null || !drawPoints) { return; }

        Vector3[] points = drawOnlyControlPoints ? vertices : rendVertices;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i];
            if (point.z <= (height - topMargin) * heightUnit
                && point.z >= bottomMargin * heightUnit
                && point.x <= (width - rightMargin) * widthUnit
                && point.x >= leftMargin * widthUnit)
            {
                Gizmos.DrawSphere(point, 0.05f);
            }
        }
    }



    private void Update()
    {
        generateTerrain();
        generateTerrObjs();
        inputUpdate();
        moveMesh();
        updateMesh();
    }


    void generateTerrain()
    {
        if (!canGenerateOthers) { return; }
        foreach (Terrainnnn terr in terrains)
        {
            float likelihoodSelector = Random.Range(0.00001f, 1f);
            if (likelihoodSelector <= terr.appearanceLikelihood && terr.canGen())
            {
                terr.startGenerateDelay();
                addTerrain(terr);

                if (!terr.canGenerateOthers)
                {
                    float delay = (terr.vertH() - 1) * (treadmillSpeed * FPS) ;
                    generateOthersDelay(delay);
                }
            }
        }
        
    }

    void generateTerrObjs()
    {
        foreach (TerrObject terrObj in terrObjects)
        {
            float likelihoodSelector = Random.Range(0.00001f, 1f);
            if (likelihoodSelector <= terrObj.appearanceLikelihood)
            {
                addTerrObj(terrObj);
            }
        }
    }

    IEnumerator generateOthersDelay(float delay)
    {
        canGenerateOthers = false;
        yield return new WaitForSecondsRealtime(delay);
        canGenerateOthers = true;
    }



    void addTerrain(Terrainnnn terrain)
    {
        int offset = 0;

        Vector3 posOffset = new Vector3(vertices[0].x, 0, vertices[0].z - (terrain.vertH() * heightUnit) + heightUnit);

        //here not <= because terrain heights are the vertecie count

        //todo: wierd shit with 30 exact? bigger nums?
        int startOffset = Random.Range(0, width + 1);

        for (int i = 0; i < terrain.vertH(); i++)
        {
            for (int k = 0; k < terrain.vertW(); k++)
            {
                int kOffset = (startOffset + k) % (width + 1);
                int horzDiff = kOffset - k;

                int vertIndex = i * width + kOffset + offset;
                int terrainIndex = i * terrain.vertW() + k;

                Vector3 terrPnt = terrain.terrainData[terrainIndex];

                float actualElevation = terrain.canGenerateOthers && terrPnt.y >= 0f? Mathf.Max(vertices[vertIndex].y, terrPnt.y) : terrPnt.y;

                vertices[vertIndex] = new Vector3((terrPnt.x + horzDiff) * widthUnit, actualElevation, terrPnt.z * heightUnit) + posOffset;
            }
            offset++;
        }

        refreshTerrObjDict();
        refreshRenderedPoints();
    }


    void refreshTerrObjDict()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (currTerrObjsDict.ContainsKey(i))
            {
                for (int k = 0; k < currTerrObjsDict[i].Count; k++)
                {
                    TerrObject terrObject = currTerrObjsDict[i][k];
                    Vector3 ogPos = terrObject.transform.position;
                    Vector3 firstRef = vertices[i];
                    Vector3 secondRef = firstRef;

                    float zPosDiff = ogPos.z - firstRef.z;


                    //behind vertices[i].z point
                    if (zPosDiff > 0 && (i - width - 1) >= 0)
                    {
                        //get ref point behind
                        secondRef = vertices[i - width - 1];
                    }
                    //in front
                    else if (zPosDiff < 0 && (i + width + 1) < vertices.Length )
                    {
                        secondRef = vertices[i + width + 1];
                    }

                    float actualYPosDiff = 0;
                    if (secondRef.y != firstRef.y)
                    {
                        float theta = zPosDiff / (secondRef.z - firstRef.z);
                        float yDiff = secondRef.y - firstRef.y;
                        actualYPosDiff = yDiff * easingFunction(theta);
                    }

                    currTerrObjsDict[i][k].transform.position = new Vector3(ogPos.x, firstRef.y + actualYPosDiff + hitBoxElevOffset(terrObject), ogPos.z);
                }
            }
        }
    }


    void addTerrObj(TerrObject terrObj)
    {
        int index = Random.Range(0, width + 1);

        float vertOffset = Random.Range(-(heightUnit) / 2f, heightUnit / 2f);//TerrObject.OBJ_TYPE.STATIC ? Random.Range(-(heightUnit) / 2f, heightUnit / 2f) : 0f;

        if (terrObj.objType == TerrObject.OBJ_TYPE.STATIC_HAZ && !canAddToThisLane(index, vertOffset)) { return; }

        
        terrObj.hitBox.size = new Vector3(terrObj.hitBoxUnitWidth * widthUnit / terrObj.transform.localScale.x, terrObj.hitBox.size.y, terrObj.hitBox.size.z);

        //if its not a hazard, meaning it doesnt have to be in a designated lane, can be anywhere in the horz.
        float horizOffset = terrObj.objType == TerrObject.OBJ_TYPE.STATIC ? Random.Range(-(widthUnit) / 2f, widthUnit / 2f) : 0f;
        float flipMultiplyer = terrObj.canFlip ? Random.Range(0, 2) * 2 - 1 : 1;

        TerrObject terrObjInst = Instantiate(terrObj);
        terrObjInst.RandomizeSpriteType();

        terrObjInst.gameObject.GetComponent<SpriteRenderer>().flipX = flipMultiplyer > 0? false : true;

        Vector3 pos = vertices[index] + new Vector3(horizOffset, hitBoxElevOffset(terrObj), vertOffset);

        terrObjInst.transform.position = pos;
        //currTerrObjs.Add(terrObjInst);
        if (!currTerrObjsDict.ContainsKey(index))
        {
            currTerrObjsDict.Add(index, new List<TerrObject>());
        }

        currTerrObjsDict[index].Add(terrObjInst);
    }


    bool canAddToThisLane(int laneIndex, float vertOffset)
    {

        for (int currIndex = laneIndex; currIndex < vertices.Length; currIndex  += (width + 1))
        {
            if (currTerrObjsDict.ContainsKey(currIndex))
            {

                for (int i = 0; i < currTerrObjsDict[currIndex].Count; i++)
                {
                    TerrObject terrObj = currTerrObjsDict[currIndex][i];

                    float diffDist2TO = (vertices[laneIndex].z + vertOffset) - (terrObj.transform.position.z);

                    if (diffDist2TO < terrObj.minHeightUnitsForNextHaz * heightUnit)
                    {
                        return false;
                    } 

                }

            }
        }

        return true;
        

    }


    float hitBoxElevOffset(TerrObject terrObj)
    {
        return (terrObj.hitBox.size.y / 2f * terrObj.transform.localScale.y) * (1f -  terrObj.elevationOffsetPerc);
    }

    void inputUpdate()
    {
        int dir = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)){ dir++; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){ dir--; }

        changeLane(dir);
    }


    void changeLane(int dir)
    {
        if (changeLaneDir != 0) { return; }

        changeLaneDir = dir;


    }


    void moveMesh()
    {
        //for controling lane changing

        float change = 0f;

        if (changeLaneDir != 0)
        {
            if (totalFrameCounter > framesForLaneChange)
            {
                changeLaneDir = 0;
                totalFrameCounter = 0f;
                xLaneChangePositionProgress = 0f;
            }

            else
            {
                totalFrameCounter++;

                float theta = changeLaneStep * totalFrameCounter * Mathf.PI;
                float progressVal = easingFunction(theta);

                float currDist = Mathf.Lerp(0, widthUnit * changeLaneDir, progressVal);
                change = currDist - xLaneChangePositionProgress;
                xLaneChangePositionProgress = currDist;
            }
        }



        //applying treadmill move and lane change
        for (int i = 0; i < rendVertices.Length; i++)
        {
            rendVertices[i] += new Vector3(change / (float)(fillPointCount + 1), 0, -treadmillSpeed);
        }


        for (int k = 0; k < vertices.Length; k++)
        {


            vertices[k] += new Vector3(change, 0, -treadmillSpeed);
        }


        foreach (int key in currTerrObjsDict.Keys)
        {
            for (int t = 0; t < currTerrObjsDict[key].Count; t++)
            {
                currTerrObjsDict[key][t].transform.position += new Vector3(change, 0, -treadmillSpeed);
            }
            
        }


        //incase points move so much that need to move rows more than once
        //fire safety here too lol
        int safety = 0;
        while (applyTreadmillEffect()) { safety++; if (safety > 10) { break; } }
        safety = 0;
        while (applyHorizontalWrap()) { safety++; if (safety > 10) { break; } }
        refreshRenderedPoints();
    }



    bool applyTreadmillEffect()
    {

        if (vertices[0].z < (height - 1) * heightUnit)
        {
            Vector3[] shortenedList = new Vector3[vertices.Length - (width + 1)];

            Vector3[] prevVertices = (Vector3[])vertices.Clone();

            for (int i = 0; i < shortenedList.Length; i++)
            {
                shortenedList[i] = vertices[i];
            }

            for (int k = 0; k < vertices.Length; k++)
            {
                if (k <= width)
                {
                    vertices[k] = new Vector3(shortenedList[0].x + k * widthUnit, 0, shortenedList[0].z + heightUnit);
                }
                else
                {
                    vertices[k] = shortenedList[k - width - 1];
                }
            }



            //now adjust index (which are the keys for the dictionary) so that they
            //change by one row

            List<int> terrKeys2Remove = new List<int>();

            foreach (int key in currTerrObjsDict.Keys)
            {

                if (key >= vertices.Length - (width+1))
                {
                    terrKeys2Remove.Add(key);
                    for (int t = 0; t < currTerrObjsDict[key].Count; t++)
                    {
                        Destroy(currTerrObjsDict[key][t].gameObject);
                    }
                    
                }
            }

            foreach (int key in terrKeys2Remove)
            {
                currTerrObjsDict.Remove(key);
            }

            Dictionary<int, List<TerrObject>> copy = new Dictionary<int, List<TerrObject>>();

            foreach (int key in currTerrObjsDict.Keys)
            {
                int nextRowIndex = key + width + 1;
                copy.Add(nextRowIndex, new List<TerrObject>());
                for (int t = 0; t < currTerrObjsDict[key].Count; t++)
                {
                    copy[nextRowIndex].Add(currTerrObjsDict[key][t]);
                }
            }

            currTerrObjsDict = copy;

            return true;
        }

        return false;

    }


    bool applyHorizontalWrap()
    {
        if (vertices[0].x > widthUnit || vertices[0].x < -widthUnit)
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
                if ((k + invModifier) % (width + 1) == 0)
                {
                    //subtract or add the index depending on col on
                    Vector3 newPnt = vertCopy[k + width * dirModifier];

                    //move x by one control unit left or right depending on dir
                    vertices[k] = new Vector3(vertCopy[k].x - dirModifier * widthUnit, newPnt.y, newPnt.z);
                    offset++;
                }
                else
                {
                    vertices[k] = shortenedList[k - offset];
                }
            }



            Dictionary<int, List<TerrObject>> currTerrObjsCopy = new Dictionary<int, List<TerrObject>>();


            //now adjust terr objects

            foreach (int key in currTerrObjsDict.Keys)
            {
                // 0 (or multiple of width + 1) mod (width + 1) = 0, modifier == 0 when moving left
                // (width + 1) - 1 mod (width + 1) = (width + 1) - 1, modifier == 1 when moving right
                int newIndex = (key) % (width + 1) == width * modifier ? key - width * dirModifier : key + dirModifier;
                float offset = (key) % (width + 1) == width * modifier ? -(width + 1) * widthUnit * dirModifier : 0;

                currTerrObjsCopy.Add(newIndex, new List<TerrObject>());

                for (int i = 0; i < currTerrObjsDict[key].Count; i++)
                {
                    currTerrObjsDict[key][i].transform.position += new Vector3(offset, 0, 0);
                    currTerrObjsCopy[newIndex].Add(currTerrObjsDict[key][i]);
                }
            }

            currTerrObjsDict = currTerrObjsCopy;


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


