using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void jogar(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void sair(){
        Application.Quit();
    }

    public void opcoes(){
        SceneManager.LoadScene("opcoesMenu");
    }

    public void voltarMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}
