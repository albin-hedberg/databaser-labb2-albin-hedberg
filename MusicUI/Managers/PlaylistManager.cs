using Labb2_DbFirst_Template.DataAccess;
using Labb2_DbFirst_Template.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace MusicUI.Managers;

public static class PlaylistManager
{
    public static event Action PlaylistsChanged;
    public static event Action PlaylistsTracksChanged;

    #region CRUD

    public static void CreatePlaylist(string playlistName)
    {
        using var dbContext = new MusicDbContext();

        dbContext.Playlists.Add(new Playlist()
        {
            PlaylistId = GetNewPlaylistId(),
            Name = playlistName
        });

        dbContext.SaveChanges();
        PlaylistsChanged.Invoke();
    }

    public static void UpdatePlaylist(Playlist playlist, string newPlaylistName)
    {
        if (playlist is null || newPlaylistName.Length <= 0)
        {
            return;
        }

        playlist.Name = newPlaylistName;

        using var dbContext = new MusicDbContext();
        dbContext.Playlists.Update(playlist);
        dbContext.SaveChanges();
        PlaylistsChanged.Invoke();
    }

    public static void DeletePlaylist(Playlist playlist)
    {
        if (playlist is null)
        {
            return;
        }

        RemoveAllTracksFromPlaylist(playlist);

        using var dbContext = new MusicDbContext();
        dbContext.Playlists.Remove(playlist);
        dbContext.SaveChanges();
        PlaylistsChanged.Invoke();
    }

    public static void AddTracksToPlaylist(Playlist playlist, List<Track> tracks)
    {
        if (playlist is null || tracks.Count <= 0)
        {
            return;
        }

        using var dbContext = new MusicDbContext();

        var playlistTracks = GetPlaylistTracks(dbContext, playlist);
        if (playlistTracks is null)
        {
            return;
        }

        var playlistTracksIds = playlistTracks.Tracks.Select(t => t.TrackId).ToList();

        var tracksToAdd = dbContext.Tracks
            .Where(t => tracks
                .Select(t => t.TrackId)
                .Except(playlistTracksIds)
                .Contains(t.TrackId))
            .ToList();

        foreach (var track in tracksToAdd)
        {
            playlistTracks.Tracks.Add(track);
        }

        dbContext.SaveChanges();
        PlaylistsTracksChanged.Invoke();
    }

    public static void RemoveTracksFromPlaylist(Playlist playlist, List<Track> tracks)
    {
        if (playlist is null || tracks.Count <= 0)
        {
            return;
        }

        using var dbContext = new MusicDbContext();

        var playlistTracks = GetPlaylistTracks(dbContext, playlist);

        if (playlistTracks is null)
        {
            return;
        }

        foreach (var track in tracks)
        {
            var trackToRemove = playlistTracks.Tracks.FirstOrDefault(t => t.TrackId == track.TrackId);

            if (trackToRemove != null)
            {
                playlistTracks.Tracks.Remove(trackToRemove);
            }
        }

        dbContext.SaveChanges();
        PlaylistsTracksChanged.Invoke();
    }

    private static void RemoveAllTracksFromPlaylist(Playlist playlist)
    {
        if (playlist is null)
        {
            return;
        }

        using var dbContext = new MusicDbContext();

        var playlistTracks = GetPlaylistTracks(dbContext, playlist);

        if (playlistTracks is null)
        {
            return;
        }

        playlistTracks.Tracks.Clear();
        dbContext.SaveChanges();
    }

    #endregion

    #region HelperFunctions

    public static List<Playlist> GetAllPlaylists()
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Playlists.ToList();
    }

    public static Playlist? GetPlaylistTracks(MusicDbContext dbContext,Playlist playlist)
    {
        //using var dbContext = new MusicDbContext();

        return dbContext.Playlists
            .Include(p => p.Tracks)
            .FirstOrDefault(p => p.PlaylistId == playlist.PlaylistId);

        //return dbContext.Playlists
        //    .Where(p => p.PlaylistId == playlist.PlaylistId)
        //    .Include(p => p.Tracks)
        //    .SelectMany(p => p.Tracks)
        //    .ToList();
    }

    public static List<Track> GetPlaylistTracksWithInfo(Playlist playlist)
    {
        using var dbContext = new MusicDbContext();

        //return dbContext.Tracks
        //    .Include(t => t.Genre)
        //    .Include(t => t.Album)
        //    .Include(t => t.Album.Artist)
        //    .Where(t => t.Playlists
        //        .Any(p => p.PlaylistId == playlist.PlaylistId))
        //    .OrderBy(t => t.TrackId)
        //    .ToList();

        return dbContext.Tracks
            .Where(t => t.Playlists
                .Any(p => p.PlaylistId == playlist.PlaylistId))
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

    public static List<Track> GetPlaylistTracksWithInfoByGenre(Playlist playlist, Genre genre)
    {
        using var dbContext = new MusicDbContext();

        return dbContext.Tracks
            .Where(t => t.Playlists
                .Any(p => p.PlaylistId == playlist.PlaylistId) && t.GenreId == genre.GenreId)
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

    private static int GetNewPlaylistId()
    {
        return GetAllPlaylists().Last().PlaylistId + 1;
    }

    #endregion
}
