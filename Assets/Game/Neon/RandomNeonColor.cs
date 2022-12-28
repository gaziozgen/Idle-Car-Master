using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNeonColor : MonoBehaviour {

	public Projector projector;
	public Shader shader;
	public Texture texture;
	[Header("material details")]
	public Material NeonMat;
	public Color color;

	// Use this for initialization
	void Start () {
		
		projector = gameObject.GetComponent<Projector> ();

		projector.material = new Material (shader);
		NeonMat = projector.material;

		color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));

		NeonMat.SetColor ("_Color", color);

		NeonMat.SetTexture ("_ShadowTex", texture);
		NeonMat.SetTexture ("_FalloffTex", texture);
	}

}
