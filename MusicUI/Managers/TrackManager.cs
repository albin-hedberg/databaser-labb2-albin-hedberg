using Labb2_DbFirst_Template.DataAccess;
using Labb2_DbFirst_Template.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace MusicUI.Managers;

public static class TrackManager
{
    public static event Action TracksChanged;

    #region CRUD

    public static void CreateTrack(string name, Album album, Genre genre, int minutes, int seconds)
    {
        using var dbContext = new MusicDbContext();

        dbContext.Tracks.Add(new Track()
        {
            TrackId = GetNewTrackId(),
            Name = name,
            AlbumId = album.AlbumId,
            GenreId = genre.GenreId,
            Milliseconds = ConvertToMilliseconds(minutes, seconds),
            MediaTypeId = 1 // MPEG audio file
        });

        dbContext.SaveChanges();
        TracksChanged.Invoke();
    }

    public static void UpdateTrack(string name, Track track, Album album, Genre genre, int minutes, int seconds)
    {
        if (track is null ||
            album is null ||
            genre is null)
        {
            return;
        }

        using var dbContext = new MusicDbContext();

        var trackToUpdate = dbContext.Tracks.Find(track.TrackId);
        if (trackToUpdate is null)
        {
            return;
        }

        trackToUpdate.Name = name;
        trackToUpdate.AlbumId = album.AlbumId;
        trackToUpdate.GenreId = genre.GenreId;
        trackToUpdate.Milliseconds = ConvertToMilliseconds(minutes, seconds);

        dbContext.Tracks.Update(trackToUpdate);

        dbContext.SaveChanges();
        TracksChanged.Invoke();
    }

    public static void DeleteTrack(Track track)
    {
        if (track is null)
        {
            return;
        }

        using var dbContext = new MusicDbContext();

        var trackAsList = new List<Track> { track };

        foreach (var playlist in dbContext.Playlists.ToList())
        {
            PlaylistManager.RemoveTracksFromPlaylist(playlist, trackAsList);
        }

        dbContext.Tracks.Remove(track);

        dbContext.SaveChanges();
        TracksChanged.Invoke();
    }

    #endregion

    #region HelperFunctions

    public static List<Track> GetAllTracks()
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Tracks.ToList();
    }

    public static List<Track> GetAllTracksWithInfo()
    {
        using var dbContext = new MusicDbContext();

        return dbContext.Tracks
            .OrderBy(t => t.TrackId)
            .Select(t => new Track()
            {
                TrackId = t.TrackId,
                Name = t.Name,
                Milliseconds = t.Milliseconds,
                Genre = new Genre()
                {
                    Name = t.Genre.Name
                },
                Album = new Album()
                {
                    Title = t.Album.Title,
                    Artist = new Artist()
                    {
                        Name = t.Album.Artist.Name
                    }
                }
            })
            .ToList();
    }

    public static List<Track> GetAllTracksWithInfoByGenre(Genre genre)
    {
        using var dbContext = new MusicDbContext();

        return dbContext.Tracks
            .Where(t => t.GenreId == genre.GenreId)
            .OrderBy(t => t.TrackId)
            .Select(t => new Track()
            {
                TrackId = t.TrackId,
                Name = t.Name,
                Milliseconds = t.Milliseconds,
                Genre = new Genre()
                {
                    Name = t.Genre.Name
                },
                Album = new Album()
                {
                    Title = t.Album.Title,
                    Artist = new Artist()
                    {
                        Name = t.Album.Artist.Name
                    }
                }
            })
            .ToList();
    }

    public static List<Track> GetAllTracksWithAllInfo()
    {
        using var dbContext = new MusicDbContext();

        return dbContext.Tracks
            .Include(t => t.Genre)
            .Include(t => t.Album)
            .Include(t => t.Album.Artist)
            .OrderBy(t => t.TrackId)
            .ToList();
    }

    public static List<Genre> GetAllGenres()
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Genres.ToList();
    }

    public static List<Genre> GetAllGenresForComboBox()
    {
        using var dbContext = new MusicDbContext();

        var genres = dbContext.Genres.ToList();

        var allGenres = new Genre()
        {
            Name = "All Genres"
        };

        genres.Insert(0, allGenres);

        return genres;
    }

    public static int GetNewTrackId()
    {
        return GetAllTracks().Last().TrackId + 1;
    }

    public static int ConvertToMilliseconds(int minutes, int seconds)
    {
        //int milliseconds = (minutes * 60) * 1000;
        //milliseconds += (seconds * 1000);
        //return milliseconds;

        return ((minutes * 60) * 1000) + (seconds * 1000);
    }

    public static (int minutes, int seconds) ConvertFromMilliseconds(int milliseconds)
    {
        int totalSeconds = milliseconds / 1000;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return (minutes, seconds);
    }

    #endregion
}
