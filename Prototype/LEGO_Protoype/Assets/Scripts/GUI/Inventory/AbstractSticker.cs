using UnityEngine;
using System.Collections;

public abstract class AbstractSticker
{
		private Texture2D stickerTexture;
		private GameObject stickerPrefab;
		private GameObject stickerInstance;
		private AnimatedTexture animation;
		protected Vector2 size;
	
		public AbstractSticker(GameObject prefab, Texture2D stickerTexture, Vector2 size)
		{
				this.stickerPrefab = prefab;
				this.stickerTexture = stickerTexture;
				this.size = size;
		}
	
		public abstract void ConcreteInit();

		public abstract void Augment(GameObject entity);
	
		public Vector3 position {
				get {
						return stickerInstance.transform.position;
				}
				set {
						stickerInstance.transform.position = value;
				}
		}
	
		public void Instantiate()
		{
				stickerInstance = UnityEngine.Object.Instantiate(stickerPrefab) as GameObject;
				ConcreteInit();
		}
	
		public GameObject GetGameObject()
		{
				return stickerInstance;
		}
	
		public bool IsInstantiated()
		{
				return stickerInstance != null;
		}
	
		public Texture2D GetTexture()
		{
				return stickerTexture;
		}
	
		public Bounds GetBounds()
		{
			return stickerInstance.GetComponent<MeshRenderer>().bounds;
		}
	
		public Vector2 GetSize()
		{
				return size;
		}
	
		public void Destroy()
		{
				UnityEngine.Object.Destroy(stickerInstance);
				//Object.Destroy(stickerInstance);
				Debug.Log("AbstractSticker : Destroyed()");
		}
}
