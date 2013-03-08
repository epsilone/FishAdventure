// unused
public class AIManager
{
    private static AIManager INSTANCE;

    private static AIManager GetInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new AIManager();
        }
        return INSTANCE;
    }
}