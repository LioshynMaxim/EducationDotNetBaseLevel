namespace Madule1_Task1_ClassLibrary
{
    public static class TeskHelloHelper
    {
        public static string SayHello(string username)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            return $"{currentTime} Hello, {username}!";
        }
    }
}
