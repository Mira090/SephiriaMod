using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Utilities
{
    public class LogComponent : MonoBehaviour
    {
        public static readonly bool LogAwake = true;
        public static readonly bool LogStart = false;
        private void Awake()
        {
            if (LogAwake)
                Melon<Core>.Logger.Msg("Awake: " + name);
        }
        private void Start()
        {
            if (LogStart)
                Melon<Core>.Logger.Msg("Start: " + name);
        }
        private void OnDestroy()
        {
            if (LogAwake)
                Melon<Core>.Logger.Msg("OnDestroy: " + name);
            if (LogStart)
                Melon<Core>.Logger.Msg($"[OnDestroy] {gameObject.name} destroyed!\n{Environment.StackTrace}");
        }
    }
}
