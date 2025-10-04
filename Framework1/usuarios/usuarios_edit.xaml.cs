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
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data;
using Framework1.conexion_dba;
using System.Security.Cryptography;


namespace Framework1.usuarios
{
    /// <summary>
    /// Lógica de interacción para usuarios_edit.xaml
    /// </summary>
    public partial class usuarios_edit : Window
    {
        public usuarios_edit()
        {
            InitializeComponent();
            Globales.Sqlite_Conex.Open();
            Mostrar_Datos_Grid();
            Cargartipo();
        }



        private void Cargartipo()
        {
            try
            {
                // 👀 Aquí usamos la misma columna que en usuarios_edit
                string query = "SELECT TIPO FROM USUARIOS ORDER BY ID;";
                SQLiteCommand cmd = new SQLiteCommand(query, Globales.Sqlite_Conex);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmb_usuarios_tipo.Items.Add(reader["TIPO"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }



        public void Mostrar_Datos_Grid()
        {
            try
            {
                string Consulta_Sql = "SELECT * FROM USUARIOS ORDER BY ID";
                SQLiteCommand Cmd_Usuarios = new SQLiteCommand(Consulta_Sql, Globales.Sqlite_Conex);
                Cmd_Usuarios.ExecuteNonQuery();

                //conectar y llenar
                SQLiteDataAdapter adaptador = new SQLiteDataAdapter(Cmd_Usuarios);
                DataTable tabla_usuarios = new DataTable("USUARIOS");
                adaptador.Fill(tabla_usuarios);

                //mostrar info

                grid_usuarios.ItemsSource = tabla_usuarios.DefaultView;

                adaptador.Update(tabla_usuarios);
            }

            catch(Exception ex)
            {
                MessageBox.Show("Error 404 " + ex.Message, "Error 404", MessageBoxButton.OK);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Globales.Sqlite_Conex.Close();
        }

        private void agregar_click(object sender, RoutedEventArgs e)
        {
            
        }

        private void grid_usuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid Grid = (DataGrid)sender;
                DataRowView fila = Grid.SelectedItem as DataRowView;

                if(fila != null)
                {
                    Globales.ID_Usuario = Convert.ToInt32(fila["ID"].ToString());
                    Globales.Nombre_Usuario = fila["NOMBRE"].ToString();
                    Globales.Clave_Usuario = fila["CLAVE"].ToString();
                    Globales.Correo_Usuario = fila["CORREO"].ToString();
                    Globales.Tipo_Usuario = fila["TIPO"].ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 " + ex.Message, "Error 404", MessageBoxButton.OK);
            }
        }



        private void cmb_usuarios_tipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_usuarios_tipo.SelectedItem != null)
            {
                Globales.Tipo_Usuario = cmb_usuarios_tipo.SelectedItem.ToString();
            }
        }





        private void eliminar_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_usuarios.SelectedItems.Count==0)
                {
                    MessageBox.Show("Por favor, seleccione un usuario para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("Se eliminara el usuario: " + Globales.ID_Usuario + " y con nombre: " + Globales.Nombre_Usuario + " Deseas continuar?","aviso",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 " + ex.Message, "Error 404", MessageBoxButton.OK);
            }
        }
    }
}
