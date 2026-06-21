using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CommandGiver : MonoBehaviour
{
    private List<ulong> clientIds = new(4);
    private List<ulong> clientIdsWithoutMe = new(4);

    private void Start()
    {
        TurnManager.Instance.GetAllClientIds(clientIds);

        for (int i = 0; i < clientIds.Count; i++)
        {
            if (clientIds[i] == NetworkManager.Singleton.LocalClientId) continue;

            clientIdsWithoutMe.Add(clientIds[i]);
        }
    }
    
    public void OnHintCommandClicked()
    {
        ICommand command = new HintCommand(NetworkManager.Singleton.LocalClientId);
    }

    public void OnHideNumberCommandClicked()
    {
        foreach (ulong id in clientIdsWithoutMe)
        {
            ICommand command = new HideNumberCommand(id);
        }
    }
}
