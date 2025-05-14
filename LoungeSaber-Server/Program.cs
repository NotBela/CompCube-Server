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
    }
}