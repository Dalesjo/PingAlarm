﻿namespace PingAlarm.Contract
{
    public class PingHostStatus
    {
        public int Failures { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
    }
}