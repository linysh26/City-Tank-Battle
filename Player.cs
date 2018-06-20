using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Singleton<Player> {

	public bool isRunning = true;

	public int HP = 100;
	public float speed = 20.0f;

	//mark for navigation
	public GameObject mark;

    //status
    public int index = 2;               //坦克的状态

    //particle
    public GameObject steam;        //状态正常
    public GameObject smoke;        //破损
    public GameObject fire;         //无法行动

    //bullet factory
	List<Bullet> clip_free = new List<Bullet>();
	List<Bullet> clip_used = new List<Bullet>();

    //fire
    public float bulletHealTime = 1.0f;       //冷却时间
    public float lastFireTime = 0;      //上次发射时间
    public int bulletATK = 20;          //普通攻击伤害

    //rockets factory
    Bullet rocket;

    //special fire
    public float rocketHealTime = 2.0f; //火箭攻击冷却时间
    public int rocketsCounter = 5;      //火箭指示物
    public int rocketsATK = 50;         //火箭攻击伤害

	//pibe
	public Transform[] parts;				//坦克部件

    void Start () {
		HP = 100;
		parts = this.GetComponentsInChildren<Transform> ();
		this.GetComponent<Rigidbody> ().isKinematic = false;
		steam = Instantiate (Resources.Load ("Steam"), this.transform) as GameObject;
		steam.transform.position = this.transform.position;
		smoke = Instantiate (Resources.Load ("Smoke"), this.transform) as GameObject;
		smoke.transform.position = this.transform.position;
		fire = Instantiate (Resources.Load ("FireMobile"), this.transform) as GameObject;
		fire.transform.position = this.transform.position;
		mark = Instantiate (Resources.Load ("mark")) as GameObject;
		mark.transform.position = this.transform.position;
		steam.SetActive (true);
		smoke.SetActive (false);
		fire.SetActive (false);
	}
	
	void Update () {
		if (!isRunning) {
			return;
		}
        //移动
		Move ();
        //攻击
        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + bulletHealTime)
        {
            lastFireTime = Time.time;
            clip_used.Add(Attack());
        }
        else if (Input.GetMouseButtonDown(1) && rocketsCounter > 0 && Time.time >= lastFireTime + rocketHealTime)
        {
            lastFireTime = Time.time;
            specialAttck();
        }
        // 管理子弹
        foreach (Bullet bullet in clip_used)
        {
            if (!bullet.enable)
                recollectBullet();
            else
                bullet.Update();
        }
        // 破损
		if (HP < 50 && HP > 10 && index == 2) {
			this.smoke.SetActive (true);
			this.steam.SetActive (false);
			index--;
		} else if (HP <= 10 && index == 1) {
			Instantiate (Resources.Load ("damaged_explosion"), this.transform.position, new Quaternion (0, 0, 90, 0));
			this.fire.SetActive (true);
			index--;
		} 
		else {
			if (steam.GetComponent<ParticleSystem> ().isStopped)
				steam.SetActive (true);
		}
    }
    // 移动
	void Move(){
		float translationY = Input.GetAxis ("Horizontal");
		float translationX = Input.GetAxis ("Vertical");
		Vector3 nextPosition = this.transform.position;
		Vector3 forward = Camera.main.transform.forward;
		Vector3 right = Camera.main.transform.right;
		right.y = 0;
		forward.y = 0;
		NavMeshAgent NMAgent = mark.GetComponent<NavMeshAgent>();
		nextPosition += translationX * forward + translationY * right;
		if (nextPosition != this.transform.position) {
			// 车身朝向
			parts [5].LookAt (new Vector3(nextPosition.x, parts[5].transform.position.y, nextPosition.z));
			// 车身移动
			NMAgent.SetDestination(nextPosition);
			this.transform.position = mark.transform.position;
		}
	}
    // 通常弹
    public Bullet Attack()
    {
        Bullet bullet;
        if (clip_free.ToArray().Length == 0)
        {
            bullet = new Bullet();
			bullet.owner = "Player";
            bullet.setGameObject(Instantiate(Resources.Load("bullet")) as GameObject);
            this.clip_free.Add(bullet);
        }
        bullet = this.clip_free[0];
		bullet.initialize(this.parts, bulletATK, "Explosion4", 10.0f);
        this.clip_free.Remove(bullet);
        return bullet;
    }
    // 火箭炮
	public void specialAttck()
    {
        if(rocket == null)
        {
            rocket = new Bullet();
            rocket.setGameObject(Instantiate(Resources.Load("rocket")) as GameObject);
        }
		rocket.initialize(this.parts, rocketsATK, "Explosion2", 10.0f);
        //移除一个火箭指示物，发动效果：这张卡在回合结束前可以发动一次火球
        rocketsCounter--;
    }
    // 回收通常弹
    public void recollectBullet()
    {
        Bullet bullet = clip_used[0];
        this.clip_used.Remove(bullet);
        this.clip_free.Add(bullet);
        bullet.beCollect();
    }

    public void Restart(){
		isRunning = true;
        index = 2;
		this.transform.position = new Vector3 (0, 0, 0);
		steam.SetActive (true);
		smoke.SetActive (false);
		fire.SetActive (false);
		this.GetComponent<HP> ().Restart ();
	}
}
