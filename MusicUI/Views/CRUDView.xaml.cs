using System.Windows;
using System.Windows.Controls;
using Labb2_DbFirst_Template.DataAccess.Entities;
using System.ComponentModel;
using System.Windows.Data;
using MusicUI.Managers;

namespace MusicUI.Views
{
    /// <summary>
    /// Interaction logic for CRUDView.xaml
    /// </summary>
    public partial class CRUDView : UserControl
    {
        private bool _sortTrackListByDefaultOrder;

        public CRUDView()
        {
            InitializeComponent();
            PopulateTracksList();
            PopulateArtistsList();
            PopulateAlbumsList();
            PopulateGenreList();
            PopulateLengthComboBoxes();

            TrackManager.TracksChanged += TrackManager_TracksChanged;
            ArtistManager.ArtistsChanged += ArtistManager_ArtistsChanged;
            AlbumManager.AlbumsChanged += AlbumManager_AlbumsChanged;
        }

        #region Events

        private void TrackManager_TracksChanged()
        {
            PopulateTracksList();
        }

        private void ArtistManager_ArtistsChanged()
        {
            PopulateTracksList();
            PopulateArtistsList();
            PopulateAlbumsList();
        }

        private void AlbumManager_AlbumsChanged()
        {
            PopulateTracksList();
            PopulateArtistsList();
            PopulateAlbumsList();
        }

        #endregion

        #region PopulateLists

        private void PopulateTracksList()
        {
            TracksList.ItemsSource = TrackManager.GetAllTracksWithAllInfo();
        }

        private void PopulateArtistsList()
        {
            ModifyArtistsList.ItemsSource = ArtistManager.GetAllArtists();

            NewAlbumArtistComboBox.ItemsSource = ArtistManager.GetAllArtists();
            UpdateAlbumArtistComboBox.ItemsSource = ArtistManager.GetAllArtists();
        }

        private void PopulateAlbumsList()
        {
            ModifyAlbumsList.ItemsSource = AlbumManager.GetAllAlbumsWithInfo();

            CreateTrackAlbumComboBox.ItemsSource = AlbumManager.GetAllAlbumsWithInfo();
            UpdateTrackAlbumComboBox.ItemsSource = AlbumManager.GetAllAlbumsWithInfo();
        }

        private void PopulateGenreList()
        {
            CreateTrackGenreComboBox.ItemsSource = TrackManager.GetAllGenres();
            UpdateTrackGenreComboBox.ItemsSource = TrackManager.GetAllGenres();
        }

        private void PopulateLengthComboBoxes()
        {
            for (int i = 0; i < 100; i++)
            {
                CreateTrackLengthMinutes.Items.Add(i);
                UpdateTrackLengthMinutes.Items.Add(i);

                if (i < 60)
                {
                    CreateTrackLengthSeconds.Items.Add(i);
                    UpdateTrackLengthSeconds.Items.Add(i);
                }
            }

            CreateTrackLengthMinutes.SelectedIndex = 0;
            CreateTrackLengthSeconds.SelectedIndex = 0;

            UpdateTrackLengthMinutes.SelectedIndex = 0;
            UpdateTrackLengthSeconds.SelectedIndex = 0;
        }

        #endregion

        #region TrackCRUD

        private void CreateNewTrackkBtn_OnClickBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string trackName = CreateTrackNameTextBox.Text;
            //var selectedArtist = (Artist)CreateTrackArtistComboBox.SelectedItem;
            var selectedAlbum = (Album)CreateTrackAlbumComboBox.SelectedItem;
            var selectedGenre = (Genre)CreateTrackGenreComboBox.SelectedItem;

            if (trackName.Length < 1)
            {
                CreateTrackErrorText.Text = "You must enter a name for the track.";
                return;
            }

            if (selectedAlbum is null)
            {
                CreateTrackErrorText.Text = "You must select an album for the track.";
                return;
            }

            if (selectedGenre is null)
            {
                CreateTrackErrorText.Text = "You must select a genre for the track.";
                return;
            }

            if (CreateTrackLengthMinutes.SelectedItem is null ||
                CreateTrackLengthSeconds.SelectedItem is null)
            {
                CreateTrackErrorText.Text = "You must enter a length for the track.";
                return;
            }

            int minutes = (int)CreateTrackLengthMinutes.SelectedItem;
            int seconds = (int)CreateTrackLengthSeconds.SelectedItem;

            TrackManager.CreateTrack(trackName, selectedAlbum, selectedGenre, minutes, seconds);

            CreateTrackErrorText.Text = string.Empty;
            CreateTrackNameTextBox.Text = string.Empty;
            CreateTrackAlbumComboBox.SelectedItem = null;
            CreateTrackGenreComboBox.SelectedItem = null;
            CreateTrackLengthMinutes.SelectedIndex = 0;
            CreateTrackLengthSeconds.SelectedIndex = 0;
        }

        private void UpdateTrackBtn_OnClickBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string newTrackName = UpdateTrackNameTextBox.Text;
            var selectedTrack = (Track)TracksList.SelectedItem;
            var selectedAlbum = (Album)UpdateTrackAlbumComboBox.SelectedItem;
            var selectedGenre = (Genre)UpdateTrackGenreComboBox.SelectedItem;

            if (selectedTrack is null)
            {
                ModifyTrackErrorText.Text = "You must select a track from the list to modify.";
                return;
            }

            if (newTrackName.Length < 1)
            {
                ModifyTrackErrorText.Text = "You must enter a name for the track.";
                return;
            }

            if (selectedAlbum is null)
            {
                ModifyTrackErrorText.Text = "You must select an album for the track.";
                return;
            }

            if (selectedGenre is null)
            {
                ModifyTrackErrorText.Text = "You must select a genre for the track.";
                return;
            }

            if (UpdateTrackLengthMinutes.SelectedItem is null ||
                UpdateTrackLengthSeconds.SelectedItem is null)
            {
                ModifyTrackErrorText.Text = "You must enter a length for the track.";
                return;
            }

            int minutes = (int)UpdateTrackLengthMinutes.SelectedItem;
            int seconds = (int)UpdateTrackLengthSeconds.SelectedItem;

            TrackManager.UpdateTrack(newTrackName, selectedTrack, selectedAlbum, selectedGenre, minutes, seconds);

            ModifyTrackErrorText.Text = string.Empty;
            UpdateTrackNameTextBox.Text = string.Empty;
            UpdateTrackAlbumComboBox.SelectedItem = null;
            UpdateTrackGenreComboBox.SelectedItem = null;
            UpdateTrackLengthMinutes.SelectedIndex = 0;
            UpdateTrackLengthSeconds.SelectedIndex = 0;
        }

        private void DeleteTrackBtn_OnClickBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedTrack = (Track)TracksList.SelectedItem;

            if (selectedTrack is null)
            {
                ModifyTrackErrorText.Text = "You must select a track from the list to delete.";
                return;
            }

            TrackManager.DeleteTrack(selectedTrack);

            ModifyAlbumErrorText.Text = string.Empty;
        }

        #endregion

        #region ArtistCRUD

        private void CreateNewArtistBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string newArtistName = NewArtistNameTextBox.Text;

            if (newArtistName.Length < 1)
            {
                NewArtistErrorText.Text = "You must enter a name for the artist.";
                return;
            }

            ArtistManager.CreateArtist(newArtistName);

            NewArtistErrorText.Text = string.Empty;
            NewArtistNameTextBox.Text = string.Empty;
        }

        private void UpdateArtistBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedArtist = (Artist)ModifyArtistsList.SelectedItem;

            if (selectedArtist is null)
            {
                ModifyArtistErrorText.Text = "You must select an artist to update.";
                return;
            }

            if (UpdateArtistNameTextBox.Text.Length < 1)
            {
                ModifyArtistErrorText.Text = "You must enter a name for the artist.";
                return;
            }

            ArtistManager.UpdateArtist(selectedArtist, UpdateArtistNameTextBox.Text);

            ModifyArtistErrorText.Text = string.Empty;
            UpdateArtistNameTextBox.Text = string.Empty;
        }

        private void DeleteArtistBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedArtist = (Artist)ModifyArtistsList.SelectedItem;

            if (selectedArtist is null)
            {
                ModifyArtistErrorText.Text = "You must select an artist to delete.";
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the artist: {selectedArtist.Name}?\n\n" +
                $"Doing so will also delete any/all albums and songs associated with the artist!",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            ArtistManager.DeleteArtist(selectedArtist);

            ModifyArtistErrorText.Text = string.Empty;
        }

        private void ModifyArtistsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedArtist = (Artist)ModifyArtistsList.SelectedItem;

            if (selectedArtist is null)
            {
                UpdateArtistNameTextBox.Text = string.Empty;
                return;
            }

            UpdateArtistNameTextBox.Text = selectedArtist.Name;
        }

        #endregion

        #region AlbumCRUD

        private void CreateNewAlbumBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string newAlbumTitle = NewAlbumNameTextBox.Text;
            var newAlbumArtist = (Artist)NewAlbumArtistComboBox.SelectedItem;

            if (newAlbumTitle.Length < 1)
            {
                NewAlbumErrorText.Text = "You must enter a name for the album.";
                return;
            }

            if (newAlbumArtist is null)
            {
                NewAlbumErrorText.Text = "You must select an artist for the album";
                return;
            }

            if (AlbumManager.CheckIfAlbumAlreadyExists(newAlbumTitle, newAlbumArtist))
            {
                NewAlbumErrorText.Text = "There is already an album with that title and artist.";
                return;
            }

            AlbumManager.CreateAlbum(newAlbumTitle, newAlbumArtist);

            NewAlbumErrorText.Text = string.Empty;
            NewAlbumNameTextBox.Text = string.Empty;
            NewAlbumArtistComboBox.SelectedItem = null;
        }

        private void UpdateAlbumBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedAlbum = (Album)ModifyAlbumsList.SelectedItem;
            var selectedArtist = (Artist)UpdateAlbumArtistComboBox.SelectedItem;
            string newTitle = UpdateAlbumNameTextBox.Text;

            if (selectedAlbum is null)
            {
                ModifyAlbumErrorText.Text = "You must select an album to update.";
                return;
            }

            if (selectedArtist is null)
            {
                ModifyAlbumErrorText.Text = "You must select an artist for the album.";
                return;
            }

            if (newTitle.Length < 1)
            {
                ModifyAlbumErrorText.Text = "You must enter a name for the album.";
                return;
            }

            AlbumManager.UpdateAlbum(newTitle, selectedAlbum, selectedArtist);

            ModifyAlbumErrorText.Text = string.Empty;
            UpdateAlbumArtistComboBox.SelectedItem = null;
        }

        private void DeleteAlbumBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedAlbum = (Album)ModifyAlbumsList.SelectedItem;

            if (selectedAlbum is null)
            {
                ModifyAlbumErrorText.Text = "You must select an album from the list to delete.";
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the album: {selectedAlbum.Title}?\n\n" +
                $"Doing so will also delete any/all songs associated with the album!",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            AlbumManager.DeleteAlbum(selectedAlbum);

            ModifyAlbumErrorText.Text = string.Empty;
        }

        private void ModifyAlbumsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAlbum = (Album)ModifyAlbumsList.SelectedItem;

            if (selectedAlbum is null)
            {
                UpdateAlbumNameTextBox.Text = string.Empty;
                UpdateAlbumArtistComboBox.SelectedItem = null;
                return;
            }

            UpdateAlbumNameTextBox.Text = selectedAlbum.Title;
            UpdateAlbumArtistComboBox.SelectedItem = UpdateAlbumArtistComboBox.Items
                .Cast<Artist>()
                .FirstOrDefault(a => a.ArtistId == selectedAlbum.ArtistId);
        }

        #endregion

        #region TrackListFilter

        private void ModifyTracksList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTrack = (Track)TracksList.SelectedItem;

            if (selectedTrack is null)
            {
                UpdateTrackNameTextBox.Text = string.Empty;
                UpdateTrackAlbumComboBox.SelectedItem = null;
                UpdateTrackGenreComboBox.SelectedItem = null;
                UpdateTrackLengthMinutes.SelectedItem = null;
                UpdateTrackLengthSeconds.SelectedItem = null;

                return;
            }

            UpdateTrackNameTextBox.Text = selectedTrack.Name;
            UpdateTrackAlbumComboBox.SelectedItem = UpdateTrackAlbumComboBox.Items
                .Cast<Album>()
                .FirstOrDefault(a => a.AlbumId == selectedTrack.AlbumId);
            UpdateTrackGenreComboBox.SelectedItem = UpdateTrackGenreComboBox.Items
                .Cast<Genre>()
                .FirstOrDefault(g => g.GenreId == selectedTrack.GenreId);

            var (minutes, seconds) = TrackManager.ConvertFromMilliseconds(selectedTrack.Milliseconds);
            UpdateTrackLengthMinutes.SelectedItem = UpdateTrackLengthMinutes.Items.Cast<int>().FirstOrDefault(m => m == minutes);
            UpdateTrackLengthSeconds.SelectedItem = UpdateTrackLengthSeconds.Items.Cast<int>().FirstOrDefault(s => s == seconds);
        }

        private void SortTracksListByColumnHeader_OnClick(object sender, RoutedEventArgs e)
        {
            var headerClicked = (GridViewColumnHeader)e.OriginalSource;
            var currentView = CollectionViewSource.GetDefaultView(TracksList.ItemsSource);

            if (headerClicked is null || currentView is null)
            {
                return;
            }

            string sortBy = headerClicked.Tag.ToString();
            var direction = ListSortDirection.Ascending;
            headerClicked.FontWeight = FontWeights.Bold;

            if (currentView.SortDescriptions.Count > 0 && currentView.SortDescriptions[0].PropertyName == sortBy)
            {
                direction = (currentView.SortDescriptions[0].Direction == ListSortDirection.Ascending)
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

            currentView.SortDescriptions.Clear();
            currentView.SortDescriptions.Add(new SortDescription(sortBy, direction));
        }

        #endregion
    }
}
