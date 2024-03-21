namespace BotLib;

/// <summary>
/// Represents all needed information about user.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Number of action which user chose. Can be only in [0; 5].
    /// </summary>
    private int _typeOfAction;
    
    /// <summary>
    /// Enumeration with states which user may has.
    /// </summary>
    public enum UserStates
    {
        None,
        WaitingForCallback,
        UploadingFile,
        ChoosingAttributeForSorting,
        ChoosingAttributeForSelection,
        EnteringValueForSelection
    }

    /// <summary>
    /// Property of type of action which user chose.
    /// 0 - no action.
    /// 1 - selection by AdmArea.
    /// 2 - selection by District.
    /// 3 - selection by AdmArea and Coordinates.
    /// 4 - sorting in alphabetical order.
    /// 5 - sorting in reversed alphabetical order.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Number isn't in [0; 5].</exception>
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

    /// <summary>
    /// Represents ID of last callback which was answered.
    /// </summary>
    public string? LastQueryId { get; set; }
    
    /// <summary>
    /// Property which represents state which user has.
    /// </summary>
    public UserStates State { get; set; }
    
    /// <summary>
    /// Path to file in special directory 'receivedFiles' which was downloaded there from user.
    /// </summary>
    public string? File { get; set; }
    
    /// <summary>
    /// Property which represents type of file which user send to bot. True is in case when file has .csv extension.
    /// </summary>
    public bool? IsCsv { get; set; }
    
    /// <summary>
    /// Value of parameter to found in file.
    /// </summary>
    public string? ValueForSelection { get; set; }
    
    /// <summary>
    /// Base constructor with no parameters. Automatically sets user's state to none.
    /// </summary>
    public UserInfo()
    {
        State = UserStates.None;
    }
}