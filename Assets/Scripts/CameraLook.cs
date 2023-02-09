using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{

    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    [SerializeField] private Transform playerReference;

    private float xRotation, yRotation;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
       float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX * 100;
       float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY * 100;

       yRotation += mouseX;
       xRotation -= mouseY;

       xRotation = Mathf.Clamp(xRotation, -90f, 90f);

       transform.rotation = Quaternion.Euler(xRotation, yRotation, 0 );
       playerReference.rotation = Quaternion.Euler(0, yRotation, 0);
    
    }
}
