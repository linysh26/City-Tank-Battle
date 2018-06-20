using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HP : MonoBehaviour {

	//reference
	public Canvas canvas;
	public GameObject healthPanel;
	private Text[] texts;
	private Slider hpSlider;

	//parameter
	public float offsetX = 0;
	public float offsetY = 1;
	public float offsetZ = 0;
	public int current_hp;
	public int max_hp;

	//renderer
	private Renderer[] selfRenderer;
	private CanvasGroup canvasGroup;

	// Use this for initialization
	void Start () {

		//initialize reference
		texts = new Text[2];
		healthPanel = Instantiate (Resources.Load ("hp")) as GameObject;
		healthPanel.transform.SetParent (Player.Instance.GetComponent<HP>().canvas.transform, false);
		texts = healthPanel.GetComponentsInChildren<Text> ();
		hpSlider = healthPanel.GetComponentInChildren<Slider> ();

		//initialize parameters
		string name = "Player";
		max_hp = 100;
		current_hp = max_hp;
		texts [0].text = name;
		texts [1].text = max_hp + "/" + max_hp;

		//initialize something about renderer
		selfRenderer = this.GetComponentsInChildren<Renderer> ();
		canvasGroup = this.GetComponent<CanvasGroup> ();
	}
	
	// Update is called once per frame
	void Update () {
		hpSlider.value = (float)current_hp / (float)max_hp;
		texts[1].text = current_hp + "/" + max_hp;

		Vector3 worldPos = new Vector3 (transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z + offsetZ);
		Vector3 screenPos = Camera.main.WorldToScreenPoint (worldPos);
		Debug.Log (screenPos);
		healthPanel.transform.position = new Vector3 (screenPos.x, screenPos.y, screenPos.z);
	}

	public void setAlpha(float alpha_new){
		
	}

	public bool getDamage(int damage){
		current_hp -= damage;
		current_hp = current_hp < 0 ? 0 : current_hp;
		if (current_hp == 0) {
			healthPanel.SetActive (false);
		}
		return current_hp > 0;
	}

	public void Restart(){
		current_hp = max_hp;
		healthPanel.SetActive (true);
	}

	public void Fading(){
		
	}

	public void Sort(){
		
	}
}
