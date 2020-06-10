using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float linearThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip win;
    [SerializeField] AudioClip fail;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem failParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionOn = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) { RespondToDebugKeys(); }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) { LoadNextScene(); }
        else if (Input.GetKeyDown(KeyCode.C)) { collisionOn = !collisionOn; }

    }

    void OnCollisionEnter(Collision collision)
    {

        if (state != State.Alive || !collisionOn) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly": //do nothing
                break;
            case "Finish":
                StartWinSequence();
                break;
            default://lose & blow up
                StarFailSequence();
                break;
        }
    }

    private void StarFailSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(fail);
        failParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartWinSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(win);
        winParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private  void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) { nextSceneIndex = 0; }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        rigidBody.angularVelocity = Vector3.zero; //removes rotation from physics

        if (Input.GetKey(KeyCode.A))//left
        {
            transform.Rotate(Vector3.forward* rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))//right
        {
            transform.Rotate(-Vector3.forward*rotationThisFrame);
        }
        
    }

    private void RespondToRotateInput()
    {
        float rotationThisFrame = linearThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))//thrust
        {
            ApplyThrust(rotationThisFrame);
        }
        else
        { 
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(float rotationThisFrame)
    {
        rigidBody.AddRelativeForce(Vector3.up * rotationThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
