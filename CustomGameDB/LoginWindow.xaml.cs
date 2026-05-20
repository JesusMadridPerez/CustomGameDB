using CustomGameDB.Models;
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

namespace CustomGameDB
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btIniciarSesioon_Click(object sender, RoutedEventArgs e)
        {
            lbContrasenaIniciarSesion.Visibility = Visibility.Visible;
            lbIniciarSesionUsuario.Visibility = Visibility.Visible;
            tbUsuarioIniciarSesion.Visibility = Visibility.Visible;
            tbContrasenaInciarSesion.Visibility = Visibility.Visible;
            btAceptarIniciarSesion.Visibility = Visibility.Visible;
            lbAnyoNacimientoCrearNuevaCuenta.Visibility = Visibility.Hidden;
            lbContrasenaCrearNuevaCuenta.Visibility = Visibility.Hidden;
            lbUsuarioCrearNuevaCuenta.Visibility = Visibility.Hidden;
            lbEmailCrearNuevaCuenta.Visibility = Visibility.Hidden;
            lbAnyoNacimientoCrearNuevaCuenta.Visibility = Visibility.Hidden;
            tbUsuarioCrearNuevaCuenta.Visibility = Visibility.Hidden;
            tbContrasenaCrearNuevaCuenta.Visibility = Visibility.Hidden;
            tbEmailCrearNuevaCuenta.Visibility = Visibility.Hidden;
            dpAnyoNacimiento.Visibility = Visibility.Hidden;
            btAceptarCrearNuevaCuenta.Visibility = Visibility.Hidden;



        }

        private void btCrearNuevaCuenta_Click(object sender, RoutedEventArgs e)
        {
            lbContrasenaIniciarSesion.Visibility = Visibility.Hidden;
            lbIniciarSesionUsuario.Visibility = Visibility.Hidden;
            tbUsuarioIniciarSesion.Visibility = Visibility.Hidden;
            tbContrasenaInciarSesion.Visibility = Visibility.Hidden;
            btAceptarIniciarSesion.Visibility = Visibility.Hidden;
            lbAnyoNacimientoCrearNuevaCuenta.Visibility = Visibility.Visible;
            lbContrasenaCrearNuevaCuenta.Visibility = Visibility.Visible;
            lbUsuarioCrearNuevaCuenta.Visibility = Visibility.Visible;
            lbEmailCrearNuevaCuenta.Visibility = Visibility.Visible;
            lbAnyoNacimientoCrearNuevaCuenta.Visibility = Visibility.Visible;
            tbUsuarioCrearNuevaCuenta.Visibility = Visibility.Visible;
            tbContrasenaCrearNuevaCuenta.Visibility = Visibility.Visible;
            tbEmailCrearNuevaCuenta.Visibility = Visibility.Visible;
            dpAnyoNacimiento.Visibility = Visibility.Visible;
            btAceptarCrearNuevaCuenta.Visibility = Visibility.Visible;
        }

        private void btAceptarIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbUsuarioIniciarSesion.Text) || string.IsNullOrEmpty(tbContrasenaInciarSesion.Text))
            {
                MessageBox.Show("Rellena todos los campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new NeondbContext())
            {
                string nombreInput = tbUsuarioIniciarSesion.Text;
                string passwordInput = tbContrasenaInciarSesion.Text;
                var usuariosEnMemoria = db.Usuarios.ToList();
                var usuario = usuariosEnMemoria
                                .FirstOrDefault(u => u.Username == nombreInput);

                if (usuario != null && usuario.UserPassword == passwordInput)
                {
                    MessageBox.Show($"{usuario.Username}");
                    ((App)Application.Current).usuarioLogeado = usuario;
                    MessageBox.Show($"{((App)Application.Current).usuarioLogeado.Username}");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void btAceptarCrearNuevaCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsuarioCrearNuevaCuenta.Text) ||
                string.IsNullOrWhiteSpace(tbContrasenaCrearNuevaCuenta.Text) ||
                dpAnyoNacimiento.SelectedDate == null ||
                string.IsNullOrWhiteSpace(tbEmailCrearNuevaCuenta.Text))
            {
                MessageBox.Show("Por favor, rellena todos los campos correctamente.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new NeondbContext())
                {
                    bool existe = db.Usuarios.Any(u => u.Username == tbUsuarioCrearNuevaCuenta.Text);

                    if (existe)
                    {
                        MessageBox.Show("El nombre de usuario ya existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    int? maxId = db.Usuarios.Max(u => (int?)u.Idusuario);
                    int proximoId = (maxId ?? 0) + 1;
                    Usuario nuevoUsuario = new Usuario
                    {
                        Idusuario = proximoId,
                        Username = tbUsuarioCrearNuevaCuenta.Text,
                        UserPassword = tbContrasenaCrearNuevaCuenta.Text,
                        Email = tbEmailCrearNuevaCuenta.Text,
                        Anyonacimiento = DateOnly.FromDateTime(dpAnyoNacimiento.SelectedDate.Value)
                    };
                    db.Usuarios.Add(nuevoUsuario);
                    db.SaveChanges(); // Aquí es donde Neon recibe los datos
                    MessageBox.Show("Cuenta creada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en la base de datos: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
