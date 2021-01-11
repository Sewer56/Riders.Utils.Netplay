﻿namespace Riders.Tweakbox.Components.Netplay.Sockets.Helpers
{
    // TODO: Eliminate this class.
    public class ClientState
    {
        /// <summary>
        /// Client's last latency readout.
        /// </summary>
        public int Latency = 999;

        /// <summary>
        /// Client has skipped intro cutscene and is ready to start the race.
        /// </summary>
        public bool ReadyToStartRace = false;

        /// <summary>
        /// Client is ready to sync SRand value.
        /// </summary>
        public bool SRandSyncReady   = false;
    }
}
