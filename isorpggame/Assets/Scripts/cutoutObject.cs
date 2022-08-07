using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutoutObject : MonoBehaviour
{
    // public fields
    public float cutoutSize = 0.1f;
    public float falloffSize = 0.05f;
    // serialized fields

    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask roofMask;

    [SerializeField]
    private Camera mainCamera;

    // Update is called once per frame
    void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjs = Physics.RaycastAll(transform.position, offset, offset.magnitude, roofMask);


        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.red);

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
