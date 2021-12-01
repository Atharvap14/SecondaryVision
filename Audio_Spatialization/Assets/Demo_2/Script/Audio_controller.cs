using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_controller : MonoBehaviour
{
    public AudioSource[] audio;
    float Audio_amount;
    float wait = 0.2f;
    int number = 0;
    public float threshold = 2f;
    float time_for_play;
    float ttime;
    // Start is calle1d before the first frame update
    void Start()
    {
        Audio_amount = audio.Length;
        time_for_play = threshold/Audio_amount;
        number = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ttime);
        // if (ttime > threshold && number)
        ttime += Time.deltaTime;
        if (number == 0){
            audio[number].Play();
            number += 1;
            // isplayed = false;
            ttime = 0;
        }

        else if (ttime > (time_for_play+wait) && number < Audio_amount){
            audio[number].Play();
            number += 1;
            ttime = 0;
        }
        
        else if (ttime > time_for_play && number < Audio_amount){
            audio[number-1].Stop();
            // audio[number].Play();
            // number += 1;
            // isplayed = true;
            // ttime = 0;
        }

        

        else if (ttime > time_for_play && number == Audio_amount){
            audio[number-1].Stop();
            number = 0;
        }
    }
}
