using DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TranslateBase
{
    class MainDataContext : INotifyPropertyChanged
    {
        private WorkWithBase workWithBase;
        public WorkWithBase WorkWithBase
        {
            get { return workWithBase; }
            set { workWithBase = value; OnPropertyChanged("WorkWithBase"); }
        }
        public MainDataContext()
        {

        }
        public MainDataContext(WorkWithBase workWithBase):this(workWithBase.Countries,workWithBase.TranslatedCountries,
            workWithBase.Cities,workWithBase.TranslatedCities)
        {
            this.workWithBase = workWithBase;
        }
        public MainDataContext(AsyncObservableCollection<Country> Countries, AsyncObservableCollection<Country> TranslatedCountries,
            AsyncObservableCollection<City> Cities, AsyncObservableCollection<City> TranslatedCities)
        {
            this.TranslatedCountries = TranslatedCountries;
            this.Countries = Countries;
            this.Cities = Cities;
            this.TranslatedCities = TranslatedCities;
            ViewedCountries = new AsyncObservableCollection<Country>();
            ViewedTranslatedCountries = new AsyncObservableCollection<Country>();
            ViewedCities = new AsyncObservableCollection<City>();
            ViewedTranslatedCities = new AsyncObservableCollection<City>();
            itemCount = 10;
            RefreshViewed();
        }
        private int tabIndex;
        public int TabIndex
        {
            get { return tabIndex; }
            set
            {
                tabIndex = value; OnPropertyChanged("TabIndex");
                start = 0;
                //itemCount = 0;
                RefreshViewed();
            }
        }
        private AsyncObservableCollection<Country> countries;
        public AsyncObservableCollection<Country> Countries
        {
            get { return countries; }
            set { countries = value; OnPropertyChanged("Countries"); }
        }
        private AsyncObservableCollection<Country> viewedCountries;
        public AsyncObservableCollection<Country> ViewedCountries
        {
            get { return viewedCountries; }
            set { viewedCountries = value; OnPropertyChanged("ViewedCountries"); }
        }
        //Translated
        private AsyncObservableCollection<Country> translatedCountries;
        public AsyncObservableCollection<Country> TranslatedCountries
        {
            get { return translatedCountries; }
            set { translatedCountries = value; OnPropertyChanged("TranslatedCountries"); }
        }
        private AsyncObservableCollection<Country> viewedTranslatedCountries;
        public AsyncObservableCollection<Country> ViewedTranslatedCountries
        {
            get { return viewedTranslatedCountries; }
            set { viewedTranslatedCountries = value; OnPropertyChanged("ViewedTranslatedCountries"); }
        }

        private AsyncObservableCollection<City> cities;
        public AsyncObservableCollection<City> Cities
        {
            get { return cities; }
            set { cities = value; OnPropertyChanged("Cities"); }
        }
        private AsyncObservableCollection<City> viewedCities;
        public AsyncObservableCollection<City> ViewedCities
        {
            get { return viewedCities; }
            set { viewedCities = value; OnPropertyChanged("ViewedCities"); }
        }
        private AsyncObservableCollection<City> translatedCities;
        public AsyncObservableCollection<City> TranslatedCities
        {
            get { return translatedCities; }
            set { translatedCities = value; OnPropertyChanged("TranslatedCities"); }
        }
        private AsyncObservableCollection<City> viewedTranslatedCities;
        public AsyncObservableCollection<City> ViewedTranslatedCities
        {
            get { return viewedTranslatedCities; }
            set { viewedTranslatedCities = value; OnPropertyChanged("ViewedTranslatedCities"); }
        }

        #region Private Fields
        private int start = 0;
        private int itemCount = 10;
        private int totalItems = 0;

        private ICommand firstCommandCountries;
        private ICommand previousCommandCountries;
        private ICommand nextCommandCountries;
        private ICommand lastCommandCountries;

        private ICommand translateGeoCountriesCommand;
        private ICommand translateGeoCitiesCommand;

        private ICommand countriesSaveToBaseCommand;
        private ICommand countriesSaveToFileCommand;

        private ICommand citiesSaveToBaseCommand;
        private ICommand citiesSaveToFileCommand;
        #endregion
        public int Start { get { return start; } }

        /// <summary>
        /// Gets the index of the last item in the list.
        /// </summary>
        public int End { get { return start + itemCount < totalItems ? start + itemCount : totalItems; } }

        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        public int TotalItems { get { return totalItems; } }

        /// <summary>
        /// Gets the command for moving to the first page of products.
        /// </summary>
        public ICommand FirstCommand
        {
            get
            {
                if (firstCommandCountries == null)
                {
                    firstCommandCountries = new RelayCommand
                    (
                        param =>
                        {
                            start = 0;
                            RefreshViewed();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }
                return firstCommandCountries;
            }
        }
        /// <summary>
        /// Gets the command for moving to the previous page of countries.
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommandCountries == null)
                {
                    previousCommandCountries = new RelayCommand
                    (
                        param =>
                        {
                            start -= itemCount;
                            RefreshViewed();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }
                return previousCommandCountries;
            }
        }

        /// <summary>
        /// Gets the command for moving to the next page of countries.
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (nextCommandCountries == null)
                {
                    nextCommandCountries = new RelayCommand
                    (
                        param =>
                        {
                            start += itemCount;
                            RefreshViewed();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }
                return nextCommandCountries;
            }
        }

        /// <summary>
        /// Gets the command for moving to the last page of Countries.
        /// </summary>
        public ICommand LastCommand
        {
            get
            {
                if (lastCommandCountries == null)
                {
                    lastCommandCountries = new RelayCommand
                    (
                        param =>
                        {
                            start = (totalItems / itemCount - 1) * itemCount;
                            start += totalItems % itemCount == 0 ? 0 : itemCount;
                            RefreshViewed();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }
                return lastCommandCountries;
            }
        }
        public ICommand TranslateGeoCountriesCommand
        {
            get
            {
                if (translateGeoCountriesCommand == null)
                {
                    translateGeoCountriesCommand = new RelayCommand
                    (
                        param =>
                        {
                            workWithBase.TranslateGeoCountries();
                        },
                        param =>
                        {
                            return true;
                        }
                    );
                }
                return translateGeoCountriesCommand;
            }
        }
        
        public ICommand TranslateGeoCitiesCommand
        {
            get
            {
                if (translateGeoCitiesCommand == null)
                {
                    translateGeoCitiesCommand = new RelayCommand
                    (
                        param =>
                        {
                            workWithBase.TranslateGeoCities();
                        },
                        param =>
                        {
                            return true;
                        }
                    );
                }
                return translateGeoCitiesCommand;
            }
        }
        public ICommand CountriesSaveToBaseCommand
        {
            get
            {
                if (countriesSaveToBaseCommand == null)
                {
                    countriesSaveToBaseCommand = new RelayCommand
                        (param => { workWithBase.SaveCountriesToBase(); }, param => { return true; });
                }
                return countriesSaveToBaseCommand;
            }
        }
        public ICommand CountriesSaveToFileCommand
        {
            get
            {
                if (countriesSaveToFileCommand == null)
                {
                    countriesSaveToFileCommand = new RelayCommand
                        (param => { workWithBase.SaveCountriesToFile(); }, param => { return true; });
                }
                return countriesSaveToFileCommand;
            }
        }
        public ICommand CitiesSaveToFileCommand
        {
            get
            {
                if (citiesSaveToFileCommand == null)
                {
                    citiesSaveToFileCommand = new RelayCommand
                        (param => { workWithBase.SaveCitiesToFile(); }, param => { return true; });
                }
                return citiesSaveToFileCommand;
            }
        }
        public ICommand CitiesSaveToBaseCommand
        {
            get
            {
                if (citiesSaveToBaseCommand == null)
                {
                    citiesSaveToBaseCommand = new RelayCommand
                        (param => { workWithBase.SaveCitiesToBase(); }, param => { return true; });
                }
                return citiesSaveToBaseCommand;
            }
        }
        public void GetViewedItems<T>(AsyncObservableCollection<T> sourceCollection, AsyncObservableCollection<T> viewedCollection, int start, int itemCount, out int totalItems)
        {
            totalItems = 0;
            try
            {
                totalItems = sourceCollection.Count;
                viewedCollection.Clear();
                for (int i = start; i < start + itemCount && i < totalItems; i++)
                {
                    viewedCollection.Add(sourceCollection.ElementAt(i));
                }
            }
            catch { }
        }
        private void RefreshViewed()
        {
            if (TabIndex == 0)
            {
                GetViewedItems(Countries, ViewedCountries, Start, itemCount, out totalItems);
                int totalItemsTrash;
                GetViewedItems(TranslatedCountries, ViewedTranslatedCountries, Start, itemCount, out totalItemsTrash);
            }
            if (TabIndex == 1)
            {
                GetViewedItems(Cities, ViewedCities, Start, itemCount, out totalItems);
                int totalItemsTrash;
                GetViewedItems(TranslatedCities, ViewedTranslatedCities, Start, itemCount, out totalItemsTrash);
            }

            NotifyPropertyChanged("Start");
            NotifyPropertyChanged("End");
            NotifyPropertyChanged("TotalItems");

        }
        private void NotifyPropertyChanged(string propertyName)

        {

            if (PropertyChanged != null)

            {

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
