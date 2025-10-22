using UnityEngine;

[CreateAssetMenu(fileName = "SceneTransitionKey", menuName = "Snowdrama/Transitions/SceneTransitionKey")]
public class SceneController_TransitionKeyObject : ScriptableObject
{
    [SerializeField] private string _sceneName;
    public string SceneName { get { return _sceneName; } }
    public void GoToScene()
    {
        SceneController.GoToScene(_sceneName);
    }
}
