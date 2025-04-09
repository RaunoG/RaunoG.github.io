using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void LoadPA()
    {
        SceneManager.LoadScene("Usa");
        Debug.Log("Käivitab uue tseeni");
    }
    public void LoadLA()
    {
        SceneManager.LoadScene("Fixed La");
        Debug.Log("Käivitab uue tseeni");
    }
    public void LoadEuroopa()
    {
        SceneManager.LoadScene("Euroopa");
    }
    public void LoadAasia()
    {
        SceneManager.LoadScene("Aasia");
    }
    public void LoadAustraalia()
    {
        SceneManager.LoadScene("Austraalia");
    }
    public void LoadAafrika()
    {
        SceneManager.LoadScene("Aafrikano");
    }
}