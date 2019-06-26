using System;

namespace SZDC.Model.Events {

    /// <summary>
    /// Flags for <see cref="SzdcTrainEvent"/>. Used to determine components placed into acute angles.
    /// </summary>
    [Flags] 
    public enum TrainEventFlags {
        None = 0, // zadny - bezna kota
        IsCancelled = 1, // ruseny
        ReportingObligation = 2, // ohlasovaci povinnost
        OnlyBoarding = 4, // jen nastup
        OnlyExit = 8, // jen vystup
        StoppedForTransportReasons = 16, // zastavuje z dopravnich duvodu
        NotPublishedStop = 32, // neohlasene zastaveni
        LessThanHalfMinute = 64 // mene jak 30 sekund mezi prijezdem a odjezdem
    }
}
