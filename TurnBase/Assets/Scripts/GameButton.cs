using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButton : MonoBehaviour
{
    [SerializeField] private GameObject restartObjectsParents;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            restartObjectsParents.SetActive(false);
        }
    }

    public void OnRestartButtonClicked()
    {
        bool isSucceedToParse = int.TryParse(InputFields.Instance.DigitInput.text, out int inputDigit);

        if (!isSucceedToParse) return;

        if (inputDigit <= 0) return;
        GameManager.Instance.GameStart(inputDigit).Forget();
    }

    public void OnQuitButtonClicked()
    {
        GameManager.Instance.GameQuit();
    }

    public void OnLeaveButtonClicked()
    {
        GameManager.Instance.LoadConnectScene();
    }
}