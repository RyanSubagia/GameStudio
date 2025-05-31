using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
   public void ChangeScene()
   {
        PlayerPrefs.SetInt("CurrentLevelIndex", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level1");
   }
}
