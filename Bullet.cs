using UnityEngine;
using System.Collections;

public class Bullet: ScriptableObject{

	public string owner = "";
	public float speed = 3.0f;
    public int ATK;
    public string effect = "";
	public Vector3 direction;
	public GameObject bullet;
	public bool enable;
	// Use this for initialization
	public Bullet(){
		enable = true;
	}

	void Start () {
		
	}

	public void initialize(Transform[] parts, int damage, string effect, float speed){
		bullet.transform.position = new Vector3 (parts[1].position.x, parts[1].position.y, parts[1].position.z) + parts[1].forward * 5.0f;
		bullet.transform.rotation = parts [1].rotation;
        this.ATK = damage;
        this.effect = effect;
		this.speed = speed;
		bullet.SetActive (true);
		bullet.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		bullet.GetComponent<Rigidbody> ().angularDrag = 0;
		bullet.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0, 0, 0);
		bullet.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		bullet.GetComponent<Rigidbody> ().isKinematic = false;
	}

	public void Update(){
		if (bullet.transform.position.z > 20)
			enable = false;
		else if (enable) {
			Debug.Log (Vector3.forward);
			bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.TransformDirection (Vector3.forward) * speed;
		}
	}

	public void setGameObject(GameObject bullet){
		this.bullet = bullet;
		bullet.GetComponent<CollisionController> ().b = this;
	}

	public GameObject getBullet(){
		return this.bullet;
	}

	public void beCollect(){
		this.bullet.transform.position = new Vector3 (0, 0, 0);
		bullet.GetComponent<Rigidbody> ().isKinematic = true;
		bullet.SetActive (false);
		enable = true;
	}
}
