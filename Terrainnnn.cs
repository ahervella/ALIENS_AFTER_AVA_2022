using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class Terrainnnn : MonoBehaviour
{
    const string terrainFolderPath = "Assets/RESOURCES/TERRAINS/";

    public TextAsset terrainFile;

    public float appearanceLikelihood;

    public Vector3[] terrainData;

    //control point heights and widths
    public int width = 0;
    public int height = 0;

    //multiplyer for text file floats (so user can use large numbers
    //for precision in text file)
    public float elevationMultiplyer = 0.05f;

    //actual render widths and heights (after adding fill points)
    public int rendWidth = 0;
    public int rendHeight = 0;

    //the direct pased data from the text file in the readable order
    List<float[]> parsedData = new List<float[]>();

    private void OnEnable()
    {
        parseTerrainFile();
    }


    public void parseTerrainFile()
    {
        if (terrainFile == null) { return; }

        StreamReader sr = new StreamReader(terrainFolderPath + terrainFile.name + ".txt");


        string currLine = "";

        while ((currLine = sr.ReadLine()) != null)
        {
            string[] rowOfStringNums = currLine.Split(' ');
            float[] rowOfNums = new float[rowOfStringNums.Length];

            for (int i = 0; i < rowOfStringNums.Length; i++)
            {
                rowOfNums[i] = float.Parse(rowOfStringNums[i]);
            }

            parsedData.Add(rowOfNums);

        }


        height = parsedData.Count;
        width = parsedData[0].Length;
        terrainData = new Vector3[width * height];

        reCalcVertecies(0);

    }



    public void reCalcVertecies(int fillPointCount)

    {
        int rendPDHeight = parsedData.Count + fillPointCount * (parsedData.Count - 1);
        int rendPDWidth = parsedData[0].Length + fillPointCount * (parsedData[0].Length - 1);

        float[][] rendParsedData = new float[rendPDHeight][];
        for (int blah = 0; blah < rendParsedData.Length; blah++) { rendParsedData[blah] = new float[rendPDWidth]; }

        //build list with empty spots from fillPoint count
        for (int g = 0, rowOffset = 0; g < parsedData.Count; g++)
        {
            rendParsedData[g + rowOffset] = (new float[parsedData[g].Length + (fillPointCount * (parsedData[g].Length -1))]);

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
            //if not one of the original control points (aka still needs a value), next!
            if (l % (fillPointCount+1) != 0) { continue; }


            for (int ll = 0; ll < rendParsedData[l].Length; ll++)
            {
                //basically saying: if first non control point...
                //basically finds the first one
                if (ll % (fillPointCount + 1) != 0)
                {
                    float firstVal = rendParsedData[l][ll - 1];
                    float secondVal = rendParsedData[l][ll + fillPointCount];
                    float diff = secondVal - firstVal;

                    float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
                    
                    for (int u = 0; u < fillPointCount; u++)
                    {
                        float theta = sinRadAng * (u + 1);
                        float curveMultiplyer = Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;
                        rendParsedData[l][ll + u] = firstVal + diff * curveMultiplyer;
                    }

                    //to jump to next section to fill
                    ll += fillPointCount;
                }
            }
            
        }


        //populate empty columns between full rows
        for (int k = 0; k < rendParsedData.Length; k++)
        {
            //if a control point, next! We need to only iterate empty rows
            //basically finds the first one
            if (k % (fillPointCount + 1) == 0) { continue; }

            for (int kk = 0; kk < fillPointCount; kk++)
            {
                for (int jjj = 0; jjj < rendParsedData[k].Length; jjj++)
                {
                    float firstVal = rendParsedData[k - 1][jjj];
                    float secondVal = rendParsedData[k + fillPointCount][jjj];
                    float diff = secondVal - firstVal;

                    float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
                    float theta = sinRadAng * (kk + 1);
                    float curveMultiplyer = Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;

                    rendParsedData[k + kk][jjj] = firstVal + diff * curveMultiplyer;
                }
            }

            //to jump to next section to fill
            k += fillPointCount;

        }




        rendHeight = rendParsedData.Length;
        //each row of the text file should have the same width
        rendWidth = rendParsedData[0].Length;

        terrainData = new Vector3[rendWidth * rendHeight];

        float multiplyer = 1f / (fillPointCount + 1);
        //convert depth string vals and location to Vector3 val
        for (int i = 0; i < rendParsedData.Length; i++)
        {
            
            for (int k = 0; k < rendParsedData[i].Length; k++)
            {
                float heightVal = rendHeight - i - 1;
                float depthVal = rendParsedData[i][k] * elevationMultiplyer;

                terrainData[i * rendWidth + k] = new Vector3(k * multiplyer, depthVal, heightVal * multiplyer);
            }
        }

        //terrain data indecies are stored from left to right, going down

    }




    //move the terrain shape up and down the plane
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
