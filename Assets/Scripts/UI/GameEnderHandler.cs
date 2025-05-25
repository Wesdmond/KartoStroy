using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnderHandler : Singleton<GameEnderHandler>
{
   [SerializeField] public GameObject Win;
   [SerializeField] public GameObject Lose;

   public void Start()
   {
      Lose.SetActive(false);
      Win.SetActive(false);
   }

   public void ShowBad()
   {
      Lose.SetActive(true);
   }

   public void ShowGood()
   {
      Win.SetActive(true);
   }

   public void ExitToMainMenu()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
   }

   public void RestartGame()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }
}