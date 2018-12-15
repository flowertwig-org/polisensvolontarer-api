namespace Api.Contracts
{
    public class ChangePasswordResult
    {
        public bool IsSuccess { get; set; }
        public bool IsWeakPassword { get; set; }
        public int Warning { get; set; }
    }
}