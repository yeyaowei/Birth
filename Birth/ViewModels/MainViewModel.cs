using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Birth.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer timer = new DispatcherTimer();
        public bool IsBirthDateSet { get; set; }

        public DateTime Now { get; set; } = DateTime.Now;

        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                File.WriteAllText("birthday.ticks", _birthDate.Ticks.ToString());
            }
        }

        public TimeSpan SubstractedDate => Now.Subtract(BirthDate);
        public long PassedSeconds => (long)SubstractedDate.TotalSeconds;

        public int Month => (Now.Year - BirthDate.Year) * 12 - (BirthDate.Month - Now.Month);

        public int Year => Now.Year - BirthDate.Year - ((BirthDate.Month - Now.Month) > 0 ? 1 : 0);

        public int Day => (int)SubstractedDate.TotalDays;

        public int Week => Day / 7;

        public int Hour => (int)SubstractedDate.TotalHours;

        public int Minute => (int)SubstractedDate.TotalMinutes;

        public decimal Age
        {
            get
            {
                var lastBirthDay = new DateTime((BirthDate.Month - Now.Month) > 0 ? Now.Year - 1 : Now.Year, BirthDate.Month, BirthDate.Day);
                return (decimal)Now.Subtract(lastBirthDay).TotalSeconds / (365 * 24 * 60 * 60) + Year;
            }
        }

        public MainViewModel()
        {
            LoadBirthDate();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, e) => Now = DateTime.Now;
            timer.Start();
        }

        private void LoadBirthDate()
        {
            try
            {
                if (File.Exists("birthday.ticks"))
                {
                    BirthDate = new DateTime(long.Parse(File.ReadAllText("birthday.ticks")));
                    IsBirthDateSet = true;
                }
            }
            catch
            {
                BirthDate = new DateTime(2000, 5, 25);
            }
        }

        private bool IsLeapYear(int year)
        {
            return (year % 4 == 0) ? (year % 100 != 0) ? true : (year % 400 == 0) ? true : false : false;
        }
    }
}
