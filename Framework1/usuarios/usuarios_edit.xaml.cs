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




        private void agregar_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_id.Text == "")
                {
                    MessageBox.Show("No existe información de ID: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (txt_nombre.Text == "")
                {
                    MessageBox.Show("No existe información en Nombre: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (txt_clave.Text == "")
                {
                    MessageBox.Show("No existe información en clave: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (txt_correo.Text == "")
                {
                    MessageBox.Show("No existe información en correo: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (cmb_usuarios_tipo.Text == "" || cmb_usuarios_tipo.Text == "-- Seleccione el tipo de usuario --")
                {
                    MessageBox.Show("No existe información en tipo: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("¿Esta seguro que deseas agregar al usuario con ID: " + txt_id.Text + ", nombre: " + txt_nombre.Text + "?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        Agregar_Usuario_Nuevo();
                        Mostrar_Datos_Grid();
                        txt_id.Clear();
                        txt_nombre.Clear();
                        txt_correo.Clear();
                        txt_clave.Clear();
                        cmb_usuarios_tipo.Text = "-- Seleccione el tipo de usuario --";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 Agregar Usuario: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Agregar_Usuario_Nuevo()
        {
            try
            {
                string sql_insert = "INSERT INTO USUARIOS (ID, Nombre, Clave, Correo, Tipo)" +
                    "VALUES(@ID, @Nombre, @Clave, @Correo, @Tipo)";
                SQLiteCommand cmd_insert = new SQLiteCommand(sql_insert, Globales.Sqlite_Conex);
                cmd_insert.Parameters.AddWithValue("@ID", txt_id.Text);
                cmd_insert.Parameters.AddWithValue("@Nombre", txt_nombre.Text);
                cmd_insert.Parameters.AddWithValue("@Clave", txt_clave.Text);
                cmd_insert.Parameters.AddWithValue("@Correo", txt_correo.Text);
                cmd_insert.Parameters.AddWithValue("@Tipo", cmb_usuarios_tipo.Text);

                cmd_insert.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 Agregar Usuario: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_usuarios.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Elija al menos un usuario: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("¿Esta seguro que desea eliminar al usuario: " + Globales.Nombre_Usuario + " con ID: " + Globales.ID_Usuario + "?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        string sql = "DELETE FROM Usuarios WHERE ID=" + Globales.ID_Usuario;
                        SQLiteCommand Cmd_delete = new SQLiteCommand(sql, Globales.Sqlite_Conex);
                        Cmd_delete.ExecuteNonQuery();
                        Mostrar_Datos_Grid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 Eliminar Usuario: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void txt_id_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // ✅ Solo permite dígitos 0–9
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void txt_id_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab) e.Handled = false;
        }

        private void txt_id_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_id.Text))
                {
                    // Si el campo está vacío, asignar el siguiente ID disponible
                    txt_id.Text = ObtenerSiguienteIDDisponible().ToString();
                    return;
                }

                int idIngresado = int.Parse(txt_id.Text);

                // Verificar si existe en BD
                string query = "SELECT COUNT(*) FROM USUARIOS WHERE ID=@id";
                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Sqlite_Conex))
                {
                    cmd.Parameters.AddWithValue("@id", idIngresado);
                    object result = cmd.ExecuteScalar();
                    int count = Convert.ToInt32(result);

                    if (count > 0)
                    {
                        MessageBox.Show($"El ID {idIngresado} ya existe en la base de datos.",
                            "ID Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txt_id.Clear();
                        txt_id.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar ID: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obtiene el ID faltante más pequeño disponible (ej. si existen 1,2,4,5 → devuelve 3)
        /// </summary>
        private int ObtenerSiguienteIDDisponible()
        {
            List<int> ids = new List<int>();
            string query = "SELECT ID FROM USUARIOS ORDER BY ID ASC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Sqlite_Conex))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["ID"]));
                }
            }

            int nextId = 1;
            foreach (int id in ids)
            {
                if (id == nextId)
                    nextId++;
                else if (id > nextId)
                    break;
            }
            return nextId;
        }





        //    private void eliminar_click(object sender, RoutedEventArgs e)
        //    {
        //        try
        //        {
        //            if (grid_usuarios.SelectedItems.Count==0)
        //            {
        //                MessageBox.Show("Por favor, seleccione un usuario para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //            else
        //            {
        //                MessageBoxResult result;
        //                result = MessageBox.Show("Se eliminara el usuario: " + Globales.ID_Usuario + " y con nombre: " + Globales.Nombre_Usuario + " Deseas continuar?","aviso",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error 404 " + ex.Message, "Error 404", MessageBoxButton.OK);
        //        }
        //    }
    }
}
