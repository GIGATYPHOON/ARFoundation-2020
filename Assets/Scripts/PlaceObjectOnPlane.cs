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


    Pose hitPointthing;
    Pose firsthitPoint;

    bool canget = false;

    float skamala;
    float oldskamala;

    float scaletimer = 10f;

    [SerializeField]
    private GameObject objectozoom;

    void Start()
    {



        raycaster = GetComponent<ARRaycastManager>();
        thingamajig.GetComponent<TMP_Text>().text = "varmit";
        oldskamala = 1f;
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
            hitPointthing = hitPoint;

            // Is this the first time we will place an object?
            if (spawnedObject == null) 
            {
                // Instantiate our own prefab
                spawnedObject = Instantiate(prefab, hitPoint.position, hitPoint.rotation);
            }
            else
            {
                // If there is an existing spawnedObject, we simply move its position

                if (scaletimer <= 0f)
                {
                    spawnedObject.transform.SetPositionAndRotation(hitPoint.position, hitPoint.rotation);
                }


                //thingamajig.GetComponent<TMP_Text>().text = (Vector2.Distance(firsthitPoint.position, hitPointthing.position) + " ");

         
            }




        }
        else
        {



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




    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (canget == true)
            {

                firsthitPoint = hitPointthing;
                canget = false;
            }
        }
        else if (Input.touchCount == 0)
        {


            canget = true;

        }

        scaletimer -= 9f * Time.deltaTime;
        if (scaletimer <= 0)
        {
            scaletimer = 0;
        }


        thingamajig.GetComponent<TMP_Text>().text = " " + scaletimer;






        if (Input.touchCount == 1 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Moved)
        {

            spawnedObject.transform.rotation = Quaternion.Euler(spawnedObject.transform.rotation.eulerAngles.x, (Input.GetTouch(0).position.x - Input.GetTouch(0).rawPosition.x) * 2f, 0);

            scaletimer = 10f;
        }
        else if (Input.touchCount == 2 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Moved && Input.GetTouch(1).phase == UnityEngine.TouchPhase.Moved)
        {
            //skamala = oldskamala + ((Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position)) - 500f) / 1000f;
            oldskamala = objectozoom.transform.localScale.x;



            skamala = oldskamala + (((Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position)) - Vector2.Distance(Input.GetTouch(0).rawPosition, Input.GetTouch(1).rawPosition)) / 2000f);

            skamala = Mathf.Clamp(skamala, 1f, 3f);

            objectozoom.transform.localScale = skamala * Vector3.one;
            //spawnedObject.transform.localScale = skamala * Vector3.one;

            scaletimer = 10f;
        }
    }
}
