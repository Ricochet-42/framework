using System;
using System.Data.SQLite;
using System.Windows;

namespace Framework1.usuarios
{
    public partial class EditMaestroDialog : Window
    {
        private readonly SQLiteConnection _con;
        private readonly int _idActual;


        public EditMaestroDialog(SQLiteConnection con, int id, string nombre, string correo, string facultad, string tipo)
        {
            InitializeComponent();
            _con = con;
            _idActual = id;

            // Precarga
            txtId.Text = id.ToString();
            txtNombre.Text = nombre ?? string.Empty;
            txtCorreo.Text = correo ?? string.Empty;
            txtFacultad.Text = facultad ?? string.Empty;

            CargarTipos();
            if (!string.IsNullOrWhiteSpace(tipo))
                cmbTipo.SelectedItem = tipo;
        }

        private void CargarTipos()
        {
            try
            {
                cmbTipo.Items.Clear();
                const string query = "SELECT DISTINCT TIPO FROM MAESTROS WHERE TIPO IS NOT NULL AND TIPO<>'' ORDER BY TIPO;";
                using (var cmd = new SQLiteCommand(query, _con))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        cmbTipo.Items.Add(rdr["TIPO"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Duplicado por Nombre o Correo, excluyendo el propio ID
        private bool ExisteDuplicado(string nombre, string correo)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM MAESTROS
                WHERE (UPPER(NOMBRE)=UPPER(@nombre) OR UPPER(CORREO)=UPPER(@correo))
                  AND ID<>@id;";
            using (var cmd = new SQLiteCommand(sql, _con))
            {
                cmd.Parameters.AddWithValue("@nombre", nombre?.Trim() ?? "");
                cmd.Parameters.AddWithValue("@correo", correo?.Trim() ?? "");
                cmd.Parameters.AddWithValue("@id", _idActual);
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nombre = txtNombre.Text?.Trim();
                var correo = txtCorreo.Text?.Trim();
                var facultad = txtFacultad.Text?.Trim();
                var tipo = (cmbTipo.SelectedItem?.ToString() ?? "").Trim();

                // Validaciones mínimas
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("El nombre no puede estar vacío.", "Valida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNombre.Focus(); return;
                }
                if (string.IsNullOrWhiteSpace(correo))
                {
                    MessageBox.Show("El correo no puede estar vacío.", "Valida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCorreo.Focus(); return;
                }
                if (string.IsNullOrWhiteSpace(facultad))
                {
                    MessageBox.Show("La facultad no puede estar vacía.", "Valida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFacultad.Focus(); return;
                }
                if (string.IsNullOrWhiteSpace(tipo))
                {
                    MessageBox.Show("Seleccione un tipo.", "Valida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbTipo.Focus(); return;
                }

                // Duplicado (Nombre o Correo)
                if (ExisteDuplicado(nombre, correo))
                {
                    MessageBox.Show("Ya existe un maestro con ese Nombre o Correo.", "Duplicado",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                const string sqlUpdate = @"
                    UPDATE MAESTROS
                    SET NOMBRE=@nombre,
                        CORREO=@correo,
                        FACULTAD=@facultad,
                        TIPO=@tipo
                    WHERE ID=@id;";

                using (var cmd = new SQLiteCommand(sqlUpdate, _con))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@facultad", facultad);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@id", _idActual);

                    int n = cmd.ExecuteNonQuery();
                    if (n == 0)
                    {
                        MessageBox.Show("No se actualizó ningún registro (verifique el ID).", "Aviso",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                this.DialogResult = true; // OK
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cambios: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
