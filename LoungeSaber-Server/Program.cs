using LoungeSaber_Server.MatchRoom;
using LoungeSaber_Server.Models.Networking;
using LoungeSaber_Server.SQL;

namespace LoungeSaber_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                UserData.Instance.Start();
                MapData.Instance.Start();
                MatchRoomDirector.Start();
                Api.Api.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                UserData.Instance.Stop();
                MapData.Instance.Stop();
            }
        }

        public static void LogError(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}