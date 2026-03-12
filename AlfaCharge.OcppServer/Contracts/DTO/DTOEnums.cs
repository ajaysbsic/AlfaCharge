namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public enum ConfigurationStatus16 
    { 
        Accepted,
        Rejected,
        RebootRequired,
        NotSupported
    }

    public enum DiagnosticsStatus16
    {
        Idle,
        Uploaded,
        UploadFailed,
        Uploading
    }


    public enum FirmwareStatus16
    {
        Downloaded,
        DownloadFailed,
        Downloading,
        Idle,
        InstallationFailed,
        Installing,
        Installed
    }


    public enum AttributeEnumType201
    {
        Actual,
        Target,
        MinSet
    }

    public enum MutabilityEnumType201 
    {
        ReadOnly,
        ReadWrite,
        WriteOnly
    }

    public enum AttributeStatusEnumType201
    {
        Accepted,
        Rejected,
        UnknownComponent,
        UnknownVariable,
        NotSupportedAttributeType,
        OutOfRange
    }

    public enum ReportBaseEnumType201
    {
        ConfigurationInventory,
        FullInventory,
        SummaryInventory
    }

    public enum GenericDeviceModelStatus201
    {
        Accepted,
        Rejected,
        NotSupported
    }

    public enum LogEnumType201
    {
        DiagnosticsLog,
        SecurityLog
    }

    public enum LogStatusEnumType201
    {
        Accepted,
        Rejected,
        AcceptedCanceled
    }

    public enum UploadLogStatusEnumType201
    { 
        BadMessage, 
        PermissionDenied, 
        Accepted, 
        Rejected, 
        Aborted, 
        NotSupportedOperation 
    }

    public enum FirmwareStatusEnumType201
    {
        Idle,
        DownloadScheduled, 
        Downloading, 
        Downloaded, 
        DownloadFailed,
        Installing, 
        Installed, 
        InstallationFailed
    }

    public enum UpdateFirmwareStatusEnumType201
    {
        Accepted,
        Rejected,
        AcceptedCanceled 
    }

}