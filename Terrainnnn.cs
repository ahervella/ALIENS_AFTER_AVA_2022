using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class Terrainnnn : MonoBehaviour
{
    const string terrainFolderPath = "Assets/RESOURCES/TERRAINS/";

    public TextAsset terrainFile;

    public List<Vector3> terrainData = new List<Vector3>();

    public int width = 0;
    public int height = 0;

    private void Start()
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
            string[] lineInts = currLine.Split(' ');

            for (int i = 0; i < lineInts.Length; i++)
            {
                int depthVal = int.Parse(lineInts[i]);
                //because in terrain, read from bottom up
                terrainData.Insert(0, (new Vector3(i, depthVal, height)));
            }

            width = lineInts.Length;
            height++;
        }

    }


}
