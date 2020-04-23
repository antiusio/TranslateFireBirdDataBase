using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class City
    {
        public City()
        {

        }
        public City(int CITY_ID, int RID, int CID,string CITY_NAME)
        {
            this.CITY_ID = CITY_ID;
            this.RID = RID;
            this.CID = CID;
            this.CITY_NAME = CITY_NAME;
        }
        public City(object[] city)
        {
            this.CITY_ID = (int)city[0];
            this.RID = (int)city[1];
            this.CID = (int)city[2];
            this.CITY_NAME = (string)city[3];
        }
        public int CITY_ID { get; set; }
        public int RID { get; set; }
        public  int CID { get; set; }
        public string CITY_NAME { get; set; }
    }
}

