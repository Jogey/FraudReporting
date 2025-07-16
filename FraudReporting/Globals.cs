namespace FraudReporting
{
    /// <summary>
    /// A static class used to store global application variables.
    /// </summary>
    public static class Globals
    {
        //Database locks: prevents concurrent state modifications leading to potential loss of data
        internal static object Lock = new object();
    }
}
