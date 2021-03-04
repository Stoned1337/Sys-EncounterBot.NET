﻿using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;

namespace SysBot.Base
{
    public static class LogUtil
    {
        static LogUtil()
        {
            var config = new LoggingConfiguration();
            Directory.CreateDirectory("logs");
            var logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine("logs", "SysEncounterBotLog.txt"),
                ConcurrentWrites = true,

                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveFileName = Path.Combine("logs", "SysEncounterBotLog.{#}.txt"),
                ArchiveDateFormat = "yyyy-MM-dd",
                ArchiveAboveSize = 104857600, // 100MB (never)
                MaxArchiveFiles = 14, // 2 weeks
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            LogManager.Configuration = config;
        }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static void LogText(string message) => Logger.Log(LogLevel.Info, message);

        // hook in here if you want to forward the message elsewhere???
        public static readonly List<Action<string, string>> Forwarders = new();

        public static DateTime LastLogged { get; private set; } = DateTime.Now;

        public static void LogError(string message, string identity)
        {
            Logger.Log(LogLevel.Error, $"{identity} {message}");
            Log(message, identity);
        }

        public static void LogInfo(string message, string identity)
        {
            Logger.Log(LogLevel.Info, $"{identity} {message}");
            Log(message, identity);
        }

        private static void Log(string message, string identity)
        {
            foreach (var fwd in Forwarders)
                fwd(message, identity);

            LastLogged = DateTime.Now;
        }
    }
}
