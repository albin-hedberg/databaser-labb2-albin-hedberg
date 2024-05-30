using System.Windows;
using Labb2_DbFirst_Template.DataAccess.Entities;
using Labb2_DbFirst_Template.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MusicUI.Managers;

public class ArtistManager
{
    public static event Action ArtistsChanged;

    #region CRUD

    public static void CreateArtist(string name)
    {
        using var dbContext = new MusicDbContext();

        if (dbContext.Artists.Any(a => a.Name == name))
        {
            MessageBox.Show("There is already an artist with that name.");
            return;
        }

        dbContext.Artists.Add(new Artist()
        {
            ArtistId = GetNewArtistId(),
            Name = name
        });

        dbContext.SaveChanges();
        ArtistsChanged.Invoke();
    }

    public static void UpdateArtist(Artist artist, string name)
    {
        if (artist is null || name.Length < 1)
        {
            return;
        }

        using var dbContext = new MusicDbContext();

        artist.Name = name;

        dbContext.Artists.Update(artist);

        dbContext.SaveChanges();
        ArtistsChanged.Invoke();
    }

    public static void DeleteArtist(Artist artist)
    {
        if (artist is null)
        {
            return;
        }

        DeleteArtistTracks(artist);
        DeleteArtistAlbums(artist);

        using var dbContext = new MusicDbContext();

        dbContext.Artists.Remove(artist);

        dbContext.SaveChanges();
        ArtistsChanged.Invoke();
    }

    private static void DeleteArtistTracks(Artist artist)
    {
        using var dbContext = new MusicDbContext();

        var artistTracks = dbContext.Tracks
            .Include(t => t.Album.Artist)
            .Where(a => a.Album.ArtistId == artist.ArtistId)
            .ToList();

        foreach (var track in artistTracks)
        {
            TrackManager.DeleteTrack(track);
        }
    }

    private static void DeleteArtistAlbums(Artist artist)
    {
        using var dbContext = new MusicDbContext();

        var artistAlbums = dbContext.Albums
            .Where(a => a.ArtistId == artist.ArtistId)
            .ToList();

        foreach (var album in artistAlbums)
        {
            AlbumManager.DeleteAlbum(album);
        }
    }

    #endregion

    #region HelperFunctions

    public static List<Artist> GetAllArtists()
    {
        using var dbContext = new MusicDbContext();
        return dbContext.Artists.ToList();
    }

    public static int GetNewArtistId()
    {
        return GetAllArtists().Last().ArtistId + 1;
    }

    #endregion
}
