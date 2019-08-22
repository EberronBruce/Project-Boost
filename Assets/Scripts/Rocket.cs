using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
	bool soundPlaying = true;
	float startVolume = 1f;

	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;

	Rigidbody rigidbody;
	AudioSource audioSource;
	// Start is called before the first frame update
	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update() {
		Thrust();
		Rotate();
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
