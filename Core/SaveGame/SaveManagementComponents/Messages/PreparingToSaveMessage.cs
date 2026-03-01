/// <summary>
/// This signal is triggered RIGHT before the save system actually serializes the data
/// 
/// Objects can have an ISaveable component and those should link to the PreparingToSaveMessage
/// and then serialize all the properties on save. 
/// 
/// For example if the player position is saved, then you'd have a PlayerSave component
/// that is ISavable and then should run OnSave which would call:
/// 
/// GameDataManager.SetVector2("PlayerPosition", this.transform.position)
/// 
/// </summary>
public class PreparingToSaveMessage : AMessage { }

