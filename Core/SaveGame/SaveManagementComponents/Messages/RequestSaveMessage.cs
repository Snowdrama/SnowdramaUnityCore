/// <summary>
/// This message is used from a save point or from a menu to trigger a save game. 
/// 
/// The int is the requested save slot, if the int passed is -1 than it's considered
/// an auto save, and not a normal save.
/// </summary>
public class RequestSaveMessage : AMessage<int> { }
