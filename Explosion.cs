using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public int decrement;

	// Use this for initialization
	void Start () {
		decrement = 50;
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<ParticleSystem> ().maxParticles -= decrement;
		if (this.GetComponent<ParticleSystem> ().maxParticles == 0) {
			Destroy (this.gameObject);
		}
	}
}
