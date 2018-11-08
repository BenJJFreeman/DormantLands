using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public Transform cameraTransform;
    public Transform focus;
    public Vector3 offset;

    public float rotateSpeed;
    public float movementSpeed;
    public float scrollSpeed;


    public float journeyTime = 1.0f;

    void Start()
    {       

        offset = transform.position - focus.position;
    }
    public void UpdateCamera(bool _focus)
    {

        //float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        //float rotate = Input.GetAxis("Rotate");
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed;


        //Vector3 RIGHT = transform.right * horizontal;


        //float dist = Vector3.Distance(transform.position, focus.position);

        //dist -= 5;
        

        ///vertical*= dist * Time.deltaTime;


        Vector3 tempFocusPos = new Vector3(focus.position.x, transform.position.y, focus.position.z);
        Vector3 targetDir = tempFocusPos - transform.position;


        //targetDir.Normalize();


        if (Vector3.Distance(transform.position, tempFocusPos) > 30)
        {
            if (vertical > 0)
            {
               
            }
            else
            {
                vertical = 0;
            }

        }
        else if (Vector3.Distance(transform.position, tempFocusPos) < 5)
        {
            if (vertical > 0)
            {
                vertical = 0;
            }
            else
            {
               
            }

        }



        Vector3 FORWARD = (targetDir) * (vertical * Time.deltaTime);



        Vector3 UP = Vector3.up * -scroll;

        Vector3 TOTAL =  UP + FORWARD;

        transform.position += TOTAL * 100 * Time.deltaTime;


        //transform.Rotate(Vector3.up ,-rotate,Space.World);

        //transform.Translate(new Vector3(rotate,0,0), Space.Self);

        transform.RotateAround(focus.position,Vector3.up, -rotate * 30 * Time.deltaTime);

        if (transform.position.y < 5)
        {
            
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
        }
        else if (transform.position.y > 50)
        {
            transform.position = new Vector3(transform.position.x, 50, transform.position.z);
        }


        //transform.position = focus.position - offset;

        //float fracComplete = (Time.time - startTime) / journeyTime;
        //offset = transform.position - focus.position;
        //transform.position = Vector3.Slerp(transform.position, focus.position + offset, fracComplete);

        if (_focus)
        {
            RotateAwayFromArea();
        }
        else
        {
            RotateTowardsArea();
        }
       // UpdateFocusCamera();
        


    }
    //public void UpdateFocusCamera()
    //{    
    //    transform.position = new Vector3(focus.position.x, transform.position.y, focus.position.z - 50);
    //    transform.LookAt(focus.position);

    //}
    public void RotateAwayFromArea()
    {

        Vector3 targetDir =  transform.position - focus.position;

        float step = (rotateSpeed / 4) * Time.deltaTime;

        cameraTransform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(cameraTransform.forward, targetDir, step, 0));

        //transform.Rotate(Vector3.right);

    }
    public void RotateTowardsArea()
    {

        Vector3 targetDir = focus.position - transform.position;

        float step = (rotateSpeed/4) * Time.deltaTime;

        cameraTransform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(cameraTransform.forward, targetDir, step, 0));

        //transform.Rotate(Vector3.right * 5 * Time.deltaTime);
    }
    public void SetFocusTarget (Transform _focus)
    {

        focus = _focus;

    }
}
