using CustomGameDB.Models;
using CustomGameDB.trailers;
using FlyleafLib;
using FlyleafLib.MediaPlayer;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using Microsoft.Web.WebView2.Core;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using GameBaseDeDatos = CustomGameDB.Models.Game;
using genero = CustomGameDB.Models.Genre;
using tiendaBaseDatos = CustomGameDB.Models.Store;


namespace CustomGameDB
{
    public partial class MainWindow : Window
    {

        private DispatcherTimer _timerOcultar;
        List<genres> datosGeneros = new List<genres>();
        List<platform> datosPlataformas = new List<platform>();
        List<tiendaBaseDatos> datosStores = new List<tiendaBaseDatos>();
        List<ValorBuscar> tiendas;
        List<ValorBuscar> generos;
        List<ValorBuscar> plataformas;
        public MainWindow()
        {

            InitializeComponent();
            WindowState = WindowState.Maximized;
            itemsControlJuegos.ItemsSource = games;



            using (var db = new NeondbContext())
            {
                tiendas = db.Stores.Select(s => new ValorBuscar { Id = s.Idstore, Valor = s.Valuestore }).ToList();
                generos = db.Genres.Select(g => new ValorBuscar { Id = g.IdGenres, Valor = g.ValueGenres }).ToList();
                plataformas = db.Plataforms.Select(p => new ValorBuscar { Id = p.Id, Valor = p.Name }).ToList();
                tiendas.Insert(0, new ValorBuscar { Id = 0, Valor = "Todas" });
                generos.Insert(0, new ValorBuscar { Id = 0, Valor = "Todos" });
                plataformas.Insert(0, new ValorBuscar { Id = 0, Valor = "Todas" });
                cbTiendas2.ItemsSource = tiendas;
                cbTiendas2.DisplayMemberPath = "Valor";
                cbTiendas2.SelectedValuePath = "Id";
                cbTiendas2.SelectedIndex = 0;
                cbGeneros2.ItemsSource = generos;
                cbGeneros2.DisplayMemberPath = "Valor";
                cbGeneros2.SelectedValuePath = "Id";
                cbGeneros2.SelectedIndex = 0;
                cbPlataformas2.ItemsSource = plataformas;
                cbPlataformas2.DisplayMemberPath = "Valor";
                cbPlataformas2.SelectedValuePath = "Id";
                cbPlataformas2.SelectedIndex = 0;
            }
        }
        GameResults gameResults;
        gameTrailers gameTrailers;
        int page_size = 20;
        String search = "";
        int page = 1;
        int totalPaginas = 0;
        ObservableCollection<Game> games = new ObservableCollection<Game>();

        private void BtBuscarClick(object sender, RoutedEventArgs e)
        {


        }

        private void BtBuscar2_Click(object sender, RoutedEventArgs e)
        {

            games.Clear();
            int page = 1;
            search = tbBuscar2.Text;
            var itemTienda = cbTiendas2.SelectedItem as ValorBuscar;
            int idTienda = itemTienda.Id;

            var itemPlataforma = cbPlataformas2.SelectedItem as ValorBuscar;
            int idPlataforma = itemPlataforma.Id;
            MessageBox.Show($"{itemPlataforma.Id}");
            var itemGenero = cbGeneros2.SelectedItem as ValorBuscar;
            int idGenero = itemGenero.Id;
            MessageBox.Show($"{idTienda}");
            cargarJuegos(page_size, page, search, idTienda, idPlataforma, idGenero);
            itemsControlJuegos.Visibility = Visibility.Visible;
            itemsControlJuegos.IsEnabled = true;

        }

        private async void cargarJuegos(int page_size, int page, String search, int idTienda, int idGenero, int idPlataforma)
        {
            gameResults = await new peticionGames().GetGames(page_size, page, search, idTienda, idGenero, idPlataforma);
            totalPaginas = (int)Math.Ceiling((double)gameResults.count / page_size);
            foreach (Game game in gameResults?.results!)
            {
                games.Add(game);


            }


            _cargando = false;


        }

        private bool _cargando = false;

        private async void Games_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange == 0) return;

            var scrollViewer = (ScrollViewer)e.OriginalSource;
            double posicionActual = e.VerticalOffset + e.ViewportHeight;
            double umbralCarga = e.ExtentHeight * 0.8;
            double unbralVaciarDatos = e.ExtentHeight * 0.2;
            if (posicionActual >= umbralCarga && !_cargando)
            {
                page++;
                _cargando = true;
                var itemTienda = cbTiendas2.SelectedItem as ValorBuscar;
                int idTienda = itemTienda.Id;
                var itemPlataforma = cbPlataformas2.SelectedItem as ValorBuscar;
                int idPlataforma = itemPlataforma.Id;
                var itemGenero = cbGeneros2.SelectedItem as ValorBuscar;
                int idGenero = itemGenero.Id;
                cargarJuegos(page_size, page, search, idTienda, idPlataforma, idGenero);



            }
            else
            {
                if (posicionActual <= unbralVaciarDatos && !_cargando)
                {
                    page--;
                    _cargando = true;
                    if (page < 1) page = 1;
                    if (page != 1)
                    {
                        cargarJuegos(page_size, page, search, (int)cbTiendas2.SelectedValue, (int)cbGeneros2.SelectedValue, (int)cbPlataformas2.SelectedValue);
                    }
                }

            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            peticionGames peticion = new peticionGames();
            gameResults = new GameResults();

            cargarJuegos(page_size, page, search, 0, 0, 0);


        }

        private void btanterior_Click(object sender, RoutedEventArgs e)
        {

            tabControlJuegos.Visibility = Visibility.Visible;
            dgDatosJuegos.Visibility = Visibility.Visible;
            dgDatosJuegos.IsEnabled = true;
            gridDatosJuegos.Visibility = Visibility.Hidden;
            gridDatosJuegos.IsEnabled = false;
            cbTrailers.ItemsSource = null;
            videoControl.Source = new Uri("about:blank");


        }

        private void btanterior2_Click(object sender, RoutedEventArgs e)
        {


            dgDatosJuegos.Visibility = Visibility.Visible;
            dgDatosJuegos.IsEnabled = true;
            gridDatosJuegos.Visibility = Visibility.Hidden;
            gridDatosJuegos.IsEnabled = false;
            videoControl.Source = new Uri("about:blank");
            MessageBox.Show($" valor cbtiendas: {cbTiendas2.SelectedValue} {cbTiendas2.SelectedItem} {cbTiendas2.SelectedValuePath}");
        }




        private void tbBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtBuscarClick(sender, e);
            }



        }

        private void tbBuscar2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtBuscar2_Click(sender, e);
            }
        }

        int idJuegoSeleccionado = 0;
        DateOnly fechaLanzamiento = new DateOnly();
        private async void Tarjeta_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var elemento = sender as FrameworkElement;
            var juegoSeleccionado = elemento?.DataContext as Game;

            if (juegoSeleccionado != null)
            {
                tabControlJuegos.Visibility = Visibility.Collapsed;
                idJuegoSeleccionado = juegoSeleccionado.id;
                if (juegoSeleccionado is Game selectedGame)
                {
                    gridDatosJuegos.Visibility = Visibility.Visible;
                    gridDatosJuegos.IsEnabled = true;
                    lbName.Text = selectedGame.name;
                    tbFechaLanzamiento.Text = selectedGame.released;
                    fechaLanzamiento = DateOnly.Parse(selectedGame.released);
                    tbMetacritic.Text = selectedGame.metacritic.HasValue ? $"{selectedGame.metacritic.Value}" : " Sin datos";
                    tbHorasJugadas.Text = $"{selectedGame.playtime} horas";
                    String descripcion = await new peticionGames().GetGameDescription(selectedGame.id);

                    string descripcionConEstilos = $@"
                        <html>
                        <head>
                            <style>
                                /* El asterisco (*) y el body aseguran que todas las etiquetas hereden el color */
                                * {{
                                    color: white !important;
                                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                }}
                                body {{
                                    background-color: transparent;
                                    line-height: 1.6;
                                    font-size: 14px;
                                }}
                                /* Estilo opcional para que los enlaces no sean el azul feo de navegador */
                                a {{
                                    color: #BB86FC;
                                    text-decoration: none;
                                }}
                                /* Estilo para que las imágenes no se salgan del cuadro */
                                img {{
                                    max-width: 100%;
                                    height: auto;
                                }}
                            </style>
                        </head>
                        <body>
                            {descripcion}
                        </body>
                        </html>";

                    await webDescripcion.EnsureCoreWebView2Async();
                    webDescripcion.NavigateToString(descripcionConEstilos);


                    tbGeneros.Text = "";
                    int contador = 1;
                    if (selectedGame.genres?.Any() == true)
                    {
                        foreach (genres genre in selectedGame.genres)
                        {
                            tbGeneros.Text += $"{genre.name}";
                            contador++;
                            if (contador <= selectedGame.genres.Count)
                            {
                                tbGeneros.Text += ", ";
                            }
                            datosGeneros.Add(genre);
                        }
                    }
                    else
                    {
                        tbGeneros.Text += "Genero desconocido";
                    }
                    tbGeneros.Text = "";
                    contador = 1;
                    if (selectedGame.stores?.Any() == true)
                    {
                        foreach (stores tiendas in selectedGame.stores)
                        {

                            tbTiendas.Text += $"{tiendas.store.name}";
                            contador++;
                            if (contador <= selectedGame.stores.Count)
                            {
                                tbTiendas.Text += ", ";
                            }

                            tiendaBaseDatos tiendaBD = new tiendaBaseDatos
                            {
                                Idstore = tiendas.store.id,
                                Valuestore = tiendas.store.name
                            };
                            datosStores.Add(tiendaBD);


                        }
                    }
                    else
                    {
                        tbTiendas.Text += "No disponible en tiendas";
                    }


                    contador = 1;
                    tbPlataformas.Text = "";
                    if (selectedGame?.platforms!.Any() == true)
                    {
                        foreach (platforms plataforma in selectedGame?.platforms!)
                        {

                            tbPlataformas.Text += $"{plataforma?.platform!.name}";
                            contador++;
                            if (contador <= selectedGame?.platforms!.Count)
                            {
                                tbPlataformas.Text += ", ";
                            }
                            datosPlataformas.Add(plataforma!.platform!);
                        }
                    }
                    else
                    {
                        tbPlataformas.Text += "Plataforma desconocida";
                    }
                    contador = 1;
                    if (selectedGame?.tags?.Any() == true)
                    {
                        foreach (tags etiquetas in selectedGame.tags)
                        {
                            contador++;
                            if (contador <= selectedGame.tags.Count)
                            {
                                //lbEtiquetas.Text += ", ";
                            }

                        }
                    }

                    var trailersDelJuego = await new peticionGames().getTrailer(selectedGame.id);
                    if (trailersDelJuego?.results?.Any() == true)
                    {
                        var trailer = trailersDelJuego.results.FirstOrDefault();
                        cbTrailers.ItemsSource = trailersDelJuego.results;
                        List<ValorTrailer> trailers = new List<ValorTrailer>();
                        foreach (var item in trailersDelJuego.results)
                        {

                            trailers.Add(new ValorTrailer { Titulo = item.name, Url = item.data?.max ?? item.data?._480 });


                        }
                        cbTrailers.ItemsSource = trailers;
                        cbTrailers.DisplayMemberPath = "Titulo";
                        cbTrailers.SelectedValuePath = "Url";


                        if (trailer?.data?.max != null)
                        {
                            bordeTrailer.Visibility = Visibility.Visible;
                            videoControl.Source = new Uri(trailer.data.max);
                            bordeImagenFondo.Visibility = Visibility.Collapsed;
                            tbTrailer.Text = "Trailer";
                            cbTrailers.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (trailer?.data?._480 != null)
                            {
                                MessageBox.Show($"{trailer.data._480}");
                                bordeTrailer.Visibility = Visibility.Visible;
                                videoControl.Source = new Uri(trailer.data._480);
                                bordeImagenFondo.Visibility = Visibility.Collapsed;
                                tbTrailer.Text = "Trailer";
                                cbTrailers.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                    {
                        bordeTrailer.Visibility = Visibility.Collapsed;
                        bordeImagenFondo.Visibility = Visibility.Visible;
                        imagenFondo.Source = new BitmapImage(new Uri(selectedGame.background_image));
                        tbTrailer.Text = "Imagen principal";
                        cbTrailers.Visibility = Visibility.Hidden;
                    }






                    capturas.ItemsSource = selectedGame?.short_screenshots;
                    dgDatosJuegos.Visibility = Visibility.Hidden;
                    dgDatosJuegos.IsEnabled = false;
                    gridDatosJuegos.IsEnabled = true;
                    gridDatosJuegos.Visibility = Visibility.Visible;

                }

            }
        }

        private void cbTrailers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTrailers.Items.Count > 0)
            {
                videoControl.Source = new Uri(cbTrailers.SelectedValue.ToString());
            }

        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            List<GameBaseDeDatos> juegosGuardados = new List<GameBaseDeDatos>();
            using (var db = new NeondbContext())
            {
                if (!db.Games.Any(g => g.Id == idJuegoSeleccionado))
                {
                    MessageBox.Show($"Guardando juego con ID {idJuegoSeleccionado} en la base de datos...");
                    GameBaseDeDatos juegoNuevo = new GameBaseDeDatos
                    {
                        Id = idJuegoSeleccionado,
                        Name = lbName.Text,
                        Released = fechaLanzamiento,
                        Metacritic = int.TryParse(tbMetacritic.Text, out int metacritic) ? (int?)metacritic : null,
                        Playtime = int.TryParse(tbHorasJugadas.Text, out int playtime) ? (int?)playtime : null,
                    };
                    db.Games.Add(juegoNuevo);
                    db.SaveChanges();
                    foreach (genres generoDto in datosGeneros)
                    {
                        var generoDb = db.Genres.Local.FirstOrDefault(g => g.IdGenres == generoDto.id)
                                       ?? db.Genres.Include(g => g.IdGames)
                                                   .FirstOrDefault(g => g.IdGenres == generoDto.id);

                        if (generoDb == null)
                        {
                            generoDb = new genero
                            {
                                IdGenres = generoDto.id,
                                ValueGenres = generoDto.name,
                                IdGames = new List<GameBaseDeDatos>()
                            };
                            db.Genres.Add(generoDb);
                        }

                        if (generoDb.IdGames == null) generoDb.IdGames = new List<GameBaseDeDatos>();

                        if (!generoDb.IdGames.Any(g => g.Id == juegoNuevo.Id))
                        {
                            generoDb.IdGames.Add(juegoNuevo);
                        }
                    }
                    db.SaveChanges();
                    db.SaveChanges();
                    foreach (var plataformaDto in datosPlataformas.GroupBy(p => p.id).Select(g => g.First()))
                    {
                        var platDb = db.Plataforms.Local.FirstOrDefault(p => p.Id == plataformaDto.id)
                                     ?? db.Plataforms.FirstOrDefault(p => p.Id == plataformaDto.id);

                        if (platDb == null)
                        {
                            platDb = new Plataform { Id = plataformaDto.id, Name = plataformaDto.name };
                            db.Plataforms.Add(platDb);
                        }
                        bool existeEnDb = db.Plataforms1.Any(p1 => p1.Idgame == juegoNuevo.Id && p1.Idplataform == platDb.Id);
                        bool existeEnLocal = db.Plataforms1.Local.Any(p1 => p1.Idgame == juegoNuevo.Id && p1.Idplataform == platDb.Id);

                        if (!existeEnDb && !existeEnLocal)
                        {
                            Plataform1 nuevaRelacion = new Plataform1
                            {
                                Idgame = juegoNuevo.Id,
                                Idplataform = platDb.Id
                            };
                            db.Plataforms1.Add(nuevaRelacion);
                        }
                    }
                    db.SaveChanges();

                    foreach (tiendaBaseDatos tiendaDto in datosStores)
                    {
                        var tiendaDb = db.Stores.Include(s => s.Idgames)
                                                .FirstOrDefault(s => s.Idstore == tiendaDto.Idstore);
                        if (tiendaDb == null)
                        {
                            tiendaDb = new tiendaBaseDatos
                            {
                                Idstore = tiendaDto.Idstore,
                                Valuestore = tiendaDto.Valuestore,
                                Idgames = new List<GameBaseDeDatos>()
                            };
                            db.Stores.Add(tiendaDb);
                        }
                        if (tiendaDb.Idgames == null) tiendaDb.Idgames = new List<GameBaseDeDatos>();
                        if (!tiendaDb.Idgames.Any(g => g.Id == juegoNuevo.Id))
                        {
                            tiendaDb.Idgames.Add(juegoNuevo);
                        }
                    }
                    db.SaveChanges();


                }



                int idUsuarioActual = ((App)Application.Current).usuarioLogeado.Idusuario;

                ReviewsUsuario reviewPersonal = new ReviewsUsuario
                {
                    IdGame = idJuegoSeleccionado,
                    Esfavorito = chbFavorito.IsChecked,
                    NotaPersonal = decimal.Parse(tbSliderNota.Text) / 10,
                    IdUsuario = ((App)Application.Current).usuarioLogeado.Idusuario,
                    ReviewTexto = tbReview.Text,
                    Estadojuego = cbEstado.Text,
                    FechaUltimaModificacion = DateTime.Now,
                    HorasJugadas = int.Parse(tbHorasJugadasPersonal.Text != "" ? tbHorasJugadasPersonal.Text : "0"),
                    rutaJuego = tbSeleccionarRuta.Text != "" ? tbSeleccionarRuta.Text : null,
                    IdGameNavigation = db.Games.FirstOrDefault(g => g.Id == idJuegoSeleccionado),
                    IdUsuarioNavigation = db.Usuarios.FirstOrDefault(u => u.Idusuario == idUsuarioActual)

                };


                var registro = db.ReviewsUsuarios.Find(idUsuarioActual, idJuegoSeleccionado);
                var usuarioEnContexto = ((App)Application.Current).usuarioLogeado;
                var juegoEnContexto = db.Games.FirstOrDefault(g => g.Id == idJuegoSeleccionado);
                MessageBox.Show($"valor combo: {cbEstado.SelectedValue}, text: {cbEstado.Text}");
                if (registro != null)
                {
                    MessageBox.Show("Actualizar");
                    registro.Esfavorito = chbFavorito.IsChecked;
                    registro.NotaPersonal = decimal.Parse(tbSliderNota.Text) / 10;
                    registro.ReviewTexto = tbReview.Text;
                    registro.Estadojuego = cbEstado.Text;
                    registro.FechaUltimaModificacion = DateTime.Now;
                    registro.HorasJugadas = int.Parse(tbHorasJugadasPersonal.Text != "" ? tbHorasJugadasPersonal.Text : "0");
                    registro.rutaJuego = tbSeleccionarRuta.Text != "" ? tbSeleccionarRuta.Text : null;
                    registro.IdGameNavigation = db.Games.FirstOrDefault(g => g.Id == idJuegoSeleccionado);
                    registro.IdUsuarioNavigation = db.Usuarios.FirstOrDefault(u => u.Idusuario == idUsuarioActual);
                }
                else
                {
                    MessageBox.Show("añadir");
                    reviewPersonal.IdUsuario = idUsuarioActual;
                    db.ReviewsUsuarios.Add(reviewPersonal);
                }

                db.SaveChanges();
            }
        }



        private void tbHorasJugadasPersonal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }


        }

        private void tabControlJuegos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                if (tabControl.SelectedItem is TabItem pestañaSeleccionada)
                {
                    string nombrePestaña = pestañaSeleccionada.Header.ToString();
                    switch (nombrePestaña)
                    {
                        case "INICIO":
                            configuracionUsuario.Visibility = Visibility.Collapsed;
                            gridDatosJuegos.Visibility = Visibility.Collapsed;
                            dgDatosJuegos.Visibility = Visibility.Visible;
                            amigosControl.Visibility = Visibility.Collapsed;
                            break;

                        case "MI PERFIL":

                            configuracionUsuario.Visibility = Visibility.Visible;
                            gridDatosJuegos.Visibility = Visibility.Collapsed;
                            dgDatosJuegos.Visibility = Visibility.Collapsed;
                            amigosControl.Visibility = Visibility.Collapsed;
                            configuracionUsuario.CargarReviews();
                            break;

                        case "AMIGOS":
                            configuracionUsuario.Visibility = Visibility.Collapsed;
                            gridDatosJuegos.Visibility = Visibility.Collapsed;
                            dgDatosJuegos.Visibility = Visibility.Collapsed;
                            amigosControl.Visibility = Visibility.Visible;
                            amigosControl.CargarAmigos();
                            break;
                    }
                }
            }



        }

        private void btnRuta_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                tbSeleccionarRuta.Text = openFileDialog.FileName;
            }
        }
    }
}