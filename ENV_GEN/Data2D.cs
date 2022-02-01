using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Data2D<T>
{
    private int cols;
    private int rows;
    private T[,] data;
    private Func<T> defaultNewVal;

    /// <summary>
    /// Creates a new 2D data structure where the top row is row 0,
    /// and the left column is column 0
    /// </summary>
    /// <param name="cols"></param>
    /// <param name="rows"></param>
    public Data2D(int cols, int rows, Func<T> defaultNewVal)
    {
        this.cols = cols;
        this.rows = rows;
        this.data = new T[cols, rows];
        this.defaultNewVal = defaultNewVal;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                SetElement(defaultNewVal(), c, r);
            }
        }
    }

    /// <summary>
    /// Moves all rows up or down
    /// depending on whether the direction is negative(up) or positive(down), respecitively
    /// </summary>
    /// <param name="vertDir"></param>
    public void ShiftRows(int vertDir)
    {
        if (vertDir == 0) { return; }

        T[,] shiftedData = new T[cols, rows];

        //move down. Two for loops instead of other operations for efficiency
        if (vertDir > 0)
        {
            //reset the new row
            for (int c = 0; c < cols; c++)
            {
                shiftedData[c, 0] = defaultNewVal();
            }

            //shift remaining rows
            for (int r = 1; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shiftedData[c, r] = data[c, r - 1];
                }
            }
        }
        else
        {
            for (int c = 0; c < cols; c++)
            {
                shiftedData[c, rows - 1] = defaultNewVal();
            }

            for (int r = 1; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shiftedData[c, r - 1] = data[c, r];
                }
            }
        }

        data = shiftedData;
    }

    /// <summary>
    /// Moves all columns left or right
    /// depending on whether the number is negative(left) or positve(right), respectively
    /// </summary>
    /// <param name="horzDir"></param>
    public void ShiftColsWrapped(int horzDir)
    {
        if (horzDir == 0) { return; }

        T[,] shiftedData = new T[cols, rows];

        if (horzDir > 0)
        {
            for (int r = 0; r < rows; r++)
            {
                shiftedData[0, r] = data[cols - 1, r];
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 1; c < cols; c++)
                {
                    shiftedData[c, r] = data[c - 1, r];
                }
            }
        }
        else
        {
            for (int r = 0; r < rows; r++)
            {
                shiftedData[cols - 1, r] = data[0, r];
            }


            for (int r = 0; r < rows; r++)
            {
                for (int c = 1; c < cols; c++)
                {
                    shiftedData[c - 1, r] = data[c, r];
                }
            }
        }

        data = shiftedData;
    }

    /// <summary>
    /// Set an element in the 2D data structure
    /// </summary>
    /// <param name="element"></param>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    public void SetElement(T element, int colIndex, int rowIndex)
    {
        data[colIndex, rowIndex] = element;
    }

    /// <summary>
    /// Get an element from the 2D data structure
    /// </summary>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public T GetElement(int colIndex, int rowIndex)
    {
        return data[colIndex, rowIndex];
    }

    public void PrintData(string name)
    {
        Debug.LogFormat("Printing Data2D '{0}'", name);

        for (int r = 0; r < rows; r++)
        {
            StringBuilder rowString = new StringBuilder();
            rowString.AppendFormat("Row {0} contains: ", r);

            for (int c = 0; c < cols; c++)
            {
                rowString.AppendFormat("{0}, ", GetElement(c, r).ToString());
            }

            rowString.Append("ROW END");
            Debug.Log(rowString);
        }

        Debug.LogFormat("End of Data2D '{0}'", name);
    }

    /// <summary>
    /// Convert a 2D float data to a 1D Vector3 array
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Vector3[] ConvertToMeshArray(Data2D<float> data2D, Vector2 units)
    {
        Vector3[] meshData = new Vector3[data2D.data.Length];

        int meshIndex = 0;
        int cols = data2D.data.GetUpperBound(0) + 1;
        int rows = data2D.data.GetUpperBound(1) + 1;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                //since Data2Ds have row index 0 at the top, and if we want the bottom row to be at unity
                //location Vector3(x, elevation, 0), we need to get the difference of rows and r

                //NOTE: in unity, selecting the mesh (which is attached to the envTreadmill node) has
                //the selection point in the middle of the mesh, but the actual transformation point of reference
                //will be zero zero if we do the follow.
                meshData[meshIndex++] = new Vector3(c * units.x, data2D.data[c, r], (rows - r - 1) * units.y);
            }
        }

        return meshData;
    }
}
