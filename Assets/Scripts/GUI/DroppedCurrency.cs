using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCurrency : MonoBehaviour
{
    public float waitTime = 0f;
    public float fadeTime = 6.0f;
    private PremiumManager premiumManager;
    private int score;
    public SpriteRenderer spriteRenderer;


    void Start()
    {
        premiumManager = GameObject.Find("PremiumManager").GetComponent<PremiumManager>();
        // spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        StartCoroutine(FadeAway());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CollectCoin();
            CurrencyPool.Instance.ReturnItem(gameObject);
        }
    }

    public void CollectCoin()
    {
        premiumManager.UpdatePremium(score);
    }

    public void SetScore(int value)
    {
        score = value;
        Vector3 scale = new Vector3(1f, 1f, 1f) * Mathf.Log(score, 100) + new Vector3(0.5f,0.5f,0.5f);
        this.transform.localScale = scale;
    }

    IEnumerator FadeAway()
    {
        float elapsedTime = 0.0f;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);

        yield return new WaitForSeconds(waitTime);

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }

        CurrencyPool.Instance.ReturnItem(gameObject);
    }
}
