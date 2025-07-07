namespace ModJam5;

public static class PingConditionHandler
{
    public static void Setup()
    {
        GlobalMessenger.AddListener("ExitConversation", OnExitConversation);
        GlobalMessenger.AddListener("EnterConversation", OnEnterConversation);
    }

    private static void OnEnterConversation()
    {

    }

    private static void OnExitConversation()
    {
        if (DialogueConditionManager.SharedInstance.GetConditionState("Jam5PingStartMeditation"))
        {
            Locator.GetDeathManager().KillPlayer(DeathType.Meditation);
            PlayerData.SetPersistentCondition("KNOWS_MEDITATION", true);
        }
    }
}
