/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundManager {
	public enum LAYER{
		FRONT,
		MIDDLE,
		BACK
	}
	
	private Dictionary<LAYER, List<GameObject>> backgrounds;
	private Dictionary<LAYER, float> layers;

	public float amount = 5.00f;

	public BackgroundManager(){
		backgrounds = new Dictionary<LAYER, List<GameObject>>();
		layers = new Dictionary<LAYER, float>();

		float offset = 50.0f;

		layers[LAYER.FRONT] = offset + 2.0f;
		layers[LAYER.MIDDLE] = offset + 4.0f;
		layers[LAYER.BACK] = offset + 5.0f;
	}

	public void AddBackground(LAYER layer, GameObject background){
		if(!backgrounds.ContainsKey(layer)){
			backgrounds[layer] = new List<GameObject>();
		}
		GameObject tempBackground = UnityEngine.Object.Instantiate(background) as GameObject;
		backgrounds[layer].Add(tempBackground);
		Debug.Log("added background on " + layer);
		tempBackground.transform.position = new Vector3(0, 0, layers[layer]);
		tempBackground.transform.localScale = new Vector3(30,30,30); 
	}

	public void Clear(){
		foreach(KeyValuePair<LAYER, List<GameObject>> entry in backgrounds){
			foreach(GameObject go in entry.Value){
				UnityEngine.Object.Destroy(go);
			}
			backgrounds.Clear();
		}
	}

	public void MoveAround(){
		foreach(KeyValuePair<LAYER, List<GameObject>> entry in backgrounds){
			if((Mathf.Ceil(Random.value*10) % 2) == 0){
				foreach(GameObject background in entry.Value){
					iTween.PunchPosition(background, new Vector3(10,0,0), 4);
					//iTween.PunchPosition(background, iTween.Hash("amount", new Vector3(10,0,0), "x", 5));
					//background.transform.position = new Vector3(amount + Mathf.Sin(Time.deltaTime) * 5.0f, background.transform.position.y, background.transform.position.z);
					//Debug.Log("Background move");
				}
			}
		}
	}
}
*/