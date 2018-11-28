namespace RestService
{
    public enum UploadResult
    {
        SUCCESS,
        FAILED_FILEEXISTS,
        FAILED_ID_EXISTS,
        FAILED_FILE_OR_TYPE_INVALID,
        FAILED_OTHER,
        USER_EXISTS,
        NO_PERMISSION,
        INVALID_LOGIN
    }
}