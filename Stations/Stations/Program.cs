using Dapper;
using Newtonsoft.Json;
using Npgsql;
using RestSharp;
using System;
using System.Configuration;


namespace Stations
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

            return x ;
        }

        public static void getStations(NpgsqlConnection con)
        {
            var client = new RestClient("service");

            client.Timeout = -1;

            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", "Bearer " + getToken());

            IRestResponse response = client.Execute(request);

            var responseContent = JsonConvert.DeserializeObject<Entities.StationEntity>(response.Content);


            foreach (var item in responseContent.Data)
            {
                KapasiteGuncelle(con, item.PeopleCapacity, item.Id);
            }

        }

        public static void KapasiteGuncelle(NpgsqlConnection con,int Kapasite,int Id)
        {
            string updateQuery = @"UPDATE iett_duraklar SET kapasite = @Kapasite WHERE uid = @Id";

            var result = con.Execute(updateQuery, new
            {
                Kapasite,
                Id
            });

           
        }


        static void Main(string[] args)
        {
            var _connString = ConfigurationManager.ConnectionStrings["nc_db"].ToString();

            using (var conn = new NpgsqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    
                    Console.WriteLine("baglantı basarılı");
                    getStations(conn);

                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine(ex);
                }
            }
            
        }
    }
}
