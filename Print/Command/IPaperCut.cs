namespace AsadorMoron.Print.Command
{
    internal interface IPaperCut
    {
        byte[] Full();
        byte[] Partial();
    }
}
