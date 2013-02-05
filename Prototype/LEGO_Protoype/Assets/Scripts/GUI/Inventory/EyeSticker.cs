using UnityEngine;
using System.Collections;

public class EyeSticker : AbstractSticker
{
		private GameObject plane;
	
		public EyeSticker(GameObject prefab, Texture2D texture, Vector2 stickerSize) : base(prefab, texture, stickerSize)
		{
		
		}
	
		public override void ConcreteInit()
		{
		
		}
	
		public override void Augment(GameObject entity)
		{
				Transform bone = entity.transform.Find("c_Spine01_jnt");
				Transform stickerTransform = this.GetGameObject().transform;
				stickerTransform.parent = bone;
		}
}