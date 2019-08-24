using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
	bool soundPlaying = true;
	float startVolume = 1f;

	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;

	enum State {  Alive, Dying, Transcending }
	State state = State.Alive;

	Rigidbody rigidbody;
	AudioSource audioSource;
	// Start is called before the first frame update
	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame 
	void Update() {
		// todo somewhere stop sound on death
		if(state == State.Alive) {
			Thrust();
			Rotate();
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if(state != State.Alive) { return; } //ignore collisions

		switch (collision.gameObject.tag) {
			case "Friendly":
				//do nothing
				break;
			case "Finish":
				state = State.Transcending;
				Invoke("LoadNextLevel", 1f); // parameterise time
				break;
			default:
				state = State.Dying;
				Invoke("LoadFirstLevel", 1f); // paraeterise time
				break;
		}
	}

	private void LoadFirstLevel() {
		SceneManager.LoadScene(0);
	}

	private void LoadNextLevel() {
		SceneManager.LoadScene(1); //todo allow for more than 2 levels
	}

	private void Thrust() {
		
		if(Input.GetKey(KeyCode.Space)) { // can thrust while rotating
			rigidbody.freezeRotation = true; // take manual control of rotation
			rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
			if(!soundPlaying) {
				soundPlaying = true;
				audioSource.volume = startVolume;
				audioSource.Play();
			}
		} else {
			rigidbody.freezeRotation = false;  //resume physics control of rotation
			if(soundPlaying) {
				soundPlaying = false;
				StartCoroutine(VolumeFade(audioSource, 0f, 0.5f));
			}
		}
	}

	private void Rotate() {
		rigidbody.freezeRotation = true; // take manual control of rotation

		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if(Input.GetKey(KeyCode.A)) {
			transform.Rotate(Vector3.forward * rotationThisFrame);
		} else if(Input.GetKey(KeyCode.D)) {
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		rigidbody.freezeRotation = false;  //resume physics control of rotation
	}

	IEnumerator VolumeFade(AudioSource _AudioSource, float _EndVolume, float _FadeLength) {
		float _StartTime = Time.time;
		while(!soundPlaying && Time.time < _StartTime + _FadeLength) {
			float alpha = (_StartTime + _FadeLength - Time.time) / _FadeLength;
			// use the square here so that we fade faster and without popping
			alpha = alpha * alpha;
			_AudioSource.volume = alpha * startVolume + _EndVolume * (1.0f - alpha);

			yield return null;

		}

		if(_EndVolume < 0.1) {
			_AudioSource.UnPause();
		}
	}// end VolumeFade()
}
