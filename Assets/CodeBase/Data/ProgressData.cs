﻿using System;

namespace CodeBase.Data
{
    [Serializable]
    public class ProgressData
    {
        public PlayerData PlayerData = new();
        public SettingsData SettingsData = new();
    }
}