using System;
using System.Collections.Generic;

namespace MediaServerApp.Models.Data;

public partial class WatchHistory
{
    public int UserId { get; set; }

    public int MovieId { get; set; }

    public DateTime? LastWatched { get; set; }

    public int? TimeStop { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
