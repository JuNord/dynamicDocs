namespace RestService
{
    public enum UploadResult
    {
        Success,
        FailedFileexists,
        FailedIdExists,
        FailedFileOrTypeInvalid,
        FailedOther,
        UserExists,
        NoPermission,
        InvalidLogin,
        MissingLink
    }
}