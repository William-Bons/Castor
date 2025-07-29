using Castor.database.tab_medis;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window, INotifyPropertyChanged, IConsoleMessage, IDialog
    {
        private static docdep _lastUser;
        private static dep _selectedDepartment;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event ConsoleMessageHandler ConsoleMessage;

        public SelectUser()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (o, e) =>
            {
                using (MedisContext medis = new MedisContext())
                {
                    if(!medis.Database.CanConnect())
                    {
                        ConsoleMessage?.Invoke("Cannot set connection to POSTGREE");
                        IsEnabled = false;
                        return;
                    }

                    Departments = medis.dep
                        .Where(dep => dep.rootid == 1482) // ref Department
                        .Include(dep => dep.Docdeps)
                        .ToList();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Departments)));
                }

                long depid = Properties.Settings.Default.LastSelectedDep;
                if (depid>0)
                {
                    SelectedDepartment = Departments.Where(dep => dep.keyid == depid).ToList().Count > 0 ?
                        Departments.Where(dep => dep.keyid == depid).First() : null;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDepartment)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Users)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastConnectedUser)));
                }

            };
        }

        public ICollection<dep> Departments { get; set; }
        
        public dep SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment= value;
                Properties.Settings.Default.LastSelectedDep = _selectedDepartment.keyid;
                Properties.Settings.Default.Save();

                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(SelectedDepartment)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Users)));

                long keyid = Properties.Settings.Default.LastConnectedUserId;
                if (keyid > 0)
                {
                    _lastUser = Users.Where(u => u.keyid == keyid).ToList().Count>0 ? 
                        Users.Where(u => u.keyid == keyid)?.First() : null;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastConnectedUser)));
                    ConsoleMessage?.Invoke($"selected user: {_lastUser?.text}");
                }
            }
        }
        public ICollection<docdep> Users
        {
            get => _selectedDepartment?.Docdeps.Where(doc => doc.positionid == 50320 || doc.positionid==50454).ToList();
        }
        public docdep LastConnectedUser
        {
            get => _lastUser;
            set
            {
                _lastUser = value;
                Properties.Settings.Default.LastConnectedUserId = _lastUser?.keyid ?? 0;
                Properties.Settings.Default.Save();
            }
        }
        public static docdep ConnectedUser => _lastUser;
        public static dep SelectedDep => _selectedDepartment;

        private void ConnectAndRegisterUser(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
