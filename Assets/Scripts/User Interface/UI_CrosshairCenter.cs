using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(31)]
public class UI_CrosshairCenter : MonoBehaviour
{
    [SerializeField] Transform sightPosition;
    [SerializeField] Camera cam;
    [SerializeField] Image image;
    [SerializeField] float _maxRange = 30;
    [SerializeField] float _minRange = 1;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(sightPosition.position, sightPosition.TransformDirection(Vector3.forward), out hit, _maxRange))
        {
            if(hit.distance > _minRange)
            {
                image.transform.position = cam.WorldToScreenPoint(hit.point);
            }
            else
            {
                image.transform.position = cam.WorldToScreenPoint(sightPosition.position + sightPosition.TransformDirection(Vector3.forward)*_minRange);
            }
        }
        else
        {
            image.transform.localPosition = Vector3.zero;
        }
    }
}
