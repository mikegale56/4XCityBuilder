using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class MapPanZoom : MonoBehaviour
{
    public Tilemap map; //map must have its center at (0,0,0) local position
    public float minSize = 4; //min max size of this orthogonal camer
    public float maxSize = 9;
    public float touchZoomSpeed = 0.01f; //multiplier for touch zoom to otho camera size
    public float zoomFactor = -2.0F;

    [Range(0.0F, 10.0F)]
    public float xPanFactor = 3;
    [Range(0.0F, 10.0F)]
    public float yPanFactor = 3;

    public float inertiaDampingVal = 1.03f; //dividing factor for how quick inertia damp off

    private Camera cam;

    //drag vars
    private Vector3 dragStart;
    private float dragSSx, dragSSy;
    private bool wasDown = false;
    private bool nowDown;
    private Vector3 mapCenter;
    private Vector3 originalWS;

    private float mapHalfWidth, mapHalfHeight, camHalfWidth, camHalfHeight;

    private float minX, maxX, minY, maxY;
    public Vector3 newLocalPos;

    public Vector3 inertiaVector;

    /*void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 150, 100),
            "minX" + minX + "\n" +
            "maxX" + maxX + "\n" +
            "minY" + minY + "\n" +
            "maxY" + maxY + "\n" +
            "newLocalPos" + newLocalPos.ToString()
        );

    }*/

    // Use this for initialization
    void Start()
    {
        inertiaVector = new Vector3(0, 0, 0);
        cam = GetComponent<Camera>();
        newLocalPos = Vector3.zero;
        newLocalPos.z = -10.0F;
    }

    // Update is called once per frame
    void Update()
    {

        processDrag();
        processZoom();

    }

    public void ZoomTo(Vector3 newPos)
    {
        transform.localPosition = newPos;
    }

    private void processDrag()
    {
        //damp inertia
        inertiaVector = inertiaVector / inertiaDampingVal;
        if (inertiaVector.magnitude < 0.005)
        {
            inertiaVector = new Vector3(0, 0, 0);
        }

        //check if input is activated (mouse down or finger touching screen)
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            nowDown = (Input.touchCount > 0);
        }
        else
        {
            nowDown = (Input.GetMouseButton(2)); //|| Input.GetAxis("Mouse ScrollWheel") != 0);
        }

        //calcualte how much camera need to change position
        Vector3 WSDiff = Vector3.zero;

        //if mouse currently holding or finger touching the screen
        if (nowDown)
        {
            //get screenspace input points
            //phone
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 1)
                {
                    //one finger
                    dragSSx = Input.GetTouch(0).position.x;
                    dragSSy = Input.GetTouch(0).position.y;
                }
                else if (Input.touchCount == 2)
                {
                    //check if player is switching between 1 finger to 2 finger
                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Ended)
                    {
                        //if player add or remove finger, recalculate origional World Space point so that the map wouldn't jump around
                        wasDown = false;
                        return;
                    }
                    //two finger
                    dragSSx = (Input.GetTouch(0).position.x + Input.GetTouch(1).position.x) / 2;
                    dragSSy = (Input.GetTouch(0).position.y + Input.GetTouch(1).position.y) / 2;
                }
            }
            else
            {
                dragSSx = Input.mousePosition.x;
                dragSSy = Input.mousePosition.y;
            }

            //calcualte how much camera need to change position
            if (!wasDown)
            {
                originalWS = cam.ScreenToWorldPoint(new Vector3(dragSSx, dragSSy, 1));
            }
            else if (wasDown)
            {
                Vector3 newWS = cam.ScreenToWorldPoint(new Vector3(dragSSx, dragSSy, 1));
                WSDiff = (originalWS - newWS);
                inertiaVector = WSDiff;

            }
        }
        wasDown = nowDown;

        //move camera by WS diff vector or inertiaVector
        if (nowDown)
        {
            newLocalPos = transform.localPosition + WSDiff;
        }
        else
        {
            newLocalPos = transform.localPosition + inertiaVector;
        }

        //make sure camera does not go out of range

        Vector3Int mapSize = map.size;
        mapHalfWidth = mapSize.x / 2;
        mapHalfHeight = mapSize.y / 2;
        mapCenter = map.transform.localPosition;
        mapCenter.x += mapHalfWidth;
        mapCenter.y += mapHalfHeight;

        camHalfHeight = 2f * cam.orthographicSize / 2;
        camHalfWidth = camHalfHeight * cam.aspect;
        minX = mapCenter.x - mapHalfWidth + camHalfWidth; //min position camera should be at
        maxX = mapCenter.x + mapHalfWidth - camHalfWidth;
        minY = mapCenter.y - mapHalfHeight + camHalfHeight;
        maxY = mapCenter.y + mapHalfHeight - camHalfHeight;

        if (minX > maxX)
        { //incase the scale is too small, put camera at center
            newLocalPos = new Vector3(mapCenter.x, mapCenter.y, newLocalPos.z);
        }
        else
        {
            if (newLocalPos.x < minX)
            {
                newLocalPos = new Vector3(minX, newLocalPos.y, newLocalPos.z);
            }
            if (newLocalPos.x > maxX)
            {
                newLocalPos = new Vector3(maxX, newLocalPos.y, newLocalPos.z);
            }
        }
        if (minY > maxY)
        { //incase the scale is too small, put camera at center
            newLocalPos = new Vector3(mapCenter.x, mapCenter.y, newLocalPos.z);
        }
        else
        {
            if (newLocalPos.y < minY)
            {
                newLocalPos = new Vector3(newLocalPos.x, minY, newLocalPos.z);
            }
            if (newLocalPos.y > maxY)
            {
                newLocalPos = new Vector3(newLocalPos.x, maxY, newLocalPos.z);
            }
        }

        newLocalPos.x += Input.GetAxis("Horizontal") * xPanFactor;
        newLocalPos.y += Input.GetAxis("Vertical") * yPanFactor;

        transform.localPosition = newLocalPos;
    }

    private void processZoom()
    {
        //Get increment values
        float increment = 0;
        //float mouseX, mouseY;
        //touch screen
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 2)
            {
                //two finger
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                //scale the zoom increment value base on touchZoomSpeed
                increment = deltaMagnitudeDiff * touchZoomSpeed;
            }
        }
        else
        {
            //mice and keyboard
            increment = Input.GetAxis("Mouse ScrollWheel");
            //mouseX = Input.mousePosition.x;
            //mouseY = Input.mousePosition.y;
        }
        // To do: move the camera center to keep the mouse on the same tile
        //calculate new orthographic camera size
        float currentSize = cam.orthographicSize;
        currentSize += zoomFactor * increment;

        //check if size is out of the defined bound
        if (currentSize >= maxSize)
        {
            currentSize = maxSize;
        }
        else if (currentSize <= minSize)
        {
            currentSize = minSize;
        }

        //set new size
        cam.orthographicSize = currentSize;
    }
}
