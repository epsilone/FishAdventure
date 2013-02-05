/*
using UnityEngine;
using System.Collections;

public class DragRigid : MonoBehaviour
{	
}
*/	
/*
	public void SetSticker(AbstractSticker theSticker, bool instance)
		{
				Debug.Log("SetSticker");
				this.dragSticker = theSticker;

				if(!theSticker.IsInstantiated() && instance) {
						dragSticker.Instantiate();
				}
		}
*/
/*
		public void RefreshStencil(){
			if(stencilTexture == null){
				stencilTexture = new Texture2D(128, 128);
			}

			SetStencilTexture(stencilTexture, brush, new Vector2(0,0), 50); 		
		}

		public void SetStencilTexture(Texture2D stencil, Texture2D brush, Vector2 brushPosition, int brushSizePixels){
			int width = stencil.width;
			int height = stencil.height;
			RenderTexture rt = RenderTexture.GetTemporary(256, 256, 0, RenderTextureFormat.ARGB32);
			
			//Copy existing stencil to render texture (blit sets the active RenderTexture)
			Graphics.Blit(stencil, rt);
			
			//Apply brush
			RenderTexture.active = rt;
			float bs2 = brushSizePixels / 2f;
			Graphics.DrawTexture(new Rect(brushPosition.x - bs2, brushPosition.y - bs2, brushSizePixels, brushSizePixels), brush);
			
			//Read texture back to stencil
			stencil.ReadPixels(new Rect(0, 0, width, height), 0, 0, true);
			stencil.Apply();
			
			RenderTexture.active = null;
			rt.Release();
		}
*/

/* //Update shader variables 
		if(fish.EntityGameObject != null){
			Component[] childrenRenderer = fish.EntityGameObject.GetComponentsInChildren(typeof(Renderer));
			if(childrenRenderer.Length > 0){
				foreach(Renderer ren in childrenRenderer){
				ren.material.SetVector("_ObjPos", new Vector4( hit2.point.x, hit2.point.y, hit2.point.z, 0));
				ren.material.SetTexture("_Stencil", stencilTexture);
				//ren.material.SetFloat("_Radius", 1);
				//ren.material.SetFloat("_Cutoff", 0.99f);
				//Debug.Log("["+ren.transform.parent + "] _ObjPos = " + new Vector4( hit2.point.x, hit2.point.y, hit2.point.z, 0));
				}
			}
		}
*/