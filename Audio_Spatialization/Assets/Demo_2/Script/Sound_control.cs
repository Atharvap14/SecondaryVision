using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_control : MonoBehaviour
{
    AudioSource audio;
    float threshold = 0.5f;
    bool isplayed = false;
    float ttime;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();

        audio.Play();
        isplayed = true;
    }

    // Update is called once per frame
    void Update()
    {
        ttime += Time.deltaTime;
        if (ttime > threshold && isplayed){
            audio.Pause();
            isplayed = false;
            ttime = 0;
        }
        else if (ttime > threshold && !isplayed){
            audio.Play();
            isplayed = true;
            ttime = 0;
        }
        // Debug.Log(ttime);
    }
}
