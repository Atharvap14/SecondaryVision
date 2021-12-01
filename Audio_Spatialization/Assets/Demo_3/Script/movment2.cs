using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movment2 : MonoBehaviour
{
    public float speed = -2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
   
        gameObject.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - (speed*Time.deltaTime));
    }
}
