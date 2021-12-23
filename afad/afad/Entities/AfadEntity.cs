using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace afad.Entities
{
    public class AfadEntity
    {


        public string type { get; set; }
        public Feature[] features { get; set; }
        public int totalFeatures { get; set; }
        public int numberMatched { get; set; }
        public int numberReturned { get; set; }
        public DateTime timeStamp { get; set; }
        public Crs crs { get; set; }


        public class Crs
        {
            public string type { get; set; }
            public Properties properties { get; set; }

        }

        public class Properties
        {
            public string name { get; set; }
        }

        public class Feature
        {
            public string type { get; set; }
            public string id { get; set; }
            public Geometry geometry { get; set; }
            public string geometry_name { get; set; }
            public Properties1 properties { get; set; }
        }

        public class Geometry
        {
            public string type { get; set; }
            public object[] coordinates { get; set; }
        }

        public class Properties1
        {
            public object HizmetKonumTipi_adresIleHizmetKonumu { get; set; }
            public object HizmetKonumTipi_binaIleHizmetKonumu { get; set; }
            public object HizmetKonumTipi_DugumNoktasiIleHizmetKonumu { get; set; }
            public object HizmetKonumTipi_faaliyetKompleksiIleHizmetKonumu { get; set; }
            public string Ad { get; set; }
            public string hizmetTipi { get; set; }
            public int tucbsNo { get; set; }
            public string Iletisim_telefon { get; set; }
            public string Iletisim_eposta { get; set; }
            public object Iletisim_faks { get; set; }
            public object Iletisim_webAdresi { get; set; }
            public string Iletisim_adres { get; set; }
            public object Iletisim_calismaSaatleri { get; set; }
            public object Iletisim_iletisimTalimatlari { get; set; }
            public DateTime? surumBaslangicZamani { get; set; }
            public object SorumlulukAlanTipi_AgReferansi_eleman { get; set; }
            public object SorumlulukAlanTipi_cografiYerAdlariIleSorumlulukAlani { get; set; }
            public object SorumlulukAlanTipi_idariBirimIleSorumlulukAlani { get; set; }
            public object SorumlulukAlanTipi_poligonIleSorumlulukAlani { get; set; }
        }


        public class Polygon
        {
            public float[][][] coordinates { get; set; }
            public string type { get; set; }
        }



    }
}
