namespace Snowdrama.Transition
{
    public enum TransitionState
    {
        None,
        Start,
        HidingScene,
        SceneHidden,
        StartUnload,
        WaitingforUnload,
        StartLoad,
        WaitingForLoad,
        FakeTimeBuffer,
        ShowingScene,
        End,
    }
}