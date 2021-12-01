using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Sound_Source_Manager : MonoBehaviour
{
    public float frequency = 10.0f;
    public float cycleSpeed = 2.0f;
    private float total_dist;
    public float width;
    public float height;

    public Vector3 pos;
    private Vector3 prev_pos;
    private Vector3 pre_pos;
    private Vector3 start_pos;
    public Vector3 axis;

    public GameObject source;
    public GameObject[] Coordinates;
    GameObject Enter;
    Button Button;
    InputField X_field;
    InputField Y_field;
    InputField Z_field;
    Vector3 SourcePosition;
    Graphic placeholder;
    // Start is called before the first frame update
    void Start()
    {
        Coordinates = GameObject.FindGameObjectsWithTag("inputField");
        Enter = GameObject.FindGameObjectWithTag("Button");
        Button = Enter.GetComponent<Button>();

        X_field = Coordinates[0].GetComponent<InputField>();
        Y_field = Coordinates[1].GetComponent<InputField>();
        Z_field = Coordinates[2].GetComponent<InputField>();
        // Coordinates.text = "8";
        Button.onClick.AddListener(TaskOnClick);

        pos = transform.position;
        // start_pos = transform.position;
        axis = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        // zigzag();
        
    }

    void TaskOnClick() {
        SourcePosition = new Vector3(float.Parse(X_field.text), float.Parse(Y_field.text), float.Parse(Z_field.text));
        Instantiate(source, SourcePosition, Quaternion.identity);
        X_field.placeholder = placeholder;
    }

    // Zig-Zag movement of sound sorce around a rectangular box to perceive the size of object
    void zigzag(){
        prev_pos = transform.position;
        pos += Vector3.down * Time.deltaTime * cycleSpeed;
        // threshold += Mathf.Abs(pos.y);
        transform.position = pos + axis * Mathf.Cos(Time.time * frequency) * width;
        pre_pos = transform.position;
        total_dist += Mathf.Abs(prev_pos.y - pre_pos.y);
        
        if (total_dist >= height){
            // transform.position = start_pos;
            transform.position = SourcePosition;
            total_dist = 0.0f;
            pos = start_pos;
        }
    }
}
