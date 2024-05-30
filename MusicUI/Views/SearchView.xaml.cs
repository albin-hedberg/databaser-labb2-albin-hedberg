using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Labb2_DbFirst_Template.DataAccess.Entities;
using MusicUI.Managers;

namespace MusicUI.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        private bool _sortTrackListByDefaultOrder;
        private const string DefaultFilterText = "Filter tracks...";

        public SearchView()
        {
            InitializeComponent();

            PopulateTrackList();
            PopulateGenreComboBox();
            PopulateContextMenu();

            FilterTrackListTextBox.Text = DefaultFilterText;

            PlaylistManager.PlaylistsChanged += PlaylistManager_PlaylistsChanged;
            TrackManager.TracksChanged += TrackManager_TracksChanged;
            ArtistManager.ArtistsChanged += ArtistManager_ArtistsChanged;
            AlbumManager.AlbumsChanged += AlbumManager_AlbumsChanged;
        }

        #region Events

        private void PlaylistManager_PlaylistsChanged()
        {
            PopulateContextMenu();
        }

        private void TrackManager_TracksChanged()
        {
            PopulateTrackList();
        }

        private void ArtistManager_ArtistsChanged()
        {
            PopulateTrackList();
        }

        private void AlbumManager_AlbumsChanged()
        {
            PopulateTrackList();
        }

        #endregion

        #region Populate

        private void PopulateTrackList()
        {
            TrackList.ItemsSource = TrackManager.GetAllTracksWithInfo();

            CurrentItemCountInTrackList.Text = $"({TrackList.Items.Count} tracks)";
        }

        private void PopulateGenreComboBox()
        {
            FilterTrackListByGenreComboBox.ItemsSource = TrackManager.GetAllGenresForComboBox();
            FilterTrackListByGenreComboBox.SelectedIndex = 0;
        }

        private void PopulateContextMenu()
        {
            TrackListContextMenu.Items.Clear();

            var playlists = PlaylistManager.GetAllPlaylists();

            var topHeader = new MenuItem
            {
                Header = "Add track(s) to playlist:",
                IsEnabled = false
            };

            TrackListContextMenu.Items.Add(topHeader);
            TrackListContextMenu.Items.Add(new Separator());

            foreach (var playlist in playlists)
            {
                var menuItem = new MenuItem
                {
                    Header = playlist.Name,
                    Tag = playlist
                };
                menuItem.Click += AddTracksToPlaylist_OnClick;
                TrackListContextMenu.Items.Add(menuItem);
            }
        }

        #endregion

        #region FilterTrackList

        private void FilterTrackListTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (FilterTrackListTextBox.Text == DefaultFilterText)
            {
                FilterTrackListTextBox.Text = string.Empty;
                FilterTrackListTextBox.FontStyle = FontStyles.Normal;
            }

            FilterTrackListTextBox.Foreground = Brushes.Black;
        }

        private void FilterTrackListTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = FilterTrackListTextBox.Text;

            var view = CollectionViewSource.GetDefaultView(TrackList.ItemsSource);
            if (view is null)
            {
                return;
            }

            if (filterText.Length == 0 || filterText == DefaultFilterText)
            {
                view.Filter = null;
                CurrentItemCountInTrackList.Text = $"({TrackList.Items.Count} tracks)";
                return;
            }

            view.Filter = f =>
            {
                if (f is Track track)
                {
                    return track.Name.ToLower().Contains(filterText.ToLower()) ||
                           track.Album.Title.ToLower().Contains(filterText.ToLower()) ||
                           track.Album.Artist.Name.ToLower().Contains(filterText.ToLower());
                }

                return false;
            };

            CurrentItemCountInTrackList.Text = $"({TrackList.Items.Count} tracks)";
        }

        private void FilterTrackListTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (FilterTrackListTextBox.Text.Length < 1)
            {
                FilterTrackListTextBox.Text = DefaultFilterText;
                FilterTrackListTextBox.FontStyle = FontStyles.Italic;
            }

            FilterTrackListTextBox.Foreground = Brushes.SlateGray;
        }

        private void FilterTrackListByGenreComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterTrackListByGenreComboBox.SelectedIndex == 0)
            {
                PopulateTrackList();
                return;
            }

            var selectedGenre = (Genre)FilterTrackListByGenreComboBox.SelectedItem;

            if (selectedGenre is null)
            {
                return;
            }

            TrackList.ItemsSource = TrackManager.GetAllTracksWithInfoByGenre(selectedGenre);
            
            CurrentItemCountInTrackList.Text = $"({TrackList.Items.Count} tracks)";
        }

        private void SortTrackListByColumnHeader_OnClick(object sender, RoutedEventArgs e)
        {
            var headerClicked = (GridViewColumnHeader)e.OriginalSource;
            var trackListView = CollectionViewSource.GetDefaultView(TrackList.ItemsSource);

            if (headerClicked is null || trackListView is null)
            {
                return;
            }

            string sortBy = headerClicked.Tag.ToString();
            var direction = ListSortDirection.Ascending;
            headerClicked.FontWeight = FontWeights.Bold;

            if (trackListView.SortDescriptions.Count > 0 && trackListView.SortDescriptions[0].PropertyName == sortBy)
            {
                direction = (trackListView.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;

                if (_sortTrackListByDefaultOrder)
                {
                    sortBy = nameof(Track.TrackId);
                    direction = ListSortDirection.Ascending;
                    headerClicked.FontWeight = FontWeights.Normal;

                    _sortTrackListByDefaultOrder = false;
                }
                else
                {
                    _sortTrackListByDefaultOrder = true;
                }
            }

            trackListView.SortDescriptions.Clear();
            trackListView.SortDescriptions.Add(new SortDescription(sortBy, direction));
        }

        #endregion

        #region CRUD

        private void AddTracksToPlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
            {
                return;
            }

            var selectedPlaylist = (Playlist)menuItem.Tag;
            if (selectedPlaylist is null)
            {
                return;
            }

            var selectedTracks = TrackList.SelectedItems.Cast<Track>().ToList();
            if (selectedTracks.Count <= 0)
            {
                return;
            }

            PlaylistManager.AddTracksToPlaylist(selectedPlaylist, selectedTracks);
        }

        #endregion
    }
}
