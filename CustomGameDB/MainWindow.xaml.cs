using CustomGameDB.trailers;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
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



namespace CustomGameDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            dgDatosJuegos.ItemsSource = games;
            //Core.Initialize();
        }
        GameResults gameResults;
        //gameTrailers gameTrailers;
        int page_size = 20;
        String search = "";
        int page = 1;

        int totalPaginas = 0;

        //private LibVLC? _libVLC;
        //private System.Windows.Media.MediaPlayer mediaPlayer;


        ObservableCollection<Game> games = new ObservableCollection<Game>();

        private void BtBuscarClick(object sender, RoutedEventArgs e)
        {
            games.Clear();
            int page = 1;
            search = tbBuscar.Text;
            cargarJuegos(page_size, page, search);
            dgDatosJuegos.Visibility = Visibility.Visible;
            dgDatosJuegos.IsEnabled = true;

        }

        private async void cargarJuegos(int page_size , int page , String search) {
            //games.Clear();
            
            gameResults = await new peticionGames().GetGames(page_size , page , search);
            totalPaginas = (int)Math.Ceiling((double)gameResults.count / page_size);
            //games.A( gameResults?.results!);
            foreach (Game game in gameResults?.results!)
            {
                games.Add(game);
            }

            foreach (Game game in games)
            {
                _ = game.LoadImageAsync();
            }
            
            _cargando = false;


        }

        private bool _cargando = false;

        private async void dgGames_ScrollChanged(object sender, ScrollChangedEventArgs e)

        {
            
            // Solo actuamos si el cambio fue en el scroll vertical
            if (e.VerticalChange == 0) return;

            var scrollViewer = (ScrollViewer)e.OriginalSource;

            // e.VerticalOffset: Cuánto hemos bajado
            // e.ViewportHeight: Lo que se ve actualmente en pantalla
            // e.ExtentHeight: La altura total de todos los datos (incluyendo los no visibles)

            double posicionActual = e.VerticalOffset + e.ViewportHeight;
            double umbralCarga = e.ExtentHeight * 0.8; // El 50% de la lista

            // Si pasamos la mitad y no estamos cargando ya...
            if (posicionActual >= umbralCarga && !_cargando)
            {
                page++;
                _cargando = true;
                cargarJuegos(page_size , page , search);

                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            peticionGames peticion = new peticionGames();
            gameResults = new GameResults();
            
            cargarJuegos(page_size , page , search);


        }

        private void btanterior_Click(object sender, RoutedEventArgs e)
        {
            /*if (page >= 2) { 
                page--;
                cargarJuegos(page_size, page, search);
            }*/

            dgDatosJuegos.Visibility = Visibility.Visible;
            dgDatosJuegos.IsEnabled = true;
            gridDatosJuegos.Visibility = Visibility.Hidden;
            gridDatosJuegos.IsEnabled = false;
        }

        private void btsiguiente_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (page < totalPaginas) { 
                page = page + 1;
                cargarJuegos(page_size, page, search);
            }*/
        }

        private async void dgDatosJuegos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDatosJuegos.SelectedItem is Game selectedGame)
            {
                gridDatosJuegos.Visibility = Visibility.Visible;
                gridDatosJuegos.IsEnabled = true;
                imagenJuego.Bitmap = selectedGame.ImagenPrincipal;
                lbName.Content = selectedGame.name;
                lbfechaLanzamiento.Content = selectedGame.released;
                lbRating.Content = $"{selectedGame.rating} / {selectedGame.rating_top}";
                lbNumeroReviews.Content = $"{selectedGame.reviews_count} reviews";
                lbMetacritic.Content = selectedGame.metacritic.HasValue ? $"{selectedGame.metacritic.Value}" : " Sin datos";
                lbPlaytime.Content = $"{selectedGame.playtime} horas de juego";

                lbGeneros.Text = "";
                int contador = 1;
                if (selectedGame.genres?.Any() == true)
                {
                    foreach (genres genre in selectedGame.genres)
                    {
                        lbGeneros.Text += $"{genre.name}";
                        contador++;
                        if (contador <= selectedGame.genres.Count)
                        {
                            lbGeneros.Text += ", ";
                        }

                    }
                }
                else
                {
                    lbGeneros.Text += "Genero desconocido ";
                }
                lbTiendas.Text = "";
                contador = 1;
                if (selectedGame.stores?.Any() == true)
                {
                    foreach (stores tiendas in selectedGame.stores)
                    {

                        lbTiendas.Text += $"{tiendas.store.name}";
                        contador++;
                        if (contador <= selectedGame.stores.Count)
                        {
                            lbTiendas.Text += ", ";
                        }

                    }
                }
                else
                {
                    lbTiendas.Text += "No disponible en tiendas";
                }


                contador = 1;
                lbPlataformas.Text = "";
                if (selectedGame?.platforms!.Any() == true)
                {
                    foreach (platforms plataforma in selectedGame?.platforms!)
                    {

                        lbPlataformas.Text += $"{plataforma?.platform!.name}";
                        contador++;
                        if (contador <= selectedGame?.platforms!.Count)
                        {
                            lbPlataformas.Text += ", ";
                        }




                    }

                }
                else { 
                    lbPlataformas.Text += "Plataforma desconocida";
                }

                    lbEtiquetas.Text = "";
                contador = 1;
                if (selectedGame?.tags?.Any() == true)
                {
                    foreach (tags etiquetas in selectedGame.tags)
                    {

                        lbEtiquetas.Text += $"{etiquetas.name}";
                        contador++;
                        if (contador <= selectedGame.tags.Count)
                        {
                            lbEtiquetas.Text += ", ";
                        }

                    }
                }
                else {
                    lbEtiquetas.Text += "No tiene etiquetas";
                }
                /*
                gameTrailers = new gameTrailers();
                //gameTrailers = crearVideo(@$"{selectedGame.background_image}", selectedGame.id);
                gameTrailers = await new peticionGames().getTrailer(selectedGame!.id);
                if (gameTrailers.results?.Any() == true) { 
                    foreach (GameTrailerResult trailer in gameTrailers.results)
                    {
                        
                            using var libVLC = new LibVLC();
                            using var mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVLC);

                            videoView.MediaPlayer = mediaPlayer;

                            
                            var media = new Media(libVLC, new Uri(@$"{trailer.data?.max}"));
                            mediaPlayer.Play(media);


                            break;
                        

                        //mediaTrailer.Source = new Uri(trailer.data.max);

                    }



                }*/






                capturas.ItemsSource = selectedGame?.short_screenshots;
                dgDatosJuegos.Visibility = Visibility.Hidden;
                dgDatosJuegos.IsEnabled = false;


            }
        }

        

        private void tbBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtBuscarClick(sender, e);
            }



        }
    }
}