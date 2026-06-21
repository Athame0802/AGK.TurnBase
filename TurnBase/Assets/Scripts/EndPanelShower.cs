using TMPro;
using Unity.Netcode;
using UnityEngine;

public class EndPanelShower : MonoBehaviour
{
    [SerializeField] private GameObject EndPanel;
    [SerializeField] private TMP_Text EndTitle;
    [SerializeField] private TMP_Text EndAdditionalText;

    private void Start()
    {
        GameManager.Instance.OnGameEnd += OnGameEnd;
        EndPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameEnd -= OnGameEnd;
    }

    private void OnGameEnd()
    {
        ShowEndPanel();

        if (!CheckVictory())
        {
            EndTitle.text = "패배";
            EndAdditionalText.text = "다음 기회에...";
        }
    }

    private void ShowEndPanel()
    {
        EndPanel.SetActive(true);
    }

    private bool CheckVictory()
    {
        return TurnManager.Instance.turnClientId.Value == NetworkManager.Singleton.LocalClientId;
    }
}
