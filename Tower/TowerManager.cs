using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

	private TowerBtn towerBtnPressed;
	private SpriteRenderer spriteRenderer;
	private TowerBtn towerPrice;
	private List<Tower> TowerList = new List<Tower>();
	private List<Collider2D> BuildList = new List<Collider2D>();
	public Collider2D buildTile;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildTile = GetComponent<Collider2D>();
		spriteRenderer.enabled = false;
	}
	
	void Update () {
		
		if (Input.GetMouseButtonDown(0)){
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
			if (hit.collider.tag == "buildSite") {
				hit.collider.tag = "buildSiteFull";
				buildTile = hit.collider;
				buildTile.tag = "buildSiteFull";
				RegisterBuildSite(buildTile);
				PlaceTower(hit);
				StartCoroutine(BuildSiteReset(hit));
			}
		}
		if (spriteRenderer.enabled){
			FollowMouse();
		}
	}

	IEnumerator BuildSiteReset(RaycastHit2D hit){
		yield return new WaitForSeconds(40);
		hit.collider.tag = "buildSite";
	}

	public void RegisterBuildSite(Collider2D buildTag){
		BuildList.Add(buildTag);
	}

	public void RegisterTower(Tower tower){
		TowerList.Add(tower);
	}

	public void RenameTagsBuildSites(){
		foreach(Collider2D buildTag in BuildList){
			buildTag.tag = "buildSite";
		}
		BuildList.Clear();
	}

	public void DestroyAllTower() {
		foreach(Tower tower in TowerList) {
			Destroy(tower.gameObject);
		}
		TowerList.Clear();
	}

	public void selectedTower(TowerBtn towerSelected){
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney){
			towerBtnPressed = towerSelected;
			EnableDragSprite(towerBtnPressed.DragSprite);
			BuyTower(towerBtnPressed.TowerPrice);
		}
		
	}

	public void PlaceTower(RaycastHit2D hit){
		if(!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null){
			Tower newTower = Instantiate(towerBtnPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			RegisterTower(newTower);
			DisableDragSprite();
			
		}
	}

	public void BuyTower(int price){
		GameManager.Instance.SubtractMoney(price);
	}

	public void FollowMouse(){
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	public void EnableDragSprite(Sprite sprite){
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void DisableDragSprite(){
		spriteRenderer.enabled = false;
	}
}
