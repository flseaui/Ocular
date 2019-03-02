using System;
using UnityEngine;

public class PostProcessing : MonoBehaviour {

	[SerializeField] private Material _material;

	private void Start()
	{
		SetRedFilter(true);
		SetGreenFilter(true);
		SetBlueFilter(true);
	}
	
	public void SetRedFilter(bool state) => _material.SetFloat("_RedFilter", state ? 1 : 0);
	public void SetBlueFilter(bool state) => _material.SetFloat("_BlueFilter", state ? 1 : 0);
	public void SetGreenFilter(bool state) => _material.SetFloat("_GreenFilter", state ? 1 : 0);

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		Graphics.Blit(source, destination, _material);
	}
}
