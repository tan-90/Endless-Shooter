using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

    public Rigidbody ShellRigidBody;
    public float ForceMinimum;
    public float ForceMaximum;

    float Lifetime = 4;
    float FadeTime = 2;

    // Use this for initialization
    void Start()
    {
        float Force = Random.Range(ForceMinimum, ForceMinimum);
        ShellRigidBody.AddForce(transform.right * Force);
        ShellRigidBody.AddTorque(Random.insideUnitSphere * Force);
        StartCoroutine(Fade());

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(Lifetime);
        float Percentage = 0;
        float FadeSpeed = 1 / FadeTime;

        Material ShellMaterial = GetComponent<Renderer>().material;
        Color InitialColor = ShellMaterial.color;
        
        while(Percentage < 1)
        {
            Percentage += Time.deltaTime * FadeSpeed;
            ShellMaterial.color = Color.Lerp(InitialColor, Color.clear, Percentage);
            yield return null;
        }

        Destroy(gameObject);
    }
}
