using System;
using System.Collections.Generic;

namespace Labb2_DbFirst_Template.DataAccess.Entities;

public partial class Track
{
    public int TrackId { get; set; }

    public string Name { get; set; } = null!;

    public int? AlbumId { get; set; }

    public int MediaTypeId { get; set; }

    public int? GenreId { get; set; }

    public string? Composer { get; set; }

    public int Milliseconds { get; set; }

    public int? Bytes { get; set; }

    public double UnitPrice { get; set; }

    public virtual Album? Album { get; set; }

    public virtual Genre? Genre { get; set; }

    public virtual MediaType MediaType { get; set; } = null!;

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public string FormattedTime
    {
        get
        {
            var timeSpan = TimeSpan.FromMilliseconds(Milliseconds);
            return $"{(int)timeSpan.TotalMinutes:00}:{timeSpan.Seconds:00}";
        }
    }
}
