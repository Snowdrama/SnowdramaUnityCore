using UnityEngine;

//this object is simply used as a storage for a key that lets us load a scene

[CreateAssetMenu(fileName = "SceneTransitionKey", menuName = "Snowdrama/Transitions/SceneTransitionKey")]
public class SceneController_TransitionKeyObject : ScriptableObject
{
    [SerializeField] private string _sceneName;
    public string SceneName { get { return _sceneName; } }
    public void GoToScene()
    {
        SceneControllerJSONTest.GoToScene(_sceneName);
    }
}
