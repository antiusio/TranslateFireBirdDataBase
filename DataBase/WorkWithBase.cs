using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Translate;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace DataBase
{
    //Directory.GetCurrentDirectory() + "\\dataBase\\TEMV1.GDB"
    public class WorkWithBase : INotifyPropertyChanged
    {
        static string TranslatedCountiesFile = Directory.GetCurrentDirectory() + "\\savedData\\" + "GeoCountries.json";
        static string TranslatedCitiesFile = Directory.GetCurrentDirectory() + "\\savedData\\" + "GeoCities.json";
        Translator translator;
        public WorkWithBase(string path, string host="localhost")
        {
            Path = path;
            Host = host;
            Countries = new AsyncObservableCollection<Country>();
            Cities = new AsyncObservableCollection<City>();
            using (var connection = new FbConnection("database=" + Host + ":" + Path + ";user=sysdba;password=masterkey"))
            {
                connection.Open();
                using (var command = new FbCommand("select COUNTRY_ID, COUNTRY_NAME from GEO_COUNTRIES", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            Countries.Add(new Country(values));
                        }
                    }
                }
                using (var command = new FbCommand("select CITY_ID, RID, CID, CITY_NAME from GEO_CITIES", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            Cities.Add(new City(values));
                        }
                    }
                }
            }
            TranslatedCountries = new AsyncObservableCollection<Country>();
            TranslatedCities = new AsyncObservableCollection<City>();
            try
            {
                TranslatedCountries = GetData<Country>(TranslatedCountiesFile);
            }
            catch(Exception ex)
            {
            }
            try { TranslatedCities = GetData<City>(TranslatedCitiesFile); }
            catch { }
            
        }
        public string Path { get; set; }
        public string Host{ get; set; }
        private AsyncObservableCollection<Country> countries;
        public AsyncObservableCollection<Country> Countries
        {
            get { return countries; }
            set { countries = value; OnPropertyChanged("Countries"); }
        }
        //Translated
        private AsyncObservableCollection<Country> translatedCountries;
        public AsyncObservableCollection<Country> TranslatedCountries
        {
            get { return translatedCountries; }
            set { translatedCountries = value; OnPropertyChanged("TranslatedCountries"); }
        }
        private AsyncObservableCollection<City> cities;
        public AsyncObservableCollection<City> Cities
        {
            get { return cities; }
            set { cities = value; OnPropertyChanged("Cities"); }
        }
        private AsyncObservableCollection<City> translatedCities;
        public AsyncObservableCollection<City> TranslatedCities
        {
            get { return translatedCities; }
            set { translatedCities = value; OnPropertyChanged("TranslatedCities"); }
        }

        
            
        public Thread TranslateGeoCountries( )
        {
            Thread t = new Thread(()=> 
            {
                TranslatedCountries.Clear();
                translator = new Translator();
                foreach (var country in Countries)
                {
                    var countryCopy = new Country();
                    countryCopy.COUNTRY_ID = country.COUNTRY_ID;
                    countryCopy.COUNTRY_NAME = country.COUNTRY_NAME;
                    countryCopy.COUNTRY_NAME = translator.Translate(countryCopy.COUNTRY_NAME);
                    TranslatedCountries.Add(countryCopy);
                }
                SaveData<Country>(  TranslatedCountiesFile, TranslatedCountries);
                translator.CloseBrowser();
            });
            t.Start();
            return t;
        }
        
        public void SaveCountriesToFile()
        {
            SaveData<Country>(TranslatedCountiesFile, TranslatedCountries);
        }
        public void SaveCountriesToBase()
        {
            using (var connection = new FbConnection("database=" + Host + ":" + Path + ";user=sysdba;password=masterkey"))
            {
                connection.Open();
                foreach (var country in translatedCountries)
                {//select COUNTRY_ID, COUNTRY_NAME from GEO_COUNTRIES"
                    using (var command2 = new FbCommand("UPDATE GEO_COUNTRIES SET COUNTRY_NAME = '" + country.COUNTRY_NAME + "' WHERE COUNTRY_ID = '" + country.COUNTRY_ID + "'", connection))
                    {
                        var v = command2.ExecuteNonQuery();
                    }
                }
                        
                 
            }
        }
        public Thread TranslateGeoCities()
        {
            Thread t = new Thread(() =>
            {
                translator = new Translator();
                foreach (var city in Cities)
                {
                    var cityCopy = new City();
                    cityCopy.CITY_NAME = translator.Translate(city.CITY_NAME);
                    cityCopy.CID = city.CID;
                    cityCopy.CITY_ID = city.CITY_ID;
                    cityCopy.RID = city.RID;
                    //city.CITY_NAME = translator.Translate(city.CITY_NAME);
                    TranslatedCities.Add(cityCopy);
                }
                SaveData<City>(TranslatedCitiesFile, TranslatedCities);
                translator.CloseBrowser();
            });
            t.Start();
            return t;
        }
        public void SaveCitiesToFile()
        {
            SaveData<City>(TranslatedCitiesFile, TranslatedCities);
        }
        public void SaveCitiesToBase()
        {
            using (var connection = new FbConnection("database=" + Host + ":" + Path + ";user=sysdba;password=masterkey"))
            {
                connection.Open();
                foreach (var city in translatedCities)
                {//select COUNTRY_ID, COUNTRY_NAME from GEO_COUNTRIES"
                    using (var command2 = new FbCommand("UPDATE GEO_CITIES SET CITY_NAME = '" + city.CITY_NAME + "' WHERE CITY_ID = '" + city.CITY_ID + "'", connection))
                    {
                        try
                        {
                            var v = command2.ExecuteNonQuery();
                        }
                        catch(Exception ex) { }
                    }
                }


            }
        }
        private void SaveData<T>(string filePath, AsyncObservableCollection<T> list)
        {
            string json = JsonConvert.SerializeObject(list);
            File.WriteAllText(filePath,json);
        }
        private AsyncObservableCollection<T> GetData<T>(string filePath)
        {
            string s = File.ReadAllText(filePath);
            AsyncObservableCollection<T> rez = JsonConvert.DeserializeObject<AsyncObservableCollection<T>>(s);
            return rez;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        
    }
}
