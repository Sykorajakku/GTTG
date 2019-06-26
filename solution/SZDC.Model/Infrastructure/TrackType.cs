using System;

namespace SZDC.Model.Infrastructure {
    
    [Flags]
    public enum TrackType {
        PassengerStation = 1, // zastavka
        OvertakingStation = 2, // stanice
        BranchingOffPoint = 4, // vyhybna
        Halt = 8, // vlecka
        BlockSystem = 16, // hradlo
        TrainAnnunciatingPoint = 32, // hlaska 
        LoadingYard = 64, // nakladiste
        BranchLine = 128 // odbocka
    }
}
