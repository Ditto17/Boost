// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death; 
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem successParticle;
    [SerializeField] ParticleSystem deathParticle; 

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    // Start is called before the first frame update
    void Start () {
        rigidBody = GetComponent<Rigidbody> ();
        audioSource = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update () {
        if (state == State.Alive) {
            boost ();
            Rotate ();
        }
    }

    void OnCollisionEnter (Collision collision) {
        if (state != State.Alive) { return; }
        switch (collision.gameObject.tag) {
            case "Friendly":
                //Do Nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();               
                break;
        }
    }
    private void StartSuccessSequence(){
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticle.Play();
        Invoke ("LoadNextLevel", levelLoadDelay);
    }
     private void StartDeathSequence(){
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticle.Play();
        Invoke ("LoadFirstLevel", levelLoadDelay);
    }
    private void LoadNextLevel () {
        SceneManager.LoadScene (1);
    }
    private void LoadFirstLevel () {
        SceneManager.LoadScene (0);
    }
    private void boost () {
        if (Input.GetKey (KeyCode.Space)) //can boost while rotating
        {
            ApplyThrust();
        } else {
            audioSource.Stop ();
            mainEngineParticle.Stop();
        }
    }
    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce (Vector3.up * mainThrust);
            if (!audioSource.isPlaying) //so it doesn't layer
            {
                audioSource.PlayOneShot(mainEngine);
            }
            mainEngineParticle.Play();
    }
    private void Rotate () {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey (KeyCode.A)) {

            transform.Rotate (-Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey (KeyCode.D)) {

            transform.Rotate (Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control
    }

}