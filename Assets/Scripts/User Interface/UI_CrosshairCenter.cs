using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrosshairCenter : MonoBehaviour
{
    [SerializeField] Transform sightPosition;
    [SerializeField] Camera cam;
    [SerializeField] Image image;

    // Update is called once per frame
    void Update()
    {
        image.transform.position = cam.WorldToScreenPoint(cam.transform.position - sightPosition.TransformDirection(Vector3.forward));
    }
}
