using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEvents
{
    [UnityEngine.CreateAssetMenu(fileName = "GameEvent", menuName = "Game Events/Event")]
    public class GameEvent: ScriptableObject
    {
        #if UNITY_EDITOR
        [OnValueChanged(nameof(GenerateGuid))]
        #endif
        public bool persistent;
        [ReadOnly] private List<IGameEventListener> listeners = new List<IGameEventListener>();
        [ShowIf(nameof(persistent))]public string guid;

    }
}