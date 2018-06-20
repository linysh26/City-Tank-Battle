using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {

	public Collision collision;
	public bool flag = true;
	public Bullet b;
	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log (collision.collider.name);
		Vector3 position = collision.transform.position;
		if (collision.gameObject.name [0] == 'A') {
			AITank ai = collision.gameObject.GetComponent<AITank> ();
			ai.HP = 0;
			if (ai.HP <= 0)
				Instantiate (Resources.Load ("Explosion1"), ai.transform.position, Quaternion.identity);
			Player.Instance.recollectBullet ();
		} else if (collision.gameObject.name == "Player") {
			if (!Player.Instance.transform.GetComponent<HP> ().getDamage (b.ATK)) {
				Player.Instance.isRunning = false;
				Player.Instance.HP -= b.ATK;
				if (Player.Instance.HP <= 0) {
					Instantiate (Resources.Load ("Explosion1"), Player.Instance.transform.position, Quaternion.identity);
					TankFactory.Instance.isGameOver = true;
				} 
				TankFactory.Instance.recollectBullet ();
			}
		}
        else
        {
			if(b.owner == "TKF")
				TankFactory.Instance.recollectBullet ();
			else
				Player.Instance.recollectBullet ();
        }
		Instantiate (Resources.Load ("Explosion8"), new Vector3 (position.x, position.y, position.z), Quaternion.identity);
		flag = false;
	}
}
