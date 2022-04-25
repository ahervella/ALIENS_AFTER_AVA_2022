using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Data2D<T>
{
    private int cols;
    public int Cols => cols;

    private int rows;
    public int Rows => rows;

    private T[,] data;
    private Func<int, T> defaultResetVal;
    private Func<int, T> defaultNewVal;
    private Action<T> destructionCall;
    //element, newHorizPosDiff
    private Action<T, int> onColWrap;
    private Action<T, int> onRowShift;

    /// <summary>
    /// Creates a new 2D data structure where the top row is row 0,
    /// and the left column is column 0
    /// </summary>
    /// <param name="cols"></param>
    /// <param name="rows"></param>
    public Data2D(
        int cols, int rows,
        Func<int, T> defaultResetVal,
        Func<int, T> defaultNewVal = null,
        Action<T> destructionCall = null,
        Action<T, int> onColWrap = null,
        Action<T, int> onRowShift = null)
    {
        this.cols = cols;
        this.rows = rows;
        this.data = new T[cols, rows];

        if (defaultResetVal == null)
        {
            Debug.LogError("Data2D needs a reset value!");
        }
        this.defaultResetVal = defaultResetVal;
        
        this.defaultNewVal = defaultNewVal;
        this.destructionCall = destructionCall;
        this.onColWrap = onColWrap;
        this.onRowShift = onRowShift;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                data[c, r] = defaultResetVal(c);
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
            //destroy vanishing rows
            if (destructionCall != null)
            {
                for (int c = 0; c < cols; c++)
                {
                    for (int r = rows - vertAmnt; r < rows; r++)
                    {
                        destructionCall(data[c, r]);
                    }
                }
            }

                    //shifted rows
            for (int c = 0; c < cols; c++)
            {
                for (int r = rows - 1; r >= vertAmnt; r--)
                {
                    data[c, r] = data[c, r - vertAmnt];
                    onRowShift?.Invoke(data[c, r], vertAmnt);
                }
            }

            //reset the new row(s)
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < vertAmnt; r++)
                {
                    data[c, r] = defaultResetVal(c);
                }
            }


            //initialize the new row(s)
            if (defaultNewVal != null)
            {
                for (int r = vertAmnt - 1; r >= 0; r--)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        data[c, r] = defaultNewVal(c);
                    }
                }
            }
            
            return;
        }

        //make positive so easier to work with
        vertAmnt = -vertAmnt;

        //destroy vanishing rows
        if (destructionCall != null)
        {
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < vertAmnt; r++)
                {
                    destructionCall(data[c, r]);
                }
            }
        }
        

        //shifted rows
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows - vertAmnt; r++)
            {
                //destructionCall?.Invoke(data[c, r]);
                data[c, r] = data[c, r + vertAmnt];
                onRowShift?.Invoke(data[c, r], vertAmnt);
            }
        }

        //reset the new row(s)
        for (int c = 0; c < cols; c++)
        {
            for (int r = rows - vertAmnt; r < rows; r++)
            {
                data[c, r] = defaultResetVal(c);
            }
        }


        //initialize the new row(s)
        if (defaultNewVal != null)
        {
            for (int r = rows - vertAmnt; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    data[c, r] = defaultNewVal(c);
                }
            }
        }
    }

    //TODO: mark up and use this if we decide to get fancy
    //with keeping track of how much is visible with a PSO
    //and making terrain generation changes appear sooner
    //if we are later going to deal with terrain that is
    //longer...
    public void ReplaceRows(int rowsFromStart)
    {
        if (rowsFromStart < 1) { return; }

        //destroy replaced row(s)
        if (destructionCall != null)
        {
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rowsFromStart; r++)
                {
                    destructionCall(data[c, r]);
                }
            }
        }

        //reset the replaced row(s)
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rowsFromStart; r++)
            {
                data[c, r] = defaultResetVal(c);
            }
        }


        //initialize the replaced row(s)
        if (defaultNewVal != null)
        {
            for (int r = rowsFromStart - 1; r >= 0; r--)
            {
                for (int c = 0; c < cols; c++)
                {
                    data[c, r] = defaultNewVal(c);
                }
            }
        }
    }

    /// <summary>
    /// Resets the element at the given location using the
    /// destroy and default valie methods
    /// </summary>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    public void ResetElement(int colIndex, int rowIndex)
    {
        destructionCall?.Invoke(data[colIndex, rowIndex]);
        data[colIndex, rowIndex] = defaultNewVal(colIndex);
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

        //since we are wrapping, we need to cache the wrapped data
        T[,] shiftedData;

        if (horzAmnt > 0)
        {
            shiftedData = new T[horzAmnt, rows];

            //cache wrapped col(s)
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < horzAmnt; c++)
                {
                    shiftedData[c, r] = data[cols - horzAmnt + c, r];
                    onColWrap?.Invoke(shiftedData[c, r], horzAmnt - cols);
                }
            }

            //rest of cols
            for (int r = 0; r < rows; r++)
            {
                for (int c = cols - 1; c >= horzAmnt; c--)
                {
                    data[c, r] = data[c - horzAmnt, r];
                    onColWrap?.Invoke(data[c, r], horzAmnt);
                }
            }

            //apply cached wrapped ones
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < horzAmnt; c++)
                {
                    data[c, r] = shiftedData[c, r];
                }
            }

            return;
        }

        //make positive so easier to work with
        horzAmnt = -horzAmnt;

        shiftedData = new T[horzAmnt, rows];

        //cache wrapped col(s)
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < horzAmnt; c++)
            {
                shiftedData[c, r] = data[c, r];
                onColWrap?.Invoke(shiftedData[c, r], cols - horzAmnt);
            }
                
        }

        //rest of cols
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols - horzAmnt; c++)
            {
                data[c, r] = data[c + horzAmnt, r];
                onColWrap?.Invoke(data[c, r], -horzAmnt);
            }
        }

        //apply cached wrapped ones
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < horzAmnt; c++)
            {
                data[cols - horzAmnt + c, r] = shiftedData[c, r];
            }
        }
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
}
