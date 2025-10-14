using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Framework1.usuarios
{
    public partial class EditAlumnoDialog : Window
    {
        private readonly SQLiteConnection _con;
        private readonly int _expActual;

        public EditAlumnoDialog(SQLiteConnection con,
                                int expediente, string nombre, int semestre,
                                string correo, int edad, string genero)
        {
            InitializeComponent();
            _con = con;
            _expActual = expediente;

            // Precarga de campos
            txtExpediente.Text = expediente.ToString();
            txtNombre.Text = nombre ?? string.Empty;
            txtSemestre.Text = semestre.ToString();
            txtCorreo.Text = correo ?? string.Empty;
            txtEdad.Text = edad.ToString();

            // Cargar género (simple: set SelectedItem por texto si coincide)
            cmbGenero.SelectedItem = null;
            foreach (var item in cmbGenero.Items)
            {
                if ((item as ComboBoxItem)?.Content?.ToString()?.Equals(genero, StringComparison.OrdinalIgnoreCase) == true)
                {
                    cmbGenero.SelectedItem = item;
                    break;
                }
            }
            if (cmbGenero.SelectedItem == null && !string.IsNullOrWhiteSpace(genero))
            {
                // Si viene otro valor, agréguelo dinámicamente
                cmbGenero.Items.Add(genero);
                cmbGenero.SelectedItem = genero;
            }
        }

        // Verifica duplicado por Nombre o Correo, excluyendo el propio expediente
        private bool ExisteDuplicadoNombreOCorreo(string nombre, string correo)
        {
            const string sql = @"
                SELECT COUNT(*)
                  FROM ALUMNOS
                 WHERE (UPPER(NOMBRE)=UPPER(@n) OR UPPER(CORREO)=UPPER(@c))
                   AND EXPEDIENTE<>@exp;";
            using (var cmd = new SQLiteCommand(sql, _con))
            {
                cmd.Parameters.AddWithValue("@n", nombre?.Trim() ?? "");
                cmd.Parameters.AddWithValue("@c", correo?.Trim() ?? "");
                cmd.Parameters.AddWithValue("@exp", _expActual);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones mínimas
                if (!int.TryParse(txtSemestre.Text.Trim(), out int semestre) || semestre < 1)
                {
                    MessageBox.Show("Semestre inválido (ingrese un entero >= 1).", "Valida",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtSemestre.Focus(); return;
                }
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre no puede estar vacío.", "Valida",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNombre.Focus(); return;
                }
                if (string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                    !Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Correo inválido.", "Valida",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCorreo.Focus(); return;
                }
                if (!int.TryParse(txtEdad.Text.Trim(), out int edad) || edad < 0)
                {
                    MessageBox.Show("Edad inválida (entero >= 0).", "Valida",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEdad.Focus(); return;
                }
                string genero = (cmbGenero.SelectedItem as ComboBoxItem)?.Content?.ToString()
                                ?? cmbGenero.SelectedItem?.ToString()
                                ?? "";

                // Duplicado por Nombre o Correo (excluye el expediente actual)
                if (ExisteDuplicadoNombreOCorreo(txtNombre.Text, txtCorreo.Text))
                {
                    MessageBox.Show("Ya existe un alumno con ese Nombre o Correo.", "Duplicado",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                const string sqlUpdate = @"
                    UPDATE ALUMNOS
                       SET NOMBRE   = @n,
                           SEMESTRE = @s,
                           CORREO   = @c,
                           EDAD     = @e,
                           GENERO   = @g
                     WHERE EXPEDIENTE = @exp;";
                using (var cmd = new SQLiteCommand(sqlUpdate, _con))
                {
                    cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@s", semestre);
                    cmd.Parameters.AddWithValue("@c", txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@e", edad);
                    cmd.Parameters.AddWithValue("@g", genero.Trim());
                    cmd.Parameters.AddWithValue("@exp", _expActual);

                    int n = cmd.ExecuteNonQuery();
                    if (n == 0)
                    {
                        MessageBox.Show("No se actualizó ningún registro (verifique el expediente).", "Aviso",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                this.DialogResult = true; // cierra modal con OK
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cambios: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
