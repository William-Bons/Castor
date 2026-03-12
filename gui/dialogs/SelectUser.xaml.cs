using Castor.database;
using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window, INotifyPropertyChanged, IDialog
    {
        private static dep _selectedDepartment;
        public event PropertyChangedEventHandler? PropertyChanged;

        public SelectUser()
        {
            try
            {
                // получение списка отделений и врачей отделений из учреждения, код учреждения указан в параметре RootDepartmentId
                using (MedisContext medis = new MedisContext())
                {
                    Departments = medis.dep
                        .Where(dep => dep.rootid == Settings.Default.RootDepartmentId) // ref Department Дружносельская ПБ 1482
                        .Include(dep => dep.Docdeps)
                        .ToList();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Departments)));
                }
            }
            catch { }
            
            // Стандартная инициализация диалога
            InitializeComponent();
            DataContext = this;

            Loaded += (o, e) =>
            {
                // если отделение уже ранее выбрано то выбор его в списке отдлений
                long depid = Settings.Default.LastSelectedDep;
                if (depid>0)
                {
                    SelectedDepartment = Departments?.Where(dep => dep.keyid == depid)?.First();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDepartment)));
                }

            };
        }

        public ICollection<dep> Departments { get; set; } = new List<dep>();
        public string DbFilename { get; set; } = Settings.Default.sqliteConnection;
        
        public dep? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment= value;
                Settings.Default.LastSelectedDep = _selectedDepartment.keyid;
                DbFilename = $"dep{_selectedDepartment.keyid}.db";
                Settings.Default.sqliteConnection = DbFilename;
                Settings.Default.Save();

                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(SelectedDepartment)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DbFilename)));
            }
        }

        private void ConnectAndRegisterUser(object sender, RoutedEventArgs e)
        {
            // проверка существования файла БД и его создание в случае отсутствия
            using(CastorContext context = new CastorContext())
            {
                context.Database.EnsureCreated();
            }
            Close();
        }

    }
}
