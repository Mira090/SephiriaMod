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
            if (Core.LogFew)
                Melon<Core>.Logger.Msg("Awake: " + name);
        }
        private void Start()
        {
            //if (Core.LogMany)
                //Melon<Core>.Logger.Msg("Start: " + name);
        }
        private void OnDestroy()
        {
            if (Core.LogFew)
                Melon<Core>.Logger.Msg("OnDestroy: " + name);
            //if (Core.LogMany)
                //Melon<Core>.Logger.Msg($"[OnDestroy] {gameObject.name} destroyed!\n{Environment.StackTrace}");
        }
    }
}
