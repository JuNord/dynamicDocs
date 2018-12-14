namespace RestService.Model
{
    public enum AuthorizationResult
    {
        Authorized,
        NoPermission,
        InvalidLogin,
        Permitted,
        InvalidFormat
    }
}