namespace LogManager.Models;

public class AppConfiguration
{
    public string PassPattern { get; set; } = "^PASS_";
    public string FailPattern { get; set; } = "^FAIL_";
    public SftpConfiguration Sftp { get; set; } = new();
}

public class SftpConfiguration
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 4422;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RemoteLogPath { get; set; } = "/logs";
}
