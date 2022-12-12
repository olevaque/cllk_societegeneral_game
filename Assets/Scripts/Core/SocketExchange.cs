
using System;

public class SceneName
{
    public const string WEBGAME_JOINROOM = "WebGameJoinRoom";
    public const string WEBGAME_READMISSION = "WebGameReadMission";
    public const string WEBGAME_VOTECAPTAIN = "WebGameVoteCaptain";
    public const string WEBGAME_CHOOSECOMPANY = "WebGameChooseCompany";
    public const string WEBGAME_SPECTATOR = "WebGameSpectator";
    public const string WEBGAME_CONGRATULATION = "WebGameCongratulation";

    public const string APPGAME = "AppGame";
    public const string APPGAME_CONGRATULATION = "AppGameCongratulation";
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
    public string password = string.Empty;
    public string code = string.Empty;
    public string brainteaser = string.Empty;

    public int bngL1C1P1, bngL1C2P1, bngL2C1P1, bngL2C2P1, bngL3C1P1, bngL3C2P1;
    public int bngL1C1P2, bngL1C2P2, bngL2C1P2, bngL2C2P2, bngL3C1P2, bngL3C2P2;
    public int bngL1C1P3, bngL1C2P3, bngL2C1P3, bngL2C2P3, bngL3C1P3, bngL3C2P3;
    public int bngL1C1P4, bngL1C2P4, bngL2C1P4, bngL2C2P4, bngL3C1P4, bngL3C2P4;

    public int company;
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
    public int currentStep;
    public int minutes;
    public int seconds;
}

[Serializable]
public class UuidSessionData
{
    public string uuid;
}

[Serializable]
public class NameSessionData
{
    public string name;
}

[Serializable]
public class InfoSessionData
{
    public string status;
    public string info;
    public string pseudo;
    public int currentScene;
    public bool isVersionA;
}

[Serializable]
public class Docs
{
    public string name;
    public bool isOpen;
    public bool isLock;
    public int nbView;
    public int nbOpen;
    public int timeViewed;
}

[Serializable]
public class Player
{
    public string psckId;
    public string pseudo;
}

[Serializable]
public class PlayerForSpectator
{
    public string psckId;
    public string pseudo;
    public Docs[] docViewed;
    public bool hasDisconnect;
    public int menuTimeViewed;
}

[Serializable]
public class RoomSpectatorInfo
{
    public Player captainForSpectator;
    public PlayerForSpectator[] playersForSpectator;
    public WGCC_Data wgccData;
}

[Serializable]
public class SpectatorData
{
    public bool isVersionA;
    public string name;
    public int currentScene;
    public int currentStep;
    public RoomSpectatorInfo roomSpectatorInfo;
    public string timer;
}

[Serializable]
public class WG_NextSceneData
{
    public int nextScene;
}
[Serializable]
public class WG_NextStepData
{
    public int nextStep;
}

[Serializable]
public class WGCC_OpenCloseDocument
{
    public string docName;
}
[Serializable]
public class WGCC_PasswordData
{
    public string password;
}
[Serializable]
public class WGCC_CodeData
{
    public string code;
}
[Serializable]
public class WGCC_BrainteaserData
{
    public int questionId;
    public string answer;
}

public class CurrentCaptainData
{
    public bool youAreCaptain;
    public Player captain;
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

[Serializable]
public class UnlockData
{
    public int folderUnlock;
}