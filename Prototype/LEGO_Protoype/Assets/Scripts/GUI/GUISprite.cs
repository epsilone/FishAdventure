using System;
using UnityEngine;

public class GUISprite{
	public Texture2D texture;
	public Rect rect;
	public Color colorOn;
	public Color colorOff;

	private Color color;

	const int BORDER = 50;

	public GUISprite()
	{
		rect = new Rect(BORDER, BORDER, Screen.width - BORDER*2, Screen.height - BORDER*2);
		texture = new Texture2D((int)rect.width, (int)rect.height); 
		setColor(new Color(0f,0f,0f,1f));
	}

	public GUISprite(Rect rect, Texture2D texture, Color colorOn, Color colorOff){
		this.rect = rect;
		this.texture = texture;
		this.colorOn = colorOn;
		this.colorOff = colorOff;

		setColor(colorOff); //apply first first time
	}

	public Color getColor(){
		return this.color;
	}

	public void setColor(Color color, bool apply = true){
		texture.SetPixels(getColorArray((int)rect.width*(int)rect.height, color));
		if(apply) 
			texture.Apply();

		this.color = color;
	}

	public void negColor(){
		if(this.color == colorOff){
			setColor(colorOn);
		}else{
			setColor(colorOff);
		}
	}

	private void setAlpha(float alpha){
		GUI.color = color;
	}

	public void Draw(){
		Color tempColor = GUI.color;
		if(isPressed()){
			if(color != colorOn)
				setColor(colorOn);
		}else{
			if(color != colorOff)
				setColor(colorOff);
		}
		GUI.color = color; //new Color(0f,0f,0f,0.0f);
		GUI.DrawTexture (rect, texture, ScaleMode.ScaleToFit);
		GUI.color = tempColor;
	}

	public bool isPressed(){
		Vector3 position;

		if (Input.touchCount > 0 && (Input.GetTouch (0).phase == TouchPhase.Began || Input.GetTouch (0).phase == TouchPhase.Moved))
		{
			//Debug.Log("Touch detected");
			position = Input.GetTouch (0).position;
			position.y = Screen.height - position.y;

			return rect.Contains(position);
		}
		return false;
	}

	public static Color[] getColorArray(int size, Color color){
		Color[] colors = new Color[size];
		for(int i = 0; i < colors.Length; ++i){
			colors[i] = color;
		}
		
		return colors;
	}
}

