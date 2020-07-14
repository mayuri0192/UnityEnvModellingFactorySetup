using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject agentObj;
    Vector3 cameraOffSet;

    void Start()
    {
        agentObj = GameObject.Find("RollerAgent");
        cameraOffSet = new Vector3(0, 1, -3);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = agentObj.transform.position + cameraOffSet;
    }
}
