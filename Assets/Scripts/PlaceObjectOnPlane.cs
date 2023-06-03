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
    private GameObject debugtext;


    float zoomscale;

    float scaletimer = 10f;

    float pinchtimer = 10f;

    [SerializeField]
    private GameObject objectozoom;

    void Start()
    {



        raycaster = GetComponent<ARRaycastManager>();
        debugtext.GetComponent<TMP_Text>().text = "Waiting";

    }








    public void OnPlaceObject(InputValue value)
    {
        // Get the screen touch position
        Vector2 touchPosition = value.Get<Vector2>();


        // Perform a raycast from the touchPosition into the 3D scene to look for a plane
        if (raycaster.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon) && !IsPointOverUIObject(touchPosition))
        {
            // Get the hit point (pose) on the plane

            Pose hitPoint = hits[0].pose;

            // Is this the first time we will place an object?
            if (spawnedObject == null) 
            {
                // Instantiate our own prefab
                spawnedObject = Instantiate(prefab, hitPoint.position, hitPoint.rotation);
            }
            else
            {
                // If there is an existing spawnedObject, we simply move its position


                //scaletimer makes it so you don't TP the object as soon as you finish zooming or rotating
                //pinchtimer is the same so you dont TP when you pinch
                if (scaletimer <= 0f && pinchtimer <=0f)
                {
                    spawnedObject.transform.SetPositionAndRotation(hitPoint.position, hitPoint.rotation);
                }


         
            }




        }
        else
        {



        }



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


    //this makes it so pressing the buttons dont TP
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




    private void Update()
    {
        //these handle the timers that disallow teleporting

        scaletimer -= 35f * Time.deltaTime;
        if (scaletimer <= 0)
        {
            scaletimer = 0;
        }


        if(Input.touchCount == 1)
        {
            pinchtimer -= 80f * Time.deltaTime;
            if (pinchtimer <= 0)
            {
                pinchtimer = 0;
            }
        }
        else
        {
            pinchtimer = 10f;
        }




        //this handles the sliding rotation
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Moved)
        {

            spawnedObject.transform.rotation = Quaternion.Euler(spawnedObject.transform.rotation.eulerAngles.x, (Input.GetTouch(0).position.x - Input.GetTouch(0).rawPosition.x) * 2f, 0);

            if(Mathf.Abs((Input.GetTouch(0).position.x - Input.GetTouch(0).rawPosition.x) * 2f) > 15f)
            {
                scaletimer = 10f;
            }

        }

        //this handles the zooming thing
        else if (Input.touchCount == 2 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Moved && Input.GetTouch(1).phase == UnityEngine.TouchPhase.Moved)
        {

    


            //adds the distance difference between current fingers' positions and their positions when they first touched the screen, and adds it to the old scale. screen width  
            zoomscale = objectozoom.transform.localScale.x + (((Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position)) - Vector2.Distance(Input.GetTouch(0).rawPosition, Input.GetTouch(1).rawPosition)) / Screen.height);


            //oops! dont go below 1 or else you will get a magic mystery effect that screws with your head, and zoom 3 is close enough

            zoomscale = Mathf.Clamp(zoomscale, 1f, 3f);

            objectozoom.transform.localScale = zoomscale * Vector3.one;


            scaletimer = 10f;
        }




        debugtext.GetComponent<TMP_Text>().text = " " + scaletimer + " " + pinchtimer;


    }
}
