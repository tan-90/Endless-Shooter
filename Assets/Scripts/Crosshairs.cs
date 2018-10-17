using UnityEngine;
using System.Collections;

public class Crosshairs : MonoBehaviour {

    public LayerMask TargetMask;
    public SpriteRenderer Dot;
    public Color DotSelectionColor;
    Color DotColor;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        DotColor = Dot.color;
	}


	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
	}

    public void DetectTargets(Ray RayCast)
    {
        if(Physics.Raycast(RayCast, 100, TargetMask))
        {
            Dot.color = DotSelectionColor;
        }
        else
        {
            Dot.color = DotColor;
        }
    }
}
