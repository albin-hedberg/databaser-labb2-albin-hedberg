using Labb2_DbFirst_Template.DataAccess.Entities;
using Labb2_DbFirst_Template.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MusicUI.Managers;

public class AlbumManager
{
    public static event Action AlbumsChanged;

    #region CRUD

    public static void CreateAlbum(string title, Artist artist)
    {
        using var dbContext = new MusicDbContext();

        dbContext.Albums.Add(new Album()
        {
            AlbumId = GetNewAlbumId(),
            Title = title,
            ArtistId = artist.ArtistId
        });

        dbContext.SaveChanges();
        AlbumsChanged.Invoke();
    }

    public static void UpdateAlbum(string title, Album album, Artist artist)
    {
        using var dbContext = new MusicDbContext();
        
        var albumToUpdate = dbContext.Albums.Find(album.AlbumId);
        if (albumToUpdate is null)
        {
            return;
        }

        albumToUpdate.Title = title;
        albumToUpdate.ArtistId = artist.ArtistId;

        dbContext.SaveChanges();
        AlbumsChanged.Invoke();
    }

    public static void DeleteAlbum(Album album)
    {
        DeleteAlbumTracks(album);

        using var dbContext = new MusicDbContext();

        dbContext.Albums.Remove(album);

        dbContext.SaveChanges();
        AlbumsChanged.Invoke();
    }

    private static void DeleteAlbumTracks(Album album)
    {
        using var dbContext = new MusicDbContext();

        var albumTracks = dbContext.Tracks
            .Where(a => a.Album.AlbumId == album.AlbumId)
            .ToList();

        foreach (var track in albumTracks)
        {
            TrackManager.DeleteTrack(track);
        }
    }

    #endregion

    #region HelperFunctions

    public static List<Album> GetAllAlbums()
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Albums.ToList();
    }

    public static List<Album> GetAllAlbumsWithInfo()
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Albums
            .Include(a => a.Artist)
            .OrderBy(a => a.AlbumId)
            .ToList();
    }

    public static int GetNewAlbumId()
    {
        return GetAllAlbums().Last().AlbumId + 1;
    }

    public static bool CheckIfAlbumAlreadyExists(string title, Artist artist)
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Albums.Any(a => a.Title == title && a.ArtistId == artist.ArtistId);
    }

    #endregion
}
