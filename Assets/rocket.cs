﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    Rigidbody rigidBody;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start () {
        rigidBody = GetComponent<Rigidbody> ();
        audioSource = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update () {
        boost ();
        Rotate ();
    }
    private void boost () {
        if (Input.GetKey (KeyCode.Space)) //can boost while rotating
        {
            rigidBody.AddRelativeForce (Vector3.up * mainThrust);
            if (!audioSource.isPlaying) //so it doesn't layer
            {
                audioSource.Play ();
            }
        } else {
            audioSource.Stop ();
        }
    }
    private void Rotate () {
        rigidBody.freezeRotation = true; // take manual control of rotation

        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey (KeyCode.A)) {
           
            transform.Rotate (Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey (KeyCode.D)) {
            
            transform.Rotate (-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control
    }

}