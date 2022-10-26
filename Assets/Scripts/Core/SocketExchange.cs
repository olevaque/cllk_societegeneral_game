
using System;

public class SceneName
{
    public const string WEBGAME_JOINROOM = "WebGameJoinRoom";
    public const string WEBGAME_READMISSION = "WebGameReadMission";
    public const string WEBGAME_VOTECAPTAIN = "WebGameVoteCaptain";
    public const string WEBGAME_CHOOSECOMPANY = "WebGameChooseCompany";
    public const string WEBGAME_CONGRATULATION = "WebGameCongratulation";
}


[Serializable]
public class WGJR_Data
{
    public string uuid;
    public string firstname;
    public string initialName;
}

[Serializable]
public class WGVC_Data
{
    public Player captain;
}

[Serializable]
public class WGCC_Data
{
    public int bngL1C1P1, bngL1C2P1, bngL2C1P1, bngL2C2P1, bngL3C1P1, bngL3C2P1;
    public int bngL1C1P2, bngL1C2P2, bngL2C1P2, bngL2C2P2, bngL3C1P2, bngL3C2P2;
    public int bngL1C1P3, bngL1C2P3, bngL2C1P3, bngL2C2P3, bngL3C1P3, bngL3C2P3;
    public int bngL1C1P4, bngL1C2P4, bngL2C1P4, bngL2C2P4, bngL3C1P4, bngL3C2P4;
}

[Serializable]
public class UserVote
{
    public Player player;
    public string vote;
}

[Serializable]
public class TimerData
{
    public int minutes;
    public int seconds;
}

[Serializable]
public class InfoSessionData
{
    public string status;
    public string info;
    public string pseudo;
    public int currentScene;
}

[Serializable]
public class WG_NextSceneData
{
    public int nextScene;
}

[Serializable]
public class Player
{
    public string psckId;
    public string pseudo;
}

[Serializable]
public class ConnectedPlayersData
{
    public Player[] players;
}

[Serializable]
public class MX_VoteProgressData
{
    public UserVote[] userVotes;
}

[Serializable]
public class MX_VoteFailData
{
    public string failMessage;
}

[Serializable]
public class AgreementData
{
    public bool agree;
}

[Serializable]
public class MX_Debug
{
    public string debugMessage;
}