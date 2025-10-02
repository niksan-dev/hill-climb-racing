using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class HeadCollisionDetection : MonoBehaviour
{
    [SerializeField] CarCenteredScreenshot carScreenshot;
    private bool hasCollided = false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return;
        if (collision.gameObject.CompareTag("ground"))
        {
            hasCollided = true;
            StartCoroutine(DelayTillScreenshot());
            Debug.Log("========Head collided with an obstacle!===========");
            // You can add more logic here, like reducing health or playing a sound
        }
    }

    private void OnEnable()
    {
        hasCollided = false;
    }

    IEnumerator DelayTillScreenshot()
    {
        yield return carScreenshot.TakeCarScreenshot();

        Debug.Log($"carScreenshot  :  {carScreenshot._screenshot}");
        yield return new WaitForSeconds(1);
        EventBus.Publish(new GameOverEvent
        {
            screenshot = carScreenshot._screenshot,
            coinsCollected = UnityEngine.Random.Range(500, 1000), // Replace with actual coins collected
            distanceTraveled = UnityEngine.Random.Range(100, 200)
        });
    }
}