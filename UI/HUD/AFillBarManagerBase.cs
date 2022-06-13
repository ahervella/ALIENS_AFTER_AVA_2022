using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AFillBarManagerBase : MonoBehaviour
{
    public abstract IEnumerator TearDownCR(float delay);
}
