using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour {

    public GameObject MuzzleFlashHolder;
    public Sprite[] FlashSprites;
    public SpriteRenderer[] SpriteRenderers;

    public float FlashTime;

    void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        MuzzleFlashHolder.SetActive(true);
        int FlashIndex = Random.Range(0, FlashSprites.Length);
        for(int i = 0; i < SpriteRenderers.Length; i++)
        {
            SpriteRenderers[i].sprite = FlashSprites[FlashIndex];
        }
        Invoke("Deactivate", FlashTime);
    }

    void Deactivate()
    {
        MuzzleFlashHolder.SetActive(false);
    }
}
