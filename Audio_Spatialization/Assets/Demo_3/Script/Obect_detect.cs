using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obect_detect : MonoBehaviour
{
    public GameObject Source;
    private List<GameObject> activeSource;
    public float MaxDistance;
    public float speed;
    public bool HitDetected;
    private Renderer cube;
    public Material materialR;
    public Material materialW;

    Collider m_collider;
    public RaycastHit Hit;
    int Firstdetection = 1;
    string object_name;


    // Start is called before the first frame update
    void Start()
    {
        activeSource = new List<GameObject>();
        m_collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal") * speed;
        float zAxis = Input.GetAxis("Vertical") * speed;
        transform.Translate(new Vector3(xAxis, 0, zAxis));

    }

    void FixedUpdate()
    {
        HitDetected = Physics.BoxCast(m_collider.bounds.center, new Vector3(8, 8, 5), transform.forward, out Hit, transform.rotation, MaxDistance);
        // HitDetected = Physics.SphereCast(m_collider.bounds.center, 10.0f, transform.forward, out Hit, MaxDistance);
        if (HitDetected)
        {
            
            // Debug.Log(Hit.point.x);
            if (Firstdetection == 1)
            {
                Instantiate(Source, new Vector3(Hit.point.x + 476.41f , Hit.point.y, Hit.point.z), Quaternion.identity);
                Firstdetection = 0;
            }
            else if (object_name != Hit.collider.name)
            {
                GameObject source = Instantiate(Source, new Vector3(Hit.point.x + 476.41f , Hit.point.y, Hit.point.z), Quaternion.identity);
                activeSource.Add(source);
            }

            if (activeSource.Count > 4)
            {
                Destroy(activeSource[0]);
                activeSource.RemoveAt(0);
            }
            object_name = Hit.collider.name;

            // Instantiate(Source, new Vector3(Hit.point.x + 476.41f , Hit.point.y, Hit.point.z), Quaternion.identity);
            // Debug.Log("Hit : " + Hit.collider.name);
            cube = Hit.transform.gameObject.GetComponent<Renderer>();
            cube.sharedMaterial = materialR;
        }

        if (!HitDetected)
        {
            cube.sharedMaterial = materialW;
            // location = Hit.point;
            
        }
        // OnDrawGizmos();
    }



    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     if (HitDetected)
    //     {
    //         // Gizmos.DrawRay(transfrom.position, transform.forward* Hit.distance);

    //         Gizmos.DrawWireCube(transform.position + transform.forward * Hit.distance, transform.localScale);

    //     }
    // }
}
