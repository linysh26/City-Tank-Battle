using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITank : MonoBehaviour
{

    public int HP = 100;

    //status
    public int index = 2;               //坦克的状态
    public string attackStatus = "ready";

    //particle
    public GameObject steam;        //状态正常
    public GameObject smoke;        //破损
    public GameObject fire;         //无法行动

    //fire
    public float bulletHealTime = 5.0f;       //冷却时间
    public float lastFireTime = 0;            //上次发射时间
    public int bulletATK = 20;                //普通攻击伤害

    //target
	public Transform target;

	//pibe
	public Transform[] parts;				//坦克部件

	//ai
	public Vector3 nextPosition;
	public Vector3 aimPosition;

    void Start()
    {
        HP = 100;
		parts = this.GetComponentsInChildren<Transform> ();
		steam = Instantiate (Resources.Load ("Steam"), this.transform) as GameObject;
		steam.transform.position = this.transform.position;
		smoke = Instantiate (Resources.Load ("Smoke"), this.transform) as GameObject;
		smoke.transform.position = this.transform.position;
		fire = Instantiate (Resources.Load ("FireMobile"), this.transform) as GameObject;
		fire.transform.position = this.transform.position;
		steam.SetActive (true);
		smoke.SetActive (false);
		fire.SetActive (false);
		target = Player.Instance.transform;
    }

    public void Update()
    {
		if (HP <= 0) {
			index = 0;
		}
        //会留下残骸
		if (index == 0)
        {
			this.gameObject.SetActive (false);
            return;
        }
		Think ();
        // 破损
		if (HP < 50 && HP > 10 && index == 2)
		{
			this.smoke.SetActive (true);
			this.steam.SetActive (false);
			index--;
		}
		else if (HP <= 10 && index == 1)
		{
			Instantiate(Resources.Load("damaged_explosion"), this.transform.position, new Quaternion(0, 0, 90, 0));
			this.fire.SetActive (true);
			index--;
		}

    }

    // AI program
    void Think()
    {
		NavMeshAgent NMAgent = GetComponent<NavMeshAgent>();
        //1 发现目标
        if (target != null)
        {
			if (attackStatus == "aiming") {
				Aim (aimPosition);
				return;
			}
			if (getDistanceFrom (target.position) > 15) {
				aimPosition = target.position;
				aimPosition.y = 0.7f;
				parts [1].LookAt (aimPosition);
				NMAgent.SetDestination (target.position);
				return;
			}
			if (Time.time >= this.lastFireTime + bulletHealTime) {
				lastFireTime = Time.time;
				attackStatus = "ready";
			}
            // 冷却
            if(attackStatus == "healing")
            {
				int direction = Random.Range (0, 4);
				if (direction == 0) {
					nextPosition = new Vector3 (transform.position.x + 4, transform.position.y, transform.position.z);
				} 
				else if (direction == 1) {
					nextPosition = new Vector3 (transform.position.x - 4, transform.position.y, transform.position.z);
				} 
				else if (direction == 2) {
					nextPosition = new Vector3 (transform.position.x + 2, transform.position.y, transform.position.z + 2);
				} 
				else {
					nextPosition = new Vector3 (transform.position.x - 2, transform.position.y, transform.position.z - 2);
				}
				aimPosition = target.position;
				aimPosition.y = 0.7f;
				parts [1].LookAt (aimPosition);
				NMAgent.SetDestination (nextPosition);
				return;
            }
            // 攻击
            else if(attackStatus == "ready")
            {
				StartCoroutine(Attack());
            }
            else
            {
                return;
            }
        }
    }
	// 瞄准
	public void Aim(Vector3 lookAtPosition)
    {
        // 炮台方向
		parts [1].LookAt (lookAtPosition);
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		Physics.Raycast (transform.position, transform.forward, out hit, 100);
		Debug.DrawLine (this.transform.position, hit.point, Color.red);
    }
    // 通常弹
	IEnumerator Attack()
    {
		attackStatus = "aiming";
		aimPosition = target.position;
		aimPosition.y = 0.7f;
        yield return new WaitForSeconds(1.0f);
        // 发射
		TankFactory.Instance.ProduceBullet(parts, bulletATK);
        attackStatus = "healing";
    }

    public double getDistanceFrom(Vector3 position)
    {
        float div_x = position.x - this.transform.position.x;
        float div_z = position.z - this.transform.position.z;
        return Mathf.Abs(Mathf.Sqrt(div_x * div_x + div_z * div_z));
    }

    public void Restart()
    {
        index = 2;
		HP = 100;
		steam.SetActive (true);
		smoke.SetActive (false);
		fire.SetActive (false);
    }
}
