using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class Terrainnnn : MonoBehaviour
{
    const string terrainFolderPath = "Assets/RESOURCES/TERRAINS/";

    public TextAsset terrainFile;

    List<Vector3> tempTerrainData = new List<Vector3>();

    public Vector3[] terrainData;

    public int width = 0;
    public int height = 0;

    public int rendWidth = 0;
    public int rendHeight = 0;

    List<string[]> parsedData = new List<string[]>();

    private void Start()
    {
        parseTerrainFile();
    }

    public void parseTerrainFile()
    {
        if (terrainFile == null) { return; }

        StreamReader sr = new StreamReader(terrainFolderPath + terrainFile.name + ".txt");

        //List<<List<string>> lines = new
        //List<string[]> lines = new List<string[]>();

        string currLine = "";

        while ((currLine = sr.ReadLine()) != null)
        {
            parsedData.Add(currLine.Split(' '));

        }


        height = parsedData.Count;
        width = parsedData[0].Length;
        terrainData = new Vector3[width * height];

        reCalcVerteciesOld();

    }

    public void reCalcVertecies(int fillPointCount)

    {
        int rendPDHeight = parsedData.Count + fillPointCount * (parsedData.Count - 1);
        int rendPDWidth = parsedData[0].Length + fillPointCount * (parsedData[0].Length - 1);

        string[][] rendParsedData = new string[rendPDHeight][];
        for (int blah = 0; blah < rendParsedData.Length; blah++) { rendParsedData[blah] = new string[rendPDWidth]; }

        //build list with empty spots from fillPoint count
        for (int g = 0, rowOffset = 0; g < parsedData.Count; g++)
        {
            rendParsedData[g + rowOffset] = (new string[parsedData[g].Length + (fillPointCount * (parsedData[g].Length -1))]);

            for (int k = 0, colOffset = 0; k < parsedData[g].Length; k++)
            {
                rendParsedData[g + rowOffset][k + colOffset] = parsedData[g][k];

                colOffset += fillPointCount;
            }

            rowOffset += fillPointCount;

        }

        //calculate and populate in between empty spots between rows that already have stuff
        for (int l = 0; l < rendParsedData.Length; l++)
        {
            if (rendParsedData[l][0] == null) { continue; }


            for (int ll = 0; ll < rendParsedData[l].Length; ll++)
            {
                if (rendParsedData[l][ll] == null)
                {
                    float firstVal = float.Parse(rendParsedData[l][ll - 1]);
                    float secondVal = float.Parse(rendParsedData[l][ll + fillPointCount]);
                    float diff = secondVal - firstVal;
                    //float interval = (secondVal - firstVal) / (float)(fillPointCount + 1);
                    float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
                    
                    for (int u = 0; u < fillPointCount; u++)
                    {
                        float theta = sinRadAng * (u + 1);
                        float curveMultiplyer = Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;
                        rendParsedData[l][ll + u] = (firstVal + diff * curveMultiplyer).ToString();
                    }

                    ll += fillPointCount;
                }
            }
            
        }


        //populate empty columns between full rows
        for (int k = 0; k < rendParsedData.Length; k++)
        {
            if (rendParsedData[k][0] != null) { continue; }

            for (int kk = 0; kk < fillPointCount; kk++)
            {
                for (int jjj = 0; jjj < rendParsedData[k].Length; jjj++)
                {
                    float firstVal = float.Parse(rendParsedData[k - 1][jjj]);
                    float secondVal = float.Parse(rendParsedData[k + fillPointCount][jjj]);
                    float diff = secondVal - firstVal;
                    //float interval = (secondVal - firstVal) / (float)(fillPointCount + 1);
                    float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
                    float theta = sinRadAng * (kk + 1);
                    float curveMultiplyer = Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;

                    rendParsedData[k + kk][jjj] = (firstVal + diff * curveMultiplyer).ToString();
                }
            }

            k += fillPointCount;

        }




        rendHeight = rendParsedData.Length;
        rendWidth = rendParsedData[0].Length;

        terrainData = new Vector3[rendWidth * rendHeight];

        float multiplyer = 1f / (fillPointCount + 1);
        //convert depth string vals and location to Vector3 val
        for (int i = 0; i < rendParsedData.Length; i++)
        {
            
            for (int k = 0; k < rendParsedData[i].Length; k++)
            {
                float heightVal = (rendHeight - i - 1);
                float depthVal = float.Parse(rendParsedData[i][k]);

                terrainData[i * rendWidth + k] = new Vector3(k * multiplyer, depthVal, heightVal * multiplyer);
            }
        }

        //terrain data indecies are stored from top left, to the right, down

    }


    public void reCalcVerteciesOld()
    {
        for (int i = 0; i < height; i++)
        {

            for (int k = 0; k < width; k++)
            {
                float heightVal = (height - i - 1);
                float depthVal = float.Parse(parsedData[i][k]);

                terrainData[i * width + k] = new Vector3(k, depthVal, heightVal);
            }
        }
    }



    public Vector3[] offSetData(float offset)
    {
        Vector3[] newTerrainData = (Vector3[])terrainData.Clone();

        for (int i = 0; i < terrainData.Length; i++)
        {
            newTerrainData[i] = new Vector3(newTerrainData[i].x, newTerrainData[i].y, newTerrainData[i].z + offset);
        }

        return newTerrainData;
    }


}
