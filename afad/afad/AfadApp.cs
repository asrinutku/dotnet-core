using Dapper;
using Newtonsoft.Json;
using Npgsql;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static afad.Entities.AfadEntity;

namespace afad
{

    class AfadApp : IDataSource
    {
        public override string Name { get { return "ext_afad_2"; } }
       
        public override NetcadData ReadData()
        {
            
            var result = new NetcadData();

            result.Name = this.Name;

            var client = new RestClient("service");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            IRestResponse response = client.Execute(request);

            while (response.Content.Length < 1000)
            {
                Console.WriteLine("Tekrar istek atma denemesi..");
                response = client.Execute(request);
            }

            var data = JsonConvert.DeserializeObject<Entities.AfadEntity>(response.Content);

            foreach (var item in data.features)
            {
                
                try

                {
                    var newRow = new NetcadDataRowCollection();

                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_adres, Name = "Iletisim_adres", IsGeometry = false });
                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_calismaSaatleri, Name = "Iletisim_calismaSaatleri", IsGeometry = false });
                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_eposta, Name = "Iletisim_eposta", IsGeometry = false });
                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_faks, Name = "Iletisim_faks", IsGeometry = false });
                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_iletisimTalimatlari, Name = "Iletisim_iletisimTalimatlari", IsGeometry = false });
                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_telefon, Name = "Iletisim_telefon", IsGeometry = false });
                    newRow.Add(new NetcadDataRow() { Value = item.properties.Iletisim_webAdresi, Name = "Iletisim_webAdresi", IsGeometry = false });

                    if (item.geometry != null && item.geometry.coordinates != null)

                    {

                        if (item.geometry.type == "Polygon")

                        {

                            var coordinates = item.geometry.coordinates[0].ToString().Replace(Environment.NewLine, "");

                            var json = "{\"coordinates\":[" + coordinates + "],\"type\":\"Polygon\"}";

                            var polygon = JsonConvert.DeserializeObject<Polygon>(json);

                            var str = string.Join(", ", polygon.coordinates.ToList()[0].ToList().Select(c => c.ToString().Replace(",", ".") + " " + c.ToString().Replace(",", ".")));
                            
                            newRow.Add(new NetcadDataRow() { Value = "POLYGON((" + str + "))", Name = "poly", IsGeometry = true });

                        }


                        else if (item.geometry.type == "Point")
                        {
                            newRow.Add(new NetcadDataRow() { Value = $"POINT({item.geometry.coordinates[0]} {item.geometry.coordinates[1]})", Name = "poly", IsGeometry = true });
                        }



                        else if (item.geometry.type == "MultiPolygon")

                        {
                            var cb = new List<string>();

                            foreach (var cc in item.geometry.coordinates)

                            {

                                var coordinates = cc.ToString().Replace(Environment.NewLine, "");

                                var json = "{\"coordinates\":[" + coordinates + "],\"type\":\"Polygon\"}";

                                var polygon = JsonConvert.DeserializeObject<Polygon>(json);

                                var str = string.Join(", ", polygon.coordinates.ToList()[0].ToList().Select(c => c.ToString().Replace(",", ".") + " " + c.ToString().Replace(",", ".")));

                                cb.Add("((" + str + "))");

                            }

                            newRow.Add(new NetcadDataRow() { Value = "MULTIPOLYGON((" + string.Join(",", cb) + "))", Name = "poly", IsGeometry = true });

                        }

                    }
                result.Rows.Add(newRow);
                }

  
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
             
            }
            return result;

        }

        public static void addData(NetcadData data, NpgsqlConnection con)
        {
            foreach(var item in data.Rows)
            {

                string insertQuery = @"INSERT INTO ext_afad_2";

                try
                {
                    var result = con.Execute(insertQuery, new
                    {

                      
                        Iletisim_adres = item[4].Value,
                        Iletisim_calismaSaatleri = item[5].Value,
                        Iletisim_eposta = item[6].Value,
                        Iletisim_faks = item[7].Value,
                        Iletisim_iletisimTalimatlari = item[8].Value,
                        Iletisim_telefon = item[9].Value,
                        Iletisim_webAdresi = item[10].Value,
                    });
                }
                catch (Exception er)
                {
                    Console.WriteLine(er);
                }
            }

          
        }

        public static void Main(string[] args)
        {
            var _connString = ConfigurationManager.ConnectionStrings["nc_db"].ToString();
            IDataSource afadApp;
            afadApp = new AfadApp();

            using (var conn = new NpgsqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("veritabanı baglantı basarılı");

                    NetcadData result = afadApp.ReadData();
                    addData(result,conn);

                    Console.WriteLine("Veriler db ye eklendi ");


                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Console.Read();
        }
    }
}
