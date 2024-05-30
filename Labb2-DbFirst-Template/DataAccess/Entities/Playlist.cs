using System;
using System.Collections.Generic;

namespace Labb2_DbFirst_Template.DataAccess.Entities;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
