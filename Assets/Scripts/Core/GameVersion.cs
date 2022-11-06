using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVersion
{
    public static string SessionUUID = string.Empty;
    public static bool IsVersionA = true;

    public static Sprite GetCompany1Icon()
    {
        if (IsVersionA) return Resources.Load<Sprite>("LogoCompanies/LogoIzat");
        else return Resources.Load<Sprite>("LogoCompanies/LogoMalanium");
    }

    public static Sprite GetCompany2Icon()
    {
        if (IsVersionA) return Resources.Load<Sprite>("LogoCompanies/LogoFortisio");
        else return Resources.Load<Sprite>("LogoCompanies/LogoBankoleo");
    }

    public static string GetCompany1Name()
    {
        if (IsVersionA) return "Izat";
        else return "Malanium";
    }

    public static string GetCompany2Name()
    {
        if (IsVersionA) return "Fortisio";
        else return "Bankoleo";
    }
}