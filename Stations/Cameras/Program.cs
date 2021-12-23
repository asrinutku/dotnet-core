using Dapper;
using Newtonsoft.Json;
using Npgsql;
using RestSharp;
using System;
using System.Configuration;

namespace Cameras
{
    class Program
    {

        public static string getToken()
        {

            var client = new RestClient("service");

            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "password");

            request.AddParameter("username", "xxxx");

            request.AddParameter("password", "xxxx");

            IRestResponse response = client.Execute(request);

            var x = response.Content.Split(',')[0];
            x = x.Remove(x.Length - 1);
            x = x.Substring(17, x.Length - 17);

            return x;
        }

        public static void getCameras(NpgsqlConnection con)
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

                var responseContent = JsonConvert.DeserializeObject<Entities.CameraEntity>(response.Content);

                foreach (var item in responseContent.Data)
                {
                    string sql_objectid = "SELECT objectid FROM xxx WHERE xxx";
                    var station_objectid = con.Query(sql_objectid, new { i.uid });

                    foreach (var oid in station_objectid)
                    {
                        Console.WriteLine(item.Name + oid + i.uid);
                        KameraEkle(con, item.Name, oid.objectid, i.uid);
                    }
                }
            }
        }

            static void KameraEkle(NpgsqlConnection con, String kamera_adi, int durak_id, int uid)
            {
                string sql = @"INSERT INTO xxxx(columns) VALUES (values)";


                try
                {
                    var result = con.Execute(sql, new
                    {
                        kamera_adi,
                        durak_id,
                        uid
                    });
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                }



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

                        getCameras(conn);

                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
        }
    }
