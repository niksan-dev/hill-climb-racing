using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICoinFly : MonoBehaviour
{
    public float duration = 0.5f;
    public AnimationCurve moveCurve;
    public AnimationCurve scaleCurve;

    private RectTransform rect;
    private Image img;

    private Vector3 controlPoint; // Bezier curve control point
    private int coinValue;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    public void StartFly(RectTransform target, int value)
    {
        coinValue = value;

        // Randomize Bezier control point to create curve
        Vector3 start = rect.position;
        Vector3 end = target.position;

        // Add random offset upwards + sideways
        float height = Random.Range(2, 4);   // arc height
        float side = Random.Range(-0.5f, 0.5f);      // left/right sway
        controlPoint = start + new Vector3(side, height, 0f);

        StartCoroutine(FlyToTarget(target, coinValue));
    }


    private IEnumerator FlyBezier(Vector3 start, Vector3 control, Vector3 end)
    {
        Color startColor = img.color;
        float t = 0f;

        while (t < duration)
        {

            float height = Random.Range(-2, -4);   // arc height
            float side = Random.Range(-0.5f, 0.5f);      // left/right sway
            controlPoint = start + new Vector3(side, height, 0f);
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            // Quadratic Bezier curve
            Vector3 pos = Mathf.Pow(1 - p, 2) * start +
                          2 * (1 - p) * p * control +
                          Mathf.Pow(p, 2) * end;

            rect.position = pos;

            // Scale with curve
            rect.localScale = Vector3.one * scaleCurve.Evaluate(p);

            // Fade out smoothly
            // img.color = new Color(startColor.r, startColor.g, startColor.b, fadeCurve.Evaluate(p));

            yield return null;
        }

        //CurrencyManager.Instance.Add(coinValue);

        // Return to pool instead of destroying
        CoinPoolManager.Instance.Return(coinValue, gameObject);
    }

    private IEnumerator FlyToTarget(RectTransform target, int value)
    {
        Vector3 startPos = rect.position;

        Color startColor = img.color;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            //Vector3 endPos = target.position;
            // Lerp position with curve
            rect.position = Vector3.Lerp(startPos, target.position, moveCurve.Evaluate(p));

            // Scale down
            // rect.localScale = Vector3.one * scaleCurve.Evaluate(p);

            // Fade out
            img.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0.5f), p);

            yield return null;
        }

        // Add coins here
        //CurrencyManager.Instance.Add(value);

        CoinPoolManager.Instance.Return(coinValue, gameObject);
    }
}
