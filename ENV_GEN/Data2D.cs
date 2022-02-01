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
    private Func<int, T> defaultNewVal;
    private Action<T> destructionCall;

    /// <summary>
    /// Creates a new 2D data structure where the top row is row 0,
    /// and the left column is column 0
    /// </summary>
    /// <param name="cols"></param>
    /// <param name="rows"></param>
    public Data2D(int cols, int rows, Func<int, T> defaultNewVal, Action<T> destructionCall)
    {
        this.cols = cols;
        this.rows = rows;
        this.data = new T[cols, rows];
        this.defaultNewVal = defaultNewVal;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                SetElement(defaultNewVal(c), c, r);
            }
        }
    }

    /// <summary>
    /// Moves all rows up or down
    /// depending on whether the direction is negative(up) or positive(down), respecitively
    /// </summary>
    /// <param name="vertAmnt"></param>
    public void ShiftRows(int vertAmnt)
    {
        if (vertAmnt == 0 || vertAmnt > rows)
        {
            Debug.LogError("Error: row shift was more than the total amount of rows, or was zero!");
            return;
        }


        //move down. Two for loops instead of other operations for efficiency
        if (vertAmnt > 0)
        {
            //shifted rows
            for (int c = 0; c < cols; c++)
            {
                for (int r = rows - 1; r >= vertAmnt; r--)
                {
                    data[c, r] = data[c, r - vertAmnt];
                }
            }

            //reset the new row(s)
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < vertAmnt; r++)
                {
                    if (destructionCall != null)
                    {
                        destructionCall(data[c, r]);
                    }
                    data[c, r] = defaultNewVal(c);
                }
            }
        }
        else
        {
            //make positive so easier to work with
            vertAmnt = -vertAmnt;

            //shifted rows
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows - vertAmnt; r++)
                {
                    data[c, r] = data[c, r + vertAmnt];
                }
            }

            //reset the new row(s)
            for (int c = 0; c < cols; c++)
            {
                for (int r = rows - vertAmnt; r < rows; r++)
                {
                    if (destructionCall != null)
                    {
                        destructionCall(data[c, r]);
                    }
                    data[c, r] = defaultNewVal(c);
                }
            }
        }
    }

    /// <summary>
    /// Moves all columns left or right
    /// depending on whether the number is negative(left) or positve(right), respectively
    /// </summary>
    /// <param name="horzDir"></param>
    public void ShiftColsWrapped(int horzAmnt)
    {
        if (horzAmnt == 0 || horzAmnt > cols)
        {
            Debug.LogError("Error: col shift was more than the total amount of columns, or was zero!");
            return;
        }

        //since we are wrapping, we need to cache the new data
        T[,] shiftedData = new T[cols, rows];

        if (horzAmnt > 0)
        {
            //wrapped col(s)
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < horzAmnt; c++)
                {
                    shiftedData[c, r] = data[cols - horzAmnt + c, r];
                }
            }

            //rest of cols
            for (int r = 0; r < rows; r++)
            {
                for (int c = horzAmnt; c > cols - 1; c++)
                {
                    shiftedData[c, r] = data[c + horzAmnt, r];
                }
            }
        }
        else
        {
            //make positive so easier to work with
            horzAmnt = -horzAmnt;

            //wrapped col(s)
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < horzAmnt; c++)
                {
                    shiftedData[rows - horzAmnt + c, r] = data[c, r];
                }
                
            }

            //rest of cols
            for (int r = 0; r < rows; r++)
            {
                for (int c = horzAmnt; c < cols - 1; c++)
                {
                    shiftedData[c + horzAmnt, r] = data[c, r];
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
    /// Convert a 2D float dataa to a 1D Vector3 array
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
