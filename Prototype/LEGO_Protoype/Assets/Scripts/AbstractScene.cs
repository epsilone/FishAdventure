/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractScene
{
		protected List<GameEntity> sceneObjects;
		protected Vector3 dimensions;
		protected CameraManager cameraManager;
		protected BackgroundManager backgroundManager;	

		public AbstractScene()
		{
				sceneObjects = new List<GameEntity>();
				cameraManager = new CameraManager(Camera.main);
				backgroundManager = new BackgroundManager();
				//cameraManager.StartScroll(Vector3.up * 10, 5);
				//cameraManager.StartScroll(new Vector3(0,0,-7), 10);
		}
	
		public abstract void Init();
	
		public abstract void DestroyConcrete();

		public abstract void OnGUI();

		public abstract void UpdateConcrete();

		public void Update()
		{
			foreach(GameEntity entity in sceneObjects) {
					entity.Update();
			}
			//cameraManager.Update();
			//backgroundManager.MoveAround();
			UpdateConcrete();
		}

		
		public void Destroy()
		{
				foreach(GameEntity entity in sceneObjects) {
						entity.Destroy();
				}

				sceneObjects.Clear();
				backgroundManager.Clear();
				DestroyConcrete();
		}
}
*/