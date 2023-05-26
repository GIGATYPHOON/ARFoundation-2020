using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using static UnityEngine.GraphicsBuffer;

// Add this header if you want your component to be added automatically
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectOnPlane : MonoBehaviour
{
    // What object would you like to spawn
    [SerializeField] private GameObject prefab;
    private GameObject spawnedObject;



    [SerializeField]
    private Material alephosnormal;

    [SerializeField]
    private Material redshift;

    [SerializeField]
    private Material dethblues;



    private ARRaycastManager raycaster;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private GameObject thingamajig;


    private Vector2 OldVect2;


    private void Start()
    {
        raycaster = GetComponent<ARRaycastManager>();
        thingamajig.GetComponent<TMP_Text>().text = "varmit";
    }



    public void OnPlaceObject(InputValue value)
    {
        // Get the screen touch position
        Vector2 touchPosition = value.Get<Vector2>();   

        
        // Perform a raycast from the touchPosition into the 3D scene to look for a plane
        if(raycaster.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon) && !IsPointOverUIObject(touchPosition))
        {
            // Get the hit point (pose) on the plane
            Pose hitPoint = hits[0].pose;
            // Is this the first time we will place an object?
            if(spawnedObject == null) 
            {
                // Instantiate our own prefab
                spawnedObject = Instantiate(prefab, hitPoint.position, hitPoint.rotation);
            }
            else
            {
                // If there is an existing spawnedObject, we simply move its position

                var lookPos = hitPoint.position - spawnedObject.transform.position;
                lookPos.y = 0;

                spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
                spawnedObject.transform.Rotate(0, 180f, 0);

                //if (tr)
                //{


                //}
                //else
                //{
                //    //spawnedObject.transform.SetPositionAndRotation(hitPoint.position, hitPoint.rotation);

                //}




            }
        }



    }


    public void IDKifthiswillwork(InputValue value)
    {


    }


    public void RedTextureThing()
    {
        spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = redshift;
    }

    public void BlueTextureThing()
    {
        spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = dethblues;
    }


    public void NormalTextureThing()
    {
        spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = alephosnormal;
    }



    public bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        PointerEventData eventPosition = new PointerEventData(EventSystem.current);
        eventPosition.position = new Vector2(pos.x, pos.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventPosition, results);

        return results.Count > 0;
    }
}
