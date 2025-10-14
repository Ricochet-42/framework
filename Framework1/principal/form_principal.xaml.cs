using Framework1.usuarios;
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

namespace Framework1.principal
{
    /// <summary>
    /// Lógica de interacción para form_principal.xaml
    /// </summary>
    public partial class form_principal : Window
    {
        public form_principal()
        {
            InitializeComponent();
        }

        private void boton_home_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void boton_home_Click(object sender, RoutedEventArgs e)
        {
            usuarios_edit user = new usuarios_edit();
            user.ShowDialog();
        }

        private void Abrir(Window w, RadioButton rb)
        {
            w.Owner = this;   // opcional
            w.Show();         // o ShowDialog() si quieres modal
            rb.IsChecked = false; // vuelve a estado "no seleccionado" para que se vea como botón
        }

        private void boton_alumnos_Click(object sender, RoutedEventArgs e)
            => Abrir(new alumnos_edit(), (RadioButton)sender);

        private void boton_maestros_Click(object sender, RoutedEventArgs e)
            => Abrir(new maestros_edit(), (RadioButton)sender);

        private void boton_materias_Click(object sender, RoutedEventArgs e)
            => Abrir(new materias_edit(), (RadioButton)sender);

        private void boton_usuarios_Click(object sender, RoutedEventArgs e)
            => Abrir(new usuarios_edit(), (RadioButton)sender);
    }
    
    

}



