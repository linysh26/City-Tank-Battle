using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 * 真·坦克工厂
**/
public class TankFactory : Singleton<TankFactory>{

    // tanks
    public List<AITank> tanks;
	public List<AITank> brokens;
	//bullet factory
	List<Bullet> clip_free = new List<Bullet>();		// 所有坦克公用子弹
	List<Bullet> clip_used = new List<Bullet>();

    private static int MAX_SIZE = 10;
    public int size = 0;
	public int HP = 200;

	// factory status
	public string status = "ready";

    // game
    public bool isGameOver = false;

	// Use this for initialization
	void Start () {
		Player player = Player.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (isGameOver)
			return;
		foreach (AITank tank in tanks) {
			if (tank.index != 0)
				tank.Update ();
			else
				recollect (tank);
		}
		if (status == "ready" && size < MAX_SIZE) {
			StartCoroutine (Produce ());
		}
		// 管理子弹
		foreach (Bullet bullet in clip_used)
		{
			if (!bullet.enable)
				recollectBullet();
			else
				bullet.Update();
		}
	}

	void OnGUI(){
		if (!Player.instance.isRunning) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			if (GUI.Button (new Rect (Screen.width / 2 - 100, 40, 200, 20), "Would you want to revive?")) {
				Restart ();
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}
	IEnumerator Produce(){
		status = "producing";
		yield return new WaitForSeconds (4.0f);
		if (brokens.Count == 0) {
			Vector3 position = transform.position;
			GameObject ai = Instantiate (Resources.Load ("AI"), new Vector3(position.x + 8.0f, 2.0f, position.z - 5.0f), Quaternion.identity) as GameObject;
			tanks.Add (ai.GetComponent<AITank>());
		} else {
			AITank ai = brokens [0];
			ai.Restart ();
			brokens.Remove (ai);
			tanks.Add (ai);
		}
		status = "ready";
	}

	public void ProduceBullet(Transform[] parts, int bulletATK){
		Bullet bullet;
		if (clip_free.ToArray().Length == 0)
		{
			bullet = new Bullet();
			bullet.owner = "TKF";
			bullet.setGameObject(Instantiate(Resources.Load("bullet")) as GameObject);
			this.clip_free.Add(bullet);
		}
		bullet = this.clip_free[0];
		bullet.initialize(parts, bulletATK, "Explosion4", 1.0f);
		this.clip_free.Remove(bullet);
		clip_used.Add (bullet);
	}

	// 回收通常弹
	public void recollectBullet()
	{
		Bullet bullet = clip_used[0];
		this.clip_used.Remove(bullet);
		this.clip_free.Add(bullet);
		bullet.beCollect();
	}

	public void recollect(AITank tank){
		tanks.Remove (tank);
		brokens.Add (tank);
	}

	public void Restart(){
		Player.instance.Restart ();
	}
}

