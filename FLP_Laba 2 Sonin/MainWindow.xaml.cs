using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FLP_Laba_2_Sonin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string DB_PATH = "F:\\ВУЗ\\Функциональное и логическое программирование\\FLP_Laba 2 Sonin\\FLP_Laba 2 Sonin\\DB_flp2.db;";
        private short currentFunction = 0;

        public MainWindow()
        {
            InitializeComponent();

        }

        private List<DB_reply> DB_request(string sqlExpression)
        {
            List<DB_reply> result = new List<DB_reply>(); // Список со строками из БД 
            DB_reply reply = new DB_reply();
            using (var connection = new SqliteConnection("Data Source=" + DB_PATH + "Mode=ReadWrite;")) // Создаю строку подключения к БД
            {
                connection.Open(); // Открываю БД

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader()) // Делаю SQL запрос
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            reply.id = reader.GetInt32(0);
                            reply.One = reader.GetValue(1).ToString();
                            // Иногда в таблице только 1 колонка помимо id, на это я и проверяю
                            if (reader.FieldCount > 2)
                            reply.Two = reader.GetValue(2).ToString();
                            result.Add(reply);
                            reply = new DB_reply();
                        }
                    } else
                    {
                        return null;
                    }
                }
            }
            return result;
        }
        private void router_Click(object sender, RoutedEventArgs e) // Подсчитать число групп определённой специальности
        {
            Button btn = e.OriginalSource as Button;
            string sqlExpression;
            secondList.Visibility = Visibility.Hidden;
            switch (btn.Name)
            {
                case "numberOfGroups":
                    sqlExpression = "SELECT DISTINCT Groups.id, Specialty_code, Facult FROM Groups";
                    allList.ItemsSource = DB_request(sqlExpression);
                    sendMessage("Выберите код специальности");
                    currentFunction = 1;
                    break;
                case "studentGrade":
                    sqlExpression = "SELECT Students.id, Surname, Name FROM Students";
                    allList.ItemsSource = DB_request(sqlExpression);
                    sendMessage("Выберите студента и предмет");
                    currentFunction = 2;
                    sqlExpression = "SELECT * FROM Subjects";
                    secondList.Visibility = Visibility.Visible;
                    secondList.ItemsSource = DB_request(sqlExpression);
                    break;
            }
        }
        private void bestGroup_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Tuneyadsi_Click(object sender, RoutedEventArgs e) // Список должников
        {
            secondList.Visibility = Visibility.Hidden;
            string sqlExpression = @"SELECT DISTINCT Students.id, Surname, Name FROM Students
                                    JOIN Summary_statement on Students.id = Summary_statement.Student
                                    WHERE Summary_statement.Grade < 25";
            allList.ItemsSource = DB_request(sqlExpression);
            currentFunction = 4;
        }
        private void Army_Click(object sender, RoutedEventArgs e) // Кто учится на военной кафедре
        {
            secondList.Visibility = Visibility.Hidden;
            string sqlExpression = "SELECT Students.id, Surname, Name FROM Students WHERE Military = '1'";
            allList.ItemsSource = DB_request(sqlExpression);
            currentFunction = 5;
        }

        private void allList_SelectionChanged(object sender, SelectionChangedEventArgs e) // при нажатии на элементы ListBox
        {
            //textBox.Text = allList.SelectedIndex.ToString();
            string sqlExpression;
            switch (currentFunction)
            {
                case 1:
                    String item = allList.SelectedItem.ToString(); // Получаю выбранную строку
                    var strings = item.Split(' '); // Разделяю содержимое на 2 строки
                    //MessageBox.Show(allList.SelectedItem.ToString());
                    sqlExpression = "SELECT Groups.id, Specialty_code, Group_number FROM Groups WHERE Specialty_code = '" + strings[0] + "'"; // Помещаю первую строку с кодом специальности в запрос
                    // Изменяю текущий номер на "1" + "1", чтобы после обновления ListBox данный кусок кода заново не прошел и чтобы слекдующим шагом вывести количество групп
                    currentFunction = 11;
                    allList.ItemsSource = DB_request(sqlExpression);
                    break;
                case 11:
                    sendMessage("Число групп: " + allList.Items.Count.ToString());
                    break;
                case 2:
                    if (allList.SelectedItem != null && secondList.SelectedItem != null)
                    {
                        DB_reply student = allList.SelectedItem as DB_reply;
                        DB_reply subject = secondList.SelectedItem as DB_reply;
                        //MessageBox.Show(reply.id.ToString());
                        sqlExpression = "SELECT Summary_statement.id, Grade FROM Summary_statement WHERE Student = '" + student.id + "' AND Subject = '" + subject.id + "'";
                        currentFunction = 2;
                        if (DB_request(sqlExpression) == null)
                            sendMessage("Нет оценки");
                        else
                            sendMessage("Оценка : " + DB_request(sqlExpression)[0].One);
                    }
                    break;
            }
        }
        private void sendMessage(string message)
        {
            textBox.Text = message;
        }
    }
}
