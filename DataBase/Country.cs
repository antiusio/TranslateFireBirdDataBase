using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Country
    {
        public Country()
        {

        }
        public Country(int COUNTRY_ID, string COUNTRY_NAME)
        {
            this.COUNTRY_ID = COUNTRY_ID;
            this.COUNTRY_NAME = COUNTRY_NAME;
        }
        public Country(object[] country)
        {
            this.COUNTRY_ID = (int)country[0];
            this.COUNTRY_NAME = (string)country[1];
        }
        public int COUNTRY_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
    }
}
