/*
using System;
using UnityEngine;

public class Scene00 : AbstractScene
{
	private Texture2D tex_fish_adventure;
	private string tex_fish_adventure_path;

	private Texture2D tex_build_icon;
	private string tex_build_icon_path;
	
	public Scene00()
	{
		cameraManager = new CameraManager(Camera.main);
		backgroundManager.AddBackground(BackgroundManager.LAYER.BACK, ResourceManager.LoadPrefab("Prefabs/Backgrounds/Plane_Mid"));
		tex_fish_adventure_path = "file://" + Application.dataPath + "/Resources/Content/Textures/texture_fish_adventure.png";
		tex_build_icon_path = "file://" + Application.dataPath + "/Resources/Content/Textures/tex_build_icon_32x32.png";
	}

	public override void Init()
	{
		WWW www = new WWW(tex_fish_adventure_path);
		while(!www.isDone){
			//wait
		}
		tex_fish_adventure = new Texture2D(640, 256);
		www.LoadImageIntoTexture(tex_fish_adventure);

		WWW www2 = new WWW(tex_build_icon_path);
		while(!www2.isDone){
			//wait
		}
		tex_build_icon = new Texture2D(32, 32);
		www2.LoadImageIntoTexture(tex_build_icon);

	}

	public override void DestroyConcrete(){
		cameraManager = null;
	}
	
	public override void OnGUI(){
		if (tex_fish_adventure != null){
			GUI.DrawTexture(new Rect(60,Screen.height /2 - tex_fish_adventure.height/2, tex_fish_adventure.width, tex_fish_adventure.height), tex_fish_adventure);
		}

		if (tex_build_icon != null){
			GUI.DrawTexture(new Rect(Screen.width-tex_build_icon.width - 5,Screen.height - tex_build_icon.height -5 , tex_build_icon.width, tex_build_icon.height), tex_build_icon);
		}
	}
	
	public override void UpdateConcrete(){
		cameraManager.Update();
	}

}*/