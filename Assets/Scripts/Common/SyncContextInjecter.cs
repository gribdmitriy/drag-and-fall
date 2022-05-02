﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public class SyncContextInjecter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Inject()
        {
            SynchronizationContext.SetSynchronizationContext(new UniTaskSynchronizationContext());
        }
    }
}