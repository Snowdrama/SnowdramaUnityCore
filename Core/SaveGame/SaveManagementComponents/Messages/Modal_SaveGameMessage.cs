/// <summary>
/// Creates a new save
/// 
/// Takes an integer if you want to Create a save in a specific slot or -1 for finding a unused slot
/// 
/// Takes a string of a default name for the save
/// </summary>
public class Modal_SaveGameMessage : AMessage<int, string> { }
