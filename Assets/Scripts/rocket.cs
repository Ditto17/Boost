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

    bool isTransitioning = false;
    // Start is called before the first frame update
    void Start () {
        rigidBody = GetComponent<Rigidbody> ();
        audioSource = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update () {
        if (!isTransitioning) {
            boost ();
            Rotate ();
        }
        RespondToDebugKeys ();
    }

    private void RespondToDebugKeys () {
        if (Input.GetKeyDown (KeyCode.L)) {
            LoadNextLevel ();
        }
    }
    void OnCollisionEnter (Collision collision) {
        if (isTransitioning) { return; }
        switch (collision.gameObject.tag) {
            case "Friendly":
                //Do Nothing
                break;
            case "Finish":
                StartSuccessSequence ();
                break;
            default:
                StartDeathSequence ();
                break;
        }
    }
    private void StartSuccessSequence () {
        isTransitioning = true;
        audioSource.Stop ();
        audioSource.PlayOneShot (success);
        successParticle.Play ();
        Invoke ("LoadNextLevel", levelLoadDelay);
    }
    private void StartDeathSequence () {
        isTransitioning = true;
        audioSource.Stop ();
        audioSource.PlayOneShot (death);
        deathParticle.Play ();
        Invoke ("LoadCurrentLevel", levelLoadDelay);
    }
    private void LoadNextLevel () {

        int currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene (nextSceneIndex);
    }
    private void LoadCurrentLevel () {
        int currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;
        SceneManager.LoadScene (currentSceneIndex);
    }
    private void boost () {
        if (Input.GetKey (KeyCode.Space)) //can boost while rotating
        {
            ApplyThrust ();
        } else {
            StopApplyingThrust ();
        }
    }
    private void StopApplyingThrust () {
        audioSource.Stop ();
        mainEngineParticle.Stop ();
    }
    private void ApplyThrust () {
        rigidBody.AddRelativeForce (Vector3.up * mainThrust);
        if (!audioSource.isPlaying) //so it doesn't layer
        {
            audioSource.PlayOneShot (mainEngine);
        }
        mainEngineParticle.Play ();
    }
    private void Rotate () {

       if (Input.GetKey (KeyCode.RightArrow)) {
            RotateManually (rcsThrust * Time.deltaTime);

        } else if (Input.GetKey (KeyCode.LeftArrow)) {
            RotateManually (-rcsThrust * Time.deltaTime);
        }
    }
    private void RotateManually (float rotationThisFrame) {
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate (Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics control
    }

}