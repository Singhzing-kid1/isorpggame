using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // playerMovement vars start
    // public vars
    public CharacterController controller;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravity = 9.81f;

    // private vars
    private Vector3 playerVelocity;
    private Vector3 move;
    private bool isGrounded;
    // playerMovement vars end

    // cameraAngleChange vars start
    // private vars
    private Vector3 angle1 = new Vector3(30, -45, 0);// w=w,a=a,s=s,d=d
    private Vector3 angle2 = new Vector3(30, -135, 0);// w=a,a=s,s=d,d=w
    private Vector3 angle3 = new Vector3(30, 135, 0);// w=s,a=d,s=w,d=a
    private Vector3 angle4 = new Vector3(30, 45, 0);// w=d,a=w,s=a,d=s
    private Vector3 angle4TD = new Vector3(90, 45, 0);// w=d,a=w,s=a,d=s
    private float angle = 1;

    // serialized fields
    [SerializeField]
    private new Transform camera;
    // cameraAngleChange vars end

    // vanishRoof vars start
    // public vars
    public LayerMask layerMask;
    public float cutoutSize = 0.1f;
    public float falloffSize = 0.05f;

    // private vars
    private MeshRenderer roof;

    // serialized fields
    [SerializeField]
    private float transformX = 1.15f;

    [SerializeField]
    private float transformY = 1.5f;

    [SerializeField]
    private float transformZ = 1.25f;

    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private Camera mainCamera;
    // vanishRoof vars end



    void Update()
    {
        cameraAngleChange();

        if (Input.GetKey("escape")){
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        playerMovement();
        vanishRoof();
    }

    private void playerMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0){
            playerVelocity.y = 0f;
        }

        if (angle == 1){
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //angle 1
        }
        else if (angle == 2){
            move = new Vector3(Input.GetAxis("Vertical") * -1, 0, Input.GetAxis("Horizontal")); // angle 2
        }
        else if (angle == 3){
            move = new Vector3(Input.GetAxis("Horizontal") * -1, 0, Input.GetAxis("Vertical") * -1); //angle 3
        }
        else if (angle == 4){
            move = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal") * -1); // angle 4
        }

        controller.Move(move * playerSpeed * Time.deltaTime);

        if (move != Vector3.zero){
            gameObject.transform.forward = move;
        }

        if (Input.GetButtonDown("Jump") && isGrounded){
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void cameraAngleChange()
    {
        if (Input.GetButtonDown("cameraAngle") && angle == 1){
            camera.rotation = Quaternion.Euler(angle2.x, angle2.y, angle2.z);
            angle = 2;;
        }
        else if (Input.GetButtonDown("cameraAngle") && angle == 2){
            camera.rotation = Quaternion.Euler(angle3.x, angle3.y, angle3.z);
            angle = 3;
        }
        else if (Input.GetButtonDown("cameraAngle") && angle == 3){
            camera.rotation = Quaternion.Euler(angle4.x, angle4.y, angle4.z);
            angle = 4;
        }
        else if (Input.GetButtonDown("cameraAngle") && angle == 4){
            camera.rotation = Quaternion.Euler(angle1.x, angle1.y, angle1.z);
            angle = 1;
        }
    }

    private void vanishRoof()
    {

        Vector3 alteredTransform = new Vector3(transform.position.x + transformX, transform.position.y + transformY, transform.position.z + transformZ);

        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(transform.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - alteredTransform;
        RaycastHit[] hitObjs = Physics.RaycastAll(alteredTransform, transform.TransformDirection(Vector3.up), Mathf.Infinity, layerMask);

        if (Physics.Raycast(alteredTransform, transform.TransformDirection(Vector3.up), Mathf.Infinity, layerMask) && angle == 4){
            camera.rotation = Quaternion.Euler(angle4TD.x, angle4TD.y, angle4TD.z);
            Debug.DrawRay(alteredTransform, transform.TransformDirection(Vector3.up) * 1000, Color.green);
        }

        else if (angle == 4) {
            camera.rotation = Quaternion.Euler(angle4.x, angle4.y, angle4.z);
            Debug.DrawRay(alteredTransform, transform.TransformDirection(Vector3.up) * 1000, Color.red);
        }

        for (int i = 0; i < hitObjs.Length; ++ i){
            Material[] materials = hitObjs[i].transform.GetComponent<Renderer>().materials;

            for (int m = 0; m < materials.Length; ++m){
                materials[m].SetVector("_cutoutPos", cutoutPos);
                materials[m].SetFloat("_cutoutSize", cutoutSize);
                materials[m].SetFloat("_falloffSize", falloffSize);
            }
        }


    }
}
