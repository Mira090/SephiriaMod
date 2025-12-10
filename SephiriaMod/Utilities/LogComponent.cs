using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Utilities
{
    public class LogComponent : MonoBehaviour
    {
        private void Awake()
        {
            Melon<Core>.Logger.Msg("Awake: " + name);
        }
        private void Start()
        {
            Melon<Core>.Logger.Msg("Start: " + name);
        }
        private void OnDestroy()
        {
            Melon<Core>.Logger.Msg("OnDestroy: " + name);
            Melon<Core>.Logger.Msg($"[OnDestroy] {gameObject.name} destroyed!\n{Environment.StackTrace}");
        }
    }
}
