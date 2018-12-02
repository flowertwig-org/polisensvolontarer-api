namespace Api.Contracts
{
    public class LoginResult
    {
        public string RedirectUrl { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsWeakPassword { get; set; }
    }
}