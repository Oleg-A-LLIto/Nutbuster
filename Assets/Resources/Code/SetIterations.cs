using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



public class SetIterations : MonoBehaviour
{
    public int iter = 30;
    void Example()
    {
        Physics.defaultSolverIterations = iter;
    }
}
