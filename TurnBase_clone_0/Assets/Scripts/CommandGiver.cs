using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class CommandGiver : MonoBehaviour
{
    [SerializeField] private List<Button> commandButtons;

    private List<ulong> clientIds = new(4);
    private List<ulong> clientIdsWithoutMe = new(4);
    private bool canBeClicked = true;

    private void Start()
    {
        TurnManager.Instance.GetAllClientIds(clientIds);

        for (int i = 0; i < clientIds.Count; i++)
        {
            if (clientIds[i] == NetworkManager.Singleton.LocalClientId) continue;

            clientIdsWithoutMe.Add(clientIds[i]);
        }

        TurnManager.Instance.turnClientId.OnValueChanged += OnTurnEnd;

        OnTurnEnd(1, 1);
        RefreshCanBeClicked();
    }
    
    private void OnTurnEnd(ulong a, ulong b)
    {
        if (TurnManager.Instance.turnClientId.Value == NetworkManager.Singleton.LocalClientId)
        {
            canBeClicked = true;
            RefreshCanBeClicked();
        }
        else
        {
            canBeClicked = false;
            RefreshCanBeClicked();
        }
    }

    public void OnHintCommandClicked()
    {
        ICommand command = new HintCommand(NetworkManager.Singleton.LocalClientId);
        command.Execute();

        canBeClicked = false;
        RefreshCanBeClicked();
    }

    public void OnHideNumberCommandClicked()
    {
        foreach (ulong id in clientIdsWithoutMe)
        {
            D.Log($"[Command] {id}에게 숫자 숨기기 방해");
            ICommand command = new HideNumberCommand(id);
            command.Execute();
        }

        canBeClicked = false;
        RefreshCanBeClicked();
    }

    private void RefreshCanBeClicked()
    {
        foreach (Button button in commandButtons)
        {
            button.interactable = canBeClicked;
        }
    }
}
