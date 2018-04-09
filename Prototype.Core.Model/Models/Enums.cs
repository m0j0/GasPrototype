namespace Prototype.Core.Models
{
    public enum WaferStatus
    {
        NotPresentUnresolved,
        NotPresentResolved,
        Unknown,
        Unprocessed,
        InProcess,
        PartialProcessed,
        Processed,
        FailureDuringProcess,
        WarningDuringProcess,
        PositionUnresolved,
        LotNotAssigned,
        DummyUnprocessed,
        DummyInProcess,
        DummyFault,
        DummyWarning,
        DummyUnknown
    }

    public enum WaferWorkingMode
    {
        Standard,
        Rework,
        Gating
    }

    public enum MoveSourceMode
    {
        None,
        Main,
        Slave
    }

    public enum WaferPopupType
    {
        InfoTable,
        Commands
    }
}
