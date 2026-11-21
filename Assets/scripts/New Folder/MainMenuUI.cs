using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnLoadClicked()
    {
        // Kayıt sistemi eklendiğinde burada yükleme yapılacak
        Debug.Log("Kayıtlı oyun yükleniyor... (Yakında)");
    }

    public void OnSettingsClicked()
    {
        // Ayarlar paneli eklendiğinde burada açılacak
        Debug.Log("Ayarlar açılıyor... (Yakında)");
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
