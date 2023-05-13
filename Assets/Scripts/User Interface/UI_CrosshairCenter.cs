using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrosshairCenter : MonoBehaviour
{
    [SerializeField] Transform sightPosition;
    [SerializeField] Camera cam;
    [SerializeField] Image image;
    [SerializeField] float range;

    // Update is called once per frame
    void Update()
    {
        //image.transform.position = cam.WorldToScreenPoint(cam.transform.position - sightPosition.TransformDirection(Vector3.forward)*range);
        //image.transform.position = cam.WorldToScreenPoint(sightPosition.position + sightPosition.forward*range);
        //image.transform.position = cam.WorldToScreenPoint()

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(sightPosition.position, sightPosition.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            image.transform.position = cam.WorldToScreenPoint(hit.point);
        } else {
            image.transform.localPosition = Vector3.zero;
        }
    }
}
