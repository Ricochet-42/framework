using Framework1.conexion_dba;
using Framework1.principal;
using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Framework1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CargarUsuarios(); // 🚀 Llenamos el ComboBox al iniciar
        }

        private void CargarUsuarios()
        {
            try
            {
                Globales.Sqlite_Conex.Open();

                // 👀 Aquí usamos la misma columna que en usuarios_edit
                string query = "SELECT NOMBRE FROM USUARIOS ORDER BY ID;";
                SQLiteCommand cmd = new SQLiteCommand(query, Globales.Sqlite_Conex);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmb_usuarios.Items.Add(reader["NOMBRE"].ToString());
                }

                // >>> Corrección: cerrar reader y conexión
                reader.Close();
                Globales.Sqlite_Conex.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }

        private void cmb_usuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_usuarios.SelectedItem != null)
            {
                Globales.Nombre_Usuario = cmb_usuarios.SelectedItem.ToString();
            }
        }

        private void btn_iniciar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // justo al inicio del try, antes de armar el SQL
                string claveIngresada = (txt_contraVisible.Visibility == Visibility.Visible)
                                        ? txt_contraVisible.Text
                                        : txt_contra2.Password;


                // >>> Corrección: abrir la conexión antes de usarla
                Globales.Sqlite_Conex.Open();

                // >>> Corrección: no usar ExecuteNonQuery() en SELECT y usar parámetros
                //string Sql_login = "SELECT * FROM USUARIOS WHERE Nombre = @nombre AND CLAVE = @clave;";
                //SQLiteCommand Cmd_Login = new SQLiteCommand(Sql_login, Globales.Sqlite_Conex);
                //Cmd_Login.Parameters.AddWithValue("@nombre", cmb_usuarios.Text);
                //Cmd_Login.Parameters.AddWithValue("@clave", txt_contra2.Password);


                string Sql_login = "SELECT * FROM USUARIOS WHERE Nombre = @nombre AND CLAVE = @clave;";
                SQLiteCommand Cmd_Login = new SQLiteCommand(Sql_login, Globales.Sqlite_Conex);
                Cmd_Login.Parameters.AddWithValue("@nombre", cmb_usuarios.Text);
                Cmd_Login.Parameters.AddWithValue("@clave", claveIngresada);


                SQLiteDataReader reader = Cmd_Login.ExecuteReader();

                int contador = 0;
                while (reader.Read())
                {
                    contador++;
                }

                reader.Close();

                if (contador == 1)
                {
                    // 🚀 Validamos usuario y contraseña desde la BD

                    string query = "SELECT * FROM USUARIOS WHERE NOMBRE=@nombre AND CLAVE=@clave";
                    SQLiteCommand cmd = new SQLiteCommand(query, Globales.Sqlite_Conex);
                    // Si no hay SelectedItem, usamos Text
                    cmd.Parameters.AddWithValue("@nombre", cmb_usuarios.SelectedItem?.ToString() ?? cmb_usuarios.Text);
                    cmd.Parameters.AddWithValue("@clave", txt_contra2.Password);

                    SQLiteDataReader reader1 = cmd.ExecuteReader();

                    // >>> Corrección: validar con reader1 (no con reader)
                    if (reader1.HasRows)
                    {
                        reader1.Close(); // cerramos antes de cambiar de ventana
                        Globales.Sqlite_Conex.Close();

                        form_principal Form_P = new form_principal();
                        Form_P.Show();
                        Close();
                    }
                    else
                    {
                        reader1.Close();
                        Globales.Sqlite_Conex.Close();

                        MessageBox.Show("Usuario o contraseña incorrectos", "Error 404", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        txt_contra2.Clear();
                        txt_contra2.Focus();
                    }
                }
                else
                {
                    Globales.Sqlite_Conex.Close();

                    MessageBox.Show("error 404 login: " + MessageBoxButton.OK + MessageBoxImage.Exclamation);
                    txt_contra2.Clear();
                    txt_contra2.Focus();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("error 404 login: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                try { if (Globales.Sqlite_Conex.State == System.Data.ConnectionState.Open) Globales.Sqlite_Conex.Close(); } catch { }
            }
        }

        private void Label_Titulo_MouseEnter(object sender, MouseEventArgs e)
        {
            Label_Titulo.FontSize = 30;
            Label_Titulo.Foreground = new SolidColorBrush(Colors.DarkBlue);
        }

        private void Label_Titulo_MouseLeave(object sender, MouseEventArgs e)
        {
            Label_Titulo.FontSize = 20;
            Label_Titulo.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            txt_contraVisible.Text = txt_contra2.Password;
            txt_contra2.Visibility = Visibility.Collapsed;
            txt_contraVisible.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_contra2.Password = txt_contraVisible.Text;
            txt_contraVisible.Visibility = Visibility.Collapsed;
            txt_contra2.Visibility = Visibility.Visible;
        }
    }
}
