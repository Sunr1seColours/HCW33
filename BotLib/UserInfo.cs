namespace BotLib;

public class UserInfo
{
    public enum UserStates
    {
        None,
        InAction,
        UploadingFiles,
        
    }
    
    public UserStates State { get; set; }
    public List<Stream> Files { get; set; }

    public UserInfo()
    {
        Files = new List<Stream>();
        State = UserStates.None;
    }
}