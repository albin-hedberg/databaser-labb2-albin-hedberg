using System;
using System.Collections.Generic;

namespace Labb2_DbFirst_Template.DataAccess.Entities;

public partial class MediaType
{
    public int MediaTypeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
