using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Game_Master : MonoBehaviour
{
    public RollerAgentScript roller;
    public RollerAgentScript roller2;
    public Transform Target1;
    public Transform Target2;
    public Transform Target3;
    public Transform Target4;
    public int[] Flags = {0, 1, 0, 0};
    public OperatorScript Operator;

    public Color c1 = new Color(1f, 0f, 0f, 1f);
    public Color c2 = new Color(0f, 1f, 0f, 1f);

    public void Start()
    {
        Flags[0] = 0;
        Flags[1] = 1;
        Flags[2] = 0;
        Flags[3] = 0;

        Target1.GetComponent<MeshRenderer>().material.color = c2;
        Target2.GetComponent<MeshRenderer>().material.color = c1;
        Target3.GetComponent<MeshRenderer>().material.color = c2;
        Target4.GetComponent<MeshRenderer>().material.color = c2;

        Operator.transform.localPosition = new Vector3(0, 0.5f, 0);
        Operator.rBody.angularVelocity = Vector3.zero;
        Operator.rBody.velocity = Vector3.zero;
    }
    
    public void Count(int k)
    {
        UnityEngine.Debug.Log(k+1);
        if (k==0)
        {
            Flags[1] = 0;
            Target2.GetComponent<MeshRenderer>().material.color = c2;
        }
        Flags[k] = 1;
        if (Flags[0] == 1 && Flags[1] == 1 && Flags[2] == 1 && Flags[3] == 1)
        {
            EndAll();
        }
    }
    void Update()
    {
                   
    }

    public void EndAll()
    {
        roller.End();
        roller2.End();
    }
}
