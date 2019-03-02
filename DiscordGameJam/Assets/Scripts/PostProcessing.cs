using System;
using UnityEngine;

public class PostProcessing : MonoBehaviour {

	[SerializeField]
	Material material;

	public void SetRGB(GlassesManager.Glasses glasses)
	{
		switch (glasses)
		{
			case GlassesManager.Glasses.Red:
				material.SetFloat("_RedFilter", 1);
				material.SetFloat("_GreenFilter", 0);
				material.SetFloat("_BlueFilter", 0);
				break;
			case GlassesManager.Glasses.Green:
				material.SetFloat("_RedFilter", 0);
				material.SetFloat("_GreenFilter", 1);
				material.SetFloat("_BlueFilter", 0);
				break;
			case GlassesManager.Glasses.Blue:
				material.SetFloat("_RedFilter", 0);
				material.SetFloat("_GreenFilter", 0);
				material.SetFloat("_BlueFilter", 1);
				break;
		}
	}
	
	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		Graphics.Blit(source, destination, material);
	}
}
