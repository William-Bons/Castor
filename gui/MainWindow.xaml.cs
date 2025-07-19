using Castor.database;
using Castor.database.tables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Castor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()  
        {
            InitializeComponent();
            Console.Print("wellcommen..+");

            using (CastorSqlServerContext db = new CastorSqlServerContext())
            {
                // создаем два объекта User
                User tom = new User { Name = "Tom", Department = 6 };
                User alice = new User { Name = "Alice", Department = 12 };

                // добавляем их в бд
                db.Users.Add(tom);
                db.Users.Add(alice);
                db.SaveChanges();
                Console.Print("Объекты успешно сохранены");

                // получаем объекты из бд и выводим на консоль
                var users = db.Users.ToList();
                Console.Print("Список объектов:");
                foreach (User u in users)
                {
                    Console.Print($"{u.Id}.{u.Name} - {u.Department}");
                }
            }
        }
    }
}