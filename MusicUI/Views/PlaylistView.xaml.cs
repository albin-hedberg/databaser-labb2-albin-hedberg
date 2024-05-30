using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Labb2_DbFirst_Template.DataAccess.Entities;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using MusicUI.Managers;
using Track = Labb2_DbFirst_Template.DataAccess.Entities.Track;

namespace MusicUI.Views
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl
    {
        private bool _sortTrackListByDefaultOrder;
        private const string DefaultFilterText = "Filter tracks...";

        public PlaylistView()
        {
            InitializeComponent();
            PopulatePlaylistList();
            PopulateGenreComboBox();

            FilterTrackListTextBox.Text = DefaultFilterText;

            PlaylistManager.PlaylistsChanged += PlaylistManager_PlaylistsChanged;
            PlaylistManager.PlaylistsTracksChanged += PlaylistManager_PlaylistsTracksChanged;
            TrackManager.TracksChanged += TrackManager_TracksChanged;
            ArtistManager.ArtistsChanged += ArtistManager_ArtistsChanged;
            AlbumManager.AlbumsChanged += AlbumManager_AlbumsChanged;
        }

        #region Events

        private void PlaylistManager_PlaylistsChanged()
        {
            PopulatePlaylistList();
        }

        private void PlaylistManager_PlaylistsTracksChanged()
        {
            DisplayPlaylistTracks((Playlist)PlaylistList.SelectedItem);
        }

        private void TrackManager_TracksChanged()
        {
            DisplayPlaylistTracks((Playlist)PlaylistList.SelectedItem);
        }

        private void ArtistManager_ArtistsChanged()
        {
            DisplayPlaylistTracks((Playlist)PlaylistList.SelectedItem);
        }

        private void AlbumManager_AlbumsChanged()
        {
            DisplayPlaylistTracks((Playlist)PlaylistList.SelectedItem);
        }

        #endregion

        #region Populate

        private void PopulatePlaylistList()
        {
            PlaylistList.ItemsSource = PlaylistManager.GetAllPlaylists();
        }

        private void PopulateGenreComboBox()
        {
            FilterTrackListByGenreComboBox.ItemsSource = TrackManager.GetAllGenresForComboBox();
            FilterTrackListByGenreComboBox.SelectedIndex = 0;
        }

        private void PopulateContextMenu(Playlist currentPlaylist)
        {
            if (currentPlaylist is null)
            {
                AddTracksToPlaylistContextMenu.Items.Clear();
                AddTracksToPlaylistContextMenu.IsEnabled = false;

                RemoveTracksFromPlaylist.IsEnabled = false;

                return;
            }

            AddTracksToPlaylistContextMenu.Items.Clear();
            AddTracksToPlaylistContextMenu.IsEnabled = true;

            RemoveTracksFromPlaylist.IsEnabled = true;

            foreach (var playlist in PlaylistManager.GetAllPlaylists())
            {
                var menuItem = new MenuItem
                {
                    Header = playlist.Name,
                    Tag = playlist
                };

                if (currentPlaylist.PlaylistId == playlist.PlaylistId)
                {
                    menuItem.IsEnabled = false;
                }

                menuItem.Click += AddTracksToPlaylist_OnClick;
                AddTracksToPlaylistContextMenu.Items.Add(menuItem);
            }
        }

        private void DisplayPlaylistTracks(Playlist playlist)
        {
            if (playlist is null)
            {
                CurrentItemCountInTrackList.Text = "(0 Tracks)";
                return;
            }

            TrackList.ItemsSource = PlaylistManager.GetPlaylistTracksWithInfo(playlist);

            CurrentItemCountInTrackList.Text = $"({TrackList.Items.Count} tracks)";
        }

        private void PlaylistList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTrackListTextBox.Text = DefaultFilterText;
            FilterTrackListTextBox.FontStyle = FontStyles.Italic;
            FilterTrackListByGenreComboBox.SelectedIndex = 0;

            if (PlaylistList.SelectedItem == null)
            {
                TrackList.ItemsSource = null;
            }

            DisplayPlaylistTracks((Playlist)PlaylistList.SelectedItem);
            PopulateContextMenu((Playlist)PlaylistList.SelectedItem);
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

        private void FilterTrackListTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (FilterTrackListTextBox.Text.Length <= 0)
            {
                FilterTrackListTextBox.Text = DefaultFilterText;
                FilterTrackListTextBox.FontStyle = FontStyles.Italic;
            }

            FilterTrackListTextBox.Foreground = Brushes.SlateGray;
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

        private void FilterTrackListByGenreComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = (Genre)FilterTrackListByGenreComboBox.SelectedItem;
            var selectedPlaylist = (Playlist)PlaylistList.SelectedItem;

            if (selectedPlaylist is null || selectedGenre is null)
            {
                return;
            }

            if (FilterTrackListByGenreComboBox.SelectedIndex == 0)
            {
                DisplayPlaylistTracks((Playlist)PlaylistList.SelectedItem);
                return;
            }

            TrackList.ItemsSource = PlaylistManager.GetPlaylistTracksWithInfoByGenre(selectedPlaylist, selectedGenre);

            CurrentItemCountInTrackList.Text = $"({TrackList.Items.Count} tracks)";
            FilterTrackListTextBox.Text = DefaultFilterText;
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
        // Create
        private void NewPlaylistBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (NewPlaylistTextBox.Visibility == Visibility.Visible)
            {
                NewPlaylistTextBox.Visibility = Visibility.Collapsed;
                CreateNewPlaylistBtn.Visibility = Visibility.Collapsed;
                return;
            }

            NewPlaylistTextBox.Visibility = Visibility.Visible;
            CreateNewPlaylistBtn.Visibility = Visibility.Visible;
            NewPlaylistTextBox.Focus();
        }

        private void NewPlaylistTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            CreateNewPlaylistBtn_OnClick(sender, e);
        }

        private void CreateNewPlaylistBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (NewPlaylistTextBox.Text.Length <= 0)
            {
                return;
            }

            PlaylistManager.CreatePlaylist(NewPlaylistTextBox.Text);

            NewPlaylistTextBox.Text = string.Empty;
            NewPlaylistTextBox.Visibility = Visibility.Collapsed;
            CreateNewPlaylistBtn.Visibility = Visibility.Collapsed;
        }

        // Update
        private void EditPlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            if (PlaylistList.SelectedItem is null)
            {
                return;
            }

            if (EditPlaylistTextBox.Visibility == Visibility.Visible)
            {
                EditPlaylistTextBox.Visibility = Visibility.Collapsed;
                SaveEditPlaylistBtn.Visibility = Visibility.Collapsed;
                return;
            }

            EditPlaylistTextBox.Visibility = Visibility.Visible;
            SaveEditPlaylistBtn.Visibility = Visibility.Visible;
            EditPlaylistTextBox.Focus();

            var selectedPlaylist = (Playlist)PlaylistList.SelectedItem;

            EditPlaylistTextBox.Text = selectedPlaylist.Name;
        }

        private void EditPlaylistTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            SaveEditPlaylistBtn_OnClick(sender, e);
        }

        private void SaveEditPlaylistBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (PlaylistList.SelectedItem is null || EditPlaylistTextBox.Text.Length < 1)
            {
                return;
            }

            PlaylistManager.UpdatePlaylist((Playlist)PlaylistList.SelectedItem, EditPlaylistTextBox.Text);

            EditPlaylistTextBox.Text = string.Empty;
            EditPlaylistTextBox.Visibility = Visibility.Collapsed;
            SaveEditPlaylistBtn.Visibility = Visibility.Collapsed;
        }

        // Delete
        private void DeletePlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            if (PlaylistList.SelectedItem is null)
            {
                return;
            }

            var selectedPlaylist = (Playlist)PlaylistList.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the playlist: {selectedPlaylist.Name}?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            PlaylistManager.DeletePlaylist(selectedPlaylist);
        }

        // Add/Remove tracks
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

        private void RemoveTracksFromPlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = (Playlist)PlaylistList.SelectedItem;
            var selectedTracks = TrackList.SelectedItems.Cast<Track>().ToList();

            if (selectedTracks.Count < 1 || selectedPlaylist is null)
            {
                return;
            }

            PlaylistManager.RemoveTracksFromPlaylist(selectedPlaylist, selectedTracks);
        }

        #endregion
    }
}
