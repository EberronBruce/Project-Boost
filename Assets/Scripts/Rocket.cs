using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;
	[SerializeField] float levelLoadDelay = 2f;

	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip success;
	[SerializeField] AudioClip death;

	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] ParticleSystem successParticles;
	[SerializeField] ParticleSystem deathParticles;


	Boolean isTransitioning = false;

	bool collisionDisabled = false;

	Rigidbody rigidbody;
	AudioSource audioSource;
	// Start is called before the first frame update
	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame 
	void Update() {
		if(!isTransitioning) {
			RespondToThrustInput();
			RespondToRotateInput();
		}
		if (Debug.isDebugBuild) {
			RespondToDebugKeys();
		}
		
	}

	private void RespondToDebugKeys() {
		if (Input.GetKeyDown(KeyCode.L)) {
			LoadNextLevel();
		} else if (Input.GetKeyDown(KeyCode.C)) {
			collisionDisabled = !collisionDisabled;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if(isTransitioning || collisionDisabled) { return; } 

		switch (collision.gameObject.tag) {
			case "Friendly":
				//do nothing
				break;
			case "Finish":
				StartSuccessSequence();
				break;
			default:
				StartDeathSequence();
				break;
		}
	}

	private void StartSuccessSequence() {
		rigidbody.freezeRotation = false;
		isTransitioning = true;
		audioSource.Stop();
		audioSource.PlayOneShot(success);
		successParticles.Play();
		Invoke("LoadNextLevel", levelLoadDelay); 
	}

	private void StartDeathSequence() {
		rigidbody.freezeRotation = false;
		isTransitioning = true;
		audioSource.Stop();
		audioSource.PlayOneShot(death);
		deathParticles.Play();
		Invoke("LoadFirstLevel", levelLoadDelay); 
	}

	private void LoadFirstLevel() {
		SceneManager.LoadScene(0);
	}

	private void LoadNextLevel() {
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		int nextSceneIndex = currentSceneIndex + 1;
		if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
			nextSceneIndex = 0; // loop back to start
		}
		SceneManager.LoadScene(nextSceneIndex); //todo allow for more than 2 levels
	}

	private void RespondToThrustInput() { 
		
		if(Input.GetKey(KeyCode.Space)) { // can thrust while rotating	
			ApplyThrust();
		} else {
			StopApplyingThrust();
		}
	}

	private void StopApplyingThrust() {
		rigidbody.freezeRotation = false;  //resume physics control of rotation
		audioSource.Stop();
		mainEngineParticles.Stop();
	}

	private void ApplyThrust() {
		rigidbody.freezeRotation = true; // take manual control of rotation
		rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
		if(!audioSource.isPlaying) {
			audioSource.PlayOneShot(mainEngine);
		}
		mainEngineParticles.Play();
	}

	private void RespondToRotateInput() {
		 // take manual control of rotation

		if(Input.GetKey(KeyCode.A)) {		
			RotateManually(rcsThrust * Time.deltaTime);
		} else if(Input.GetKey(KeyCode.D)) {
			RotateManually(-rcsThrust * Time.deltaTime);		
		}

		  //resume physics control of rotation
	}

	private void RotateManually(float rotationThisFrame) {
		rigidbody.freezeRotation = true;
		transform.Rotate(Vector3.forward * rotationThisFrame);
		rigidbody.freezeRotation = false;
	}

	
}
