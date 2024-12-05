using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    public Camera MainCamera;
    private Vector3 screenBounds;
    private float objectWidth;
    private float objectHeight;

    // Use this for initialization
    void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        
        screenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        objectWidth = transform.GetComponent<MeshRenderer>().bounds.extents.x; //extents = size of width / 2
        objectHeight = transform.GetComponent<MeshRenderer>().bounds.extents.z; //extents = size of height / 2

        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, (screenBounds.x * -1) + objectWidth, screenBounds.x - objectWidth);
        viewPos.z = Mathf.Clamp(viewPos.z, (screenBounds.z * -1) + objectWidth, screenBounds.z - objectWidth);
        transform.position = viewPos;

        //print(viewPos.x + " || " + transform.position);
    }
}
