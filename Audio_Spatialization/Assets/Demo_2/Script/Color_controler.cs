using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_controler : MonoBehaviour
{
    public Material[] materials;
    public Material a;
    public Renderer[] rend;
    float material_amount;
    int number;
    float wait = 0.2f;
    public float threshold = 5f;
    float time_for_play;
    float ttime;
    // Start is called before the first frame update
    void Start()
    {
        // rend.enabled = true;
        material_amount = materials.Length;
        time_for_play = threshold/material_amount;
        number = 0;

    }

    // Update is called once per frame
    void Update()
    {
        ttime += Time.deltaTime;
        if (number == 0){
            rend[number].enabled = true;
            rend[number].sharedMaterial = materials[number];
            number += 1;
            // isplayed = false;
            ttime = 0;
        }

        else if (ttime > (time_for_play+wait) && number < material_amount){
            // audio[number].Play();
            rend[number].sharedMaterial=materials[number];
            number += 1;
            ttime = 0;
        }

        else if (ttime > time_for_play && number < material_amount){
            // rend[number-1].enabled = false;
            rend[number-1].sharedMaterial=a;
            rend[number].enabled = true;
            // rend[number].sharedMaterial=materials[number];
            // number += 1;
            // isplayed = true;
            // ttime = 0;
        }

        else if (ttime > time_for_play && number == material_amount){
            // rend[number].enabled = false;
            rend[number-1].sharedMaterial=a;
            number = 0;
        }
    }
}
