using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public enum WebSortField
    {
        Title = 0,
        DateAdded = 1,
        Year = 2,
        Genre = 3,
        Rating = 4,
        Categories = 5,
        MusicTrackNumber = 6,
        MusicComposer = 7,
        TVEpisodeNumber = 8,
        TVSeasonNumber = 9,
        PictureDateTaken = 10,
        TVDateAired = 11,
        Type = 12,
        User = 15,
        Channel = 16,
        StartTime = 17,
        NaturalTitle = 18
    }
}