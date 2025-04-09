using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class CheckAnswer : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;
    public Button checkButton;
    public Button Tagasi;
    public TMP_Text resultText;
    public TMP_Text questionText;
    public TMP_Text answerPlaceholder;
    public TMP_Text hintText;
    public RectTransform mapContainer;
    public CameraMovement cameraMovement;

    [Header("Settings")]
    public float moveSpeed = 10f;
    public Vector2 mapMovementLimits = new Vector2(500, 500);

    public List<string> answersList = new List<string>
    {
        "Beauforti meri", "Hudsoni laht", "Labradori meri", "Sargasso meri", "Mehhiko laht",
        "Alaska laht", "California laht", "Gröönimaa", "Arktika saarestik", "Victoria saar",
        "Baffinimaa", "Newfoundlandi saar", "Labradori poolsaar", "Florida poolsaar",
        "California poolsaar", "Alaska poolsaar", "Aleuudi saared", "Kordiljeerid",
        "Kaljumäestik", "Suurtasandik", "Apalatšid", "Mehhiko kiltmaa", "Mississippi madalik",
        "Denali mägi", "Suur järvistu", "St. Lawrence jőgi"
    };

    private string currentAnswer;
    private bool canCheckAnswer = true;
    private int wrongAttempts = 0;
    private GameObject currentHighlightedObject;
    private bool isDragging = false;
    private Vector2 dragStartPosition;

    void Start()
    {
        checkButton.onClick.AddListener(OnCheckAnswer);
        InitializeNewQuestion();
    }

    void Update()
    {
        HandleDragInput();
        HandleKeyboardMovement();

        // Kontrollime, kas kasutaja vajutas Enterit
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnCheckAnswer();
        }
    }

    void InitializeNewQuestion()
    {
        wrongAttempts = 0;
        hintText.text = "";
        resultText.text = "";

        if (answersList.Count > 0)
        {
            int randomIndex = Random.Range(0, answersList.Count);
            currentAnswer = answersList[randomIndex];
            answersList.RemoveAt(randomIndex);
            questionText.text = "Sisesta vastus:";
            answerPlaceholder.text = GenerateUnderscores(currentAnswer); // Alguses kõik kriipsud
            FindAndHighlightObject();
        }
        else
        {
            StartCoroutine(LoadMenuAfterDelay(2f));
        }
        Debug.Log(currentAnswer + " on vastuseks ");
    }

    void OnCheckAnswer()
    {
        if (!canCheckAnswer) return;

        string userInput = inputField.text.Trim();
        bool isCorrect = string.Equals(userInput, currentAnswer, System.StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            resultText.text = "Õige vastus!";
            resultText.color = Color.green;
            StartCoroutine(NextQuestionAfterDelay(1.5f));
        }
        else
        {
            wrongAttempts++;
            resultText.text = "Vale vastus!";
            resultText.color = Color.red;

            if (wrongAttempts == 1)
            {
                answerPlaceholder.text = ReplaceFirstUnderscoreWithLetter(currentAnswer, answerPlaceholder.text);
            }
            else if (wrongAttempts > 1)
            {
                hintText.text = currentAnswer;
                StartCoroutine(ClearHintAfterDelay(10f));
            }

            StartCoroutine(ClearResultTextAfterDelay(2f));
        }

        inputField.text = "";
        canCheckAnswer = false; // Keela uued kontrollid hetkeks
        inputField.ActivateInputField(); // Fokuseeri inputField uuesti m
    }
    public void tagasi()
    {
        SceneManager.LoadScene("Menu");
    }

    void HandleDragInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - dragStartPosition;
            dragStartPosition = Input.mousePosition;
            MoveMap(delta * moveSpeed * Time.deltaTime);
        }
    }
  

    void HandleKeyboardMovement()
    {
        Vector2 movement = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) movement.y += 1;
        if (Input.GetKey(KeyCode.DownArrow)) movement.y -= 1;
        if (Input.GetKey(KeyCode.LeftArrow)) movement.x += 1;
        if (Input.GetKey(KeyCode.RightArrow)) movement.x -= 1;

        if (movement != Vector2.zero)
        {
            MoveMap(movement * moveSpeed * Time.deltaTime);
        }
    }

    void MoveMap(Vector2 delta)
    {
        Vector2 newPosition = mapContainer.anchoredPosition + delta;
        newPosition.x = Mathf.Clamp(newPosition.x, -mapMovementLimits.x, mapMovementLimits.x);
        newPosition.y = Mathf.Clamp(newPosition.y, -mapMovementLimits.y, mapMovementLimits.y);
        mapContainer.anchoredPosition = newPosition;
    }

    void FindAndHighlightObject()
    {
        if (currentHighlightedObject != null)
            currentHighlightedObject.SetActive(false);

        currentHighlightedObject = FindInactiveObject(currentAnswer);
        if (currentHighlightedObject != null)
        {
            currentHighlightedObject.SetActive(true);
            cameraMovement.CenterOnObject(currentHighlightedObject.GetComponent<RectTransform>());
        }
        else Debug.LogError($"Objekt '{currentAnswer}' puudub!");
    }

    GameObject FindInactiveObject(string name)
    {
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            if (obj.name == name && !obj.activeInHierarchy)
                return obj;
        return null;
    }

    string GenerateUnderscores(string s)
    {
        return new string(s.Select(c => char.IsLetterOrDigit(c) ? '_' : c).ToArray());
    }

    string ReplaceFirstUnderscoreWithLetter(string correctAnswer, string placeholder)
    {
        char[] placeholderChars = placeholder.ToCharArray();
        for (int i = 0; i < correctAnswer.Length; i++)
        {
            if (placeholderChars[i] == '_')
            {
                placeholderChars[i] = correctAnswer[i]; // Asenda esimene kriips õige tähega
                break;
            }
        }
        return new string(placeholderChars);
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitializeNewQuestion();
        canCheckAnswer = true;
    }

    IEnumerator ClearResultTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultText.text = ""; // Tühjenda "Vale vastus!" teade
        canCheckAnswer = true;
    }

    IEnumerator ClearHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hintText.text = ""; // Tühjenda kogu sõna vihje
    }

    IEnumerator LoadMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }
}