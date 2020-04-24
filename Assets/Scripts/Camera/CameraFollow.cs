using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 followPOS;

    [Header("Configuration")]
    public float cameraMoveSpeed = 120f;
    public GameObject cameraFollowObj;
    public float clampAngle = 80f;
    public float inputSensitivity = 150f;

    [Header("Object references")]
    public GameObject cameraObj;
    public GameObject playerObj;

    [Header("Axis values")]
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;
    public float mouseX, mouseY;
    public float finalInputX, finalInputZ;
    public float smoothX, smoothY;
    public float rotX = 0f, rotY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize rotation values
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1)) {

            CursorConfiguration(false);
            Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, 0f); //Evitamos con algun efecto SHAKE de la camara descalibre su rotacion

            //NOTE: Configuracion previa en el editor desde InputManager
            float inputX = Input.GetAxis("RightStickHorizontal");
            float inputZ = Input.GetAxis("RightStickVertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            finalInputX = inputX + mouseX;
            finalInputZ = inputZ + mouseY;

            rotX += finalInputZ * inputSensitivity * Time.deltaTime;
            rotY += finalInputX * inputSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0f);

            transform.rotation = localRotation;

        } else {
            CursorConfiguration(true);
        }
    }

    private void LateUpdate() {
        CameraUpdater();
    }

    //Cursor configuration
    private void CursorConfiguration(bool freeMovement) {
        Cursor.lockState = freeMovement == true ? Cursor.lockState = CursorLockMode.None : Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = freeMovement;
    }
    
    private void CameraUpdater() {
        Transform target = cameraFollowObj.transform; //Obtenemos el target;

        //Movemos camara hacia el target
        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
