using System;
using System.Collections.Generic;
using System.Linq;

using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Plugins.MediaPortal.Configuration;

namespace MediaBrowser.Plugins.MediaPortal.Helpers
{
    /// <summary>
    /// Provides methods to map configure genres to MB programs
    /// </summary>
    public class GenreMapper
    {
        public const string GENRE_MOVIE = "GENREMOVIE";
        public const string GENRE_SPORT = "GENRESPORT";

        private readonly PluginConfiguration _configuration;
        private readonly List<String> _movieGenres;
        private readonly List<String> _sportGenres; 

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreMapper"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public GenreMapper(PluginConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;

            _movieGenres = new List<string>();
            _sportGenres = new List<string>();

            LoadInternalLists(_configuration.GenreMappings);
        }

        private void LoadInternalLists(Dictionary<string, List<string>> genreMappings)
        {
            if (genreMappings != null)
            {
                if (_configuration.GenreMappings.ContainsKey(GENRE_MOVIE) && _configuration.GenreMappings[GENRE_MOVIE] != null)
                {
                    _movieGenres.AddRange(_configuration.GenreMappings[GENRE_MOVIE]);
                }

                if (_configuration.GenreMappings.ContainsKey(GENRE_SPORT) && _configuration.GenreMappings[GENRE_SPORT] != null)
                {
                    _sportGenres.AddRange(_configuration.GenreMappings[GENRE_SPORT]);
                }
            }
        }

        /// <summary>
        /// Populates the program genres.
        /// </summary>
        /// <param name="program">The program.</param>
        public void PopulateProgramGenres(ProgramInfo program)
        {
            // Check there is a program and genres to map
            if (program != null && program.Genres != null && program.Genres.Count > 0)
            {
                program.IsMovie = _movieGenres.Any(g => program.Genres.Contains(g, StringComparer.InvariantCultureIgnoreCase));
                program.IsSports = _sportGenres.Any(g => program.Genres.Contains(g, StringComparer.InvariantCultureIgnoreCase));
            }
        }
    }
}