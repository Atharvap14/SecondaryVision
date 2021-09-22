using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source_size : MonoBehaviour
{
    public float frequency = 10.0f;
    public float cycleSpeed = 1.0f;
    private float total_dist;
    public float width;
    public float height;

    public Vector3 pos;
    private Vector3 prev_pos;
    private Vector3 pre_pos;
    private Vector3 start_pos;
    public Vector3 axis;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        start_pos = transform.position;
        axis = transform.right;

    }

    // Update is called once per frame
    void Update()
    {
        zigzag();
        Debug.Log(total_dist);
    }

    void zigzag(){
        prev_pos = transform.position;
        pos += Vector3.down * Time.deltaTime * cycleSpeed;
        // threshold += Mathf.Abs(pos.y);
        transform.position = pos + axis * Mathf.Cos(Time.time * frequency) * width;
        pre_pos = transform.position;
        total_dist += Mathf.Abs(prev_pos.y - pre_pos.y);
        
        if (total_dist >= height){
            transform.position = start_pos;
            total_dist = 0.0f;
            pos = start_pos;
        }
    }
}
