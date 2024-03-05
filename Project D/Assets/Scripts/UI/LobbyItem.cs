using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayerText;

    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void Initalise(LobbiesList lobbiesList, Lobby lobby)
    {
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        lobbyPlayerText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbiesList.JoinAsync(lobby);
    }
}
