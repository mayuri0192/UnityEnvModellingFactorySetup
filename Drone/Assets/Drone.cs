using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Drone : MonoBehaviour
{
    public Rigidbody rBody;

    List<Vector3> opeLoc = new List<Vector3>();
    List<Vector3> targLoc = new List<Vector3>();
    List<Vector3> wallLoc = new List<Vector3>();
    List<Vector3> endLoc = new List<Vector3>();
    List<Vector3> agentLoc = new List<Vector3>();
    List<Vector3> groundLoc = new List<Vector3>();

    private const int numDirection = 360;
    private const int maxDistance = 10;


    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        this.transform.localPosition = new Vector3(6, 5, 2.5f);
    }

    void FixedUpdate()
    {
        RayCastUpdate(out agentLoc, out endLoc, out wallLoc, out opeLoc, out targLoc, out groundLoc);
    }

    public void RayCastUpdate(out List<Vector3> return1, out List<Vector3> return2, out List<Vector3> return3, out List<Vector3> return4, out List<Vector3> return5, out List<Vector3> return6)
    {
        opeLoc = new List<Vector3>();
        targLoc = new List<Vector3>();
        wallLoc = new List<Vector3>();
        endLoc = new List<Vector3>();
        agentLoc = new List<Vector3>();
        groundLoc = new List<Vector3>();
        foreach (var direction in SphereDirections(numDirection))
        {
            Debug.DrawRay(transform.position, direction, Color.blue);


            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
            {
                if (hit.collider.tag == "RollerAgent")
                {

                    agentLoc.Add(hit.point);
                    //UnityEngine.Debug.Log("Agent detected" + agentLoc);
                }
                if (hit.collider.tag == "EndGame")
                {

                    endLoc.Add(hit.point);
                    //UnityEngine.Debug.Log("Ender detected" + endLoc);
                }
                if (hit.collider.tag == "Walls")
                {
                    wallLoc.Add(hit.point); // adding an example Vector3
                    //wall = hit.point;
                    //UnityEngine.Debug.Log("Walls detected" + wall);
                }
                if (hit.collider.tag == "Interactive")
                {

                    opeLoc.Add(hit.point);
                    //UnityEngine.Debug.Log("Operator detected"+ opeLoc);
                }
                if (hit.collider.tag == "Interactive2")
                {
                    targLoc.Add(hit.point);
                    //UnityEngine.Debug.Log("target detected"+ targLoc);
                }
                if (hit.collider.tag == "Ground")
                {
                    groundLoc.Add(hit.point);
                    //UnityEngine.Debug.Log("target detected"+ targLoc);
                }
            }
        }
        return1 = agentLoc;
        return2 = endLoc;
        return3 = wallLoc;
        return4 = opeLoc;
        return5 = targLoc;
        return6 = groundLoc;
    }


    private Vector3[] SphereDirections(int numDirections)
    {
        var pts = new Vector3[numDirections];
        var il = Math.PI * (3 - Math.Sqrt(5));
        var idd = 2f / numDirections;

        foreach (var i in Enumerable.Range(0, numDirections))
        {
            var y = i * idd - 1 + (idd / 2);
            var r = Math.Sqrt(1 - y * y);
            var phi = i * il;
            var x = (float)(Math.Cos(phi) * r);
            var z = (float)(Math.Sin(phi) * r);
            pts[i] = new Vector3(x, y, z);
        }

        return pts;
    }
}


//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using System.Linq;

//public class Drone : MonoBehaviour
//{
//    public Rigidbody rBody;
//    Vector3 opeLoc;
//    Vector3 targLoc;
//    Vector3 wallLoc;
//    Vector3 endLoc;
//    Vector3 agentLoc;

//    Vector3 ope;
//    Vector3 targ;
//    //Vector3 wall;
//    List<Vector3> wall = new List<Vector3>();

//    Vector3 end;
//    Vector3 agent;
//    private const int numDirection = 200;
//    private const int maxDistance = 10;


//    void Start()
//    {
//        rBody = GetComponent<Rigidbody>();
//        this.transform.localPosition = new Vector3(6, 5, 2.5f);
//    }

//    void FixedUpdate()
//    {
//        RayCastUpdate(out agentLoc, out endLoc, out wall, out opeLoc, out targLoc);
//    }

//    public void RayCastUpdate(out Vector3 return1, out Vector3 return2, out List<Vector3> return3, out Vector3 return4, out Vector3 return5)
//    {
//        foreach (var direction in SphereDirections(numDirection))
//        {
//            Debug.DrawRay(transform.position, direction, Color.blue);

//            RaycastHit hit;
//            if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
//            {
//                if (hit.collider.tag == "RollerAgent")
//                {

//                    agent = hit.point;
//                    //UnityEngine.Debug.Log("Agent detected" + agentLoc);
//                }
//                if (hit.collider.tag == "EndGame")
//                {

//                    end = hit.point;
//                    //UnityEngine.Debug.Log("Ender detected" + endLoc);
//                }
//                if (hit.collider.tag == "Walls")
//                {
//                    wall.Add(hit.point); // adding an example Vector3
//                    //wall = hit.point;
//                    //UnityEngine.Debug.Log("Walls detected" + wall);
//                }
//                if (hit.collider.tag == "Interactive")
//                {

//                    ope = hit.point;
//                    //UnityEngine.Debug.Log("Operator detected"+ opeLoc);
//                }
//                if (hit.collider.tag == "Interactive2")
//                {
//                    targ = hit.point;
//                    //UnityEngine.Debug.Log("target detected"+ targLoc);
//                }
//            }
//        }
//        return1 = agent;
//        return2 = end;
//        return3 = wall;
//        UnityEngine.Debug.Log(wall);
//        return4 = ope;
//        return5 = targ;
//    }


//    private Vector3[] SphereDirections(int numDirections)
//    {
//        var pts = new Vector3[numDirections];
//        var il = Math.PI * (3 - Math.Sqrt(5));
//        var idd = 2f / numDirections;

//        foreach (var i in Enumerable.Range(0, numDirections))
//        {
//            var y = i * idd - 1 + (idd / 2);
//            var r = Math.Sqrt(1 - y * y);
//            var phi = i * il;
//            var x = (float)(Math.Cos(phi) * r);
//            var z = (float)(Math.Sin(phi) * r);
//            pts[i] = new Vector3(x, y, z);
//        }

//        return pts;
//    }
//}



