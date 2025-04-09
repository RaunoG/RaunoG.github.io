using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform mapContainer; // Kaardi konteiner
    public RectTransform boundary;     // Piirangud (kui vaja)
    public float dragSpeed = 0.5f;    // Liikumise kiirus
    public float smoothTime = 0.3f;   // Sujuvusaja parameeter

    private Vector3 dragOrigin;
    private Vector3 velocity = Vector3.zero;
    private bool isCentering = false; // Kas kaamera keskendub objektile?

    void Start()
    {
        // Kontrolli, et mapContainer algpositsioon on õige
        if (mapContainer != null)
            mapContainer.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (!isCentering) // Kui kaamera ei keskendu, luba kasutajal liigutada
        {
            HandleDrag();
        }
    }

    // Keskendub objektile
    public void CenterOnObject(RectTransform target)
    {
        if (target == null || mapContainer == null) return;

        isCentering = true; // Keela kasutaja liigutamine keskendamise ajal
        Vector3 targetPosition = CalculateTargetPosition(target);
        StartCoroutine(SmoothMove(targetPosition));
    }

    // Arvuta sihtpositsioon
    private Vector3 CalculateTargetPosition(RectTransform target)
    {
        Vector2 targetPos = target.anchoredPosition * mapContainer.localScale.x;
        return new Vector3(-targetPos.x, -targetPos.y, mapContainer.localPosition.z);
    }

    // Sujuv liikumine objekti keskele
    IEnumerator SmoothMove(Vector3 targetPosition)
    {
        while (Vector3.Distance(mapContainer.localPosition, targetPosition) > 1f)
        {
            mapContainer.localPosition = Vector3.SmoothDamp(
                mapContainer.localPosition,
                targetPosition,
                ref velocity,
                smoothTime
            );
            yield return null;
        }
        isCentering = false; // Luba kasutajal uuesti liigutada
    }

    // Hiirega lohistamine
    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1)) // Parem hiireklahv
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = (Input.mousePosition - dragOrigin) * dragSpeed * Time.deltaTime;
            mapContainer.Translate(-delta);
            dragOrigin = Input.mousePosition;
        }
    }
}