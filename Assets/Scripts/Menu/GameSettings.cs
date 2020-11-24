using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings gameSettings;

    public void SetHumanVsComputer()
    {
        SaveSettings.players[0] = "HUMAN"; //Red
        SaveSettings.players[1] = "COMPUTER"; //Green
    }
}


public static class SaveSettings
{
    public static int playerCount = 0;

    public static string[] players;

    public static string[] playerName;

    public enum GameType
    {
        OFFLINE,
        PRIVATE
    }

    public static GameType gameType; 

    public static string[] winners = new string[3] { string.Empty, string.Empty, string.Empty };

    public static void SetPlayer(string[] _playerType, string[] name)
    {
        playerCount = _playerType.Length;
        players = new string[playerCount];
        playerName = new string[name.Length];
        players = _playerType;
        playerName = name;
    }    

}
