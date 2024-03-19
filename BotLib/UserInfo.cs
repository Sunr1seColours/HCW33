namespace BotLib;

public class UserInfo
{
    private int _typeOfAction;

    public enum UserStates
    {
        None,
        WaitingForCallback,
        UploadingFiles,
        ChoosingAttributeForSorting,
        ChoosingAttributeForSelection,
        EnteringValueForSelection
    }

    public int TypeOfAction
    {
        get => _typeOfAction;
        set
        {
            if (value <= 5 && value >= 0)
            {
                _typeOfAction = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"{_typeOfAction} ", "Type of Action can be only in range [0; 5].");
            }
        }
    }

    public UserStates State { get; set; }
    public Stream File { get; set; }
    
    public string ValueForSelection { get; set; }
    
    public UserInfo()
    {
        State = UserStates.None;
    }
}