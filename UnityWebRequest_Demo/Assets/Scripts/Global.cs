﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityWebRequestDemo
{
    public class Global
    {
        public static bool isDebuging = false;

        public static Root loadedServerData = null;

        public static Dictionary<int, string> jsonFormet = new Dictionary<int, string>()
        {
            {0, "{\"CloudImageData\":{0}}" }
        };

        public static string cloudDataUri = "https://testinterest.s3.amazonaws.com/interest.json";

        public static string LocalDataPath
        {
            get => Application.persistentDataPath + "/CloudData/";
        }
    }

    [Serializable]
    public class CloudImageData
    {
        public string Name;
        public string PictureUrl;
        public string DisplayName;
        public string Language;
        public int InterestID;
    }

    [Serializable]
    public class Root
    {
        public List<CloudImageData> CloudImageData;
    }
}