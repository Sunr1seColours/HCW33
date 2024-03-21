namespace BotLib;

public class UserInfo
{
    private int _typeOfAction;
    
    public enum UserStates
    {
        None,
        WaitingForCallback,
        UploadingFile,
        ChoosingAttributeForSorting,
        ChoosingAttributeForSelection,
        EnteringValueForSelection
    }

    public int TypeOfAction
    {
        get => _typeOfAction;
        set
        {
            if (value is <= 5 and >= 0)
            {
                _typeOfAction = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"{_typeOfAction} ", "Type of Action can be only in range [0; 5].");
            }
        }
    }

    public string? LastQueryId { get; set; }
    
    public UserStates State { get; set; }
    public string? File { get; set; }
    public bool? IsCsv { get; set; }
    
    public string? ValueForSelection { get; set; }
    
    public UserInfo()
    {
        State = UserStates.None;
    }
}