using Dapper;
using Newtonsoft.Json;
using Npgsql;
using RestSharp;
using System;
using System.Configuration;

namespace PeopleCount
{
    class Program
    {

        public static string getToken()
        {

            var client = new RestClient("client");

            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "password");

            request.AddParameter("username", "xxx");

            request.AddParameter("password", "xxx");

            IRestResponse response = client.Execute(request);

            var x = response.Content.Split(',')[0];
            x = x.Remove(x.Length - 1);
            x = x.Substring(17, x.Length - 17);

            return x;
        }

        public static void getPeopleCount(NpgsqlConnection con)
        {

            string sql_sd = "SELECT uid FROM iett_duraklar";
            var res = con.Query(sql_sd);

            foreach (var i in res)
            {
                string url = "service";
                var client = new RestClient(url);

                client.Timeout = -1;

                var request = new RestRequest(Method.GET);

                request.AddHeader("Authorization", "Bearer " + getToken());

                IRestResponse response = client.Execute(request);

                var responseContent = JsonConvert.DeserializeObject<Entities.PeopleCountEntity>(response.Content);

                DolulukGuncelle(con, responseContent.Data, i.uid);


            }
        }

        public static void DolulukGuncelle(NpgsqlConnection con, int Doluluk, int Id)
        {
            string updateQuery = @"UPDATE iett_duraklar SET doluluk = @Doluluk WHERE uid = @Id";

            try
            {
                var result = con.Execute(updateQuery, new
                        {
                            Doluluk,
                            Id
                        });
            }
            catch(Exception er)
            {
                Console.WriteLine(er);
            }

        }

        static void Main(string[] args)
        {
            var delay = ConfigurationManager.AppSettings["delay"];
            do
            {
                while (!Console.KeyAvailable)
                {
                    var _connString = ConfigurationManager.ConnectionStrings["nc_db"].ToString();

                    using (var conn = new NpgsqlConnection(_connString))
                    {
                        try
                        {
                            conn.Open();

                            Console.WriteLine("baglantı basarılı");
                            getPeopleCount(conn);

                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    System.Threading.Thread.Sleep(Convert.ToInt32(delay));
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }
    }
}
