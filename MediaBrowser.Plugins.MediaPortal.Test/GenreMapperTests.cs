using System;
using System.Collections.Generic;

using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Plugins.MediaPortal.Configuration;
using MediaBrowser.Plugins.MediaPortal.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaBrowser.Plugins.MediaPortal.Test
{
    [TestClass]
    public class GenreMapperTests
    {
        [TestMethod]
        public void MappingTest()
        {
            var pluginConfiguration = new PluginConfiguration()
            {
                GenreMappings = new Dictionary<String, List<String>>()
                {
                    { GenreMapper.GENRE_MOVIE, new List<string>() { "Movie", "Film" } },
                    { GenreMapper.GENRE_SPORT, new List<string>() { "Sport", "Football" } },
                }
            };

            var target = new GenreMapper(pluginConfiguration);

            // Test movie
            var movie = new ProgramInfo() { Genres = new List<String>() { "Movie", "Drama" }};
            target.PopulateProgramGenres(movie);
            Assert.IsTrue(movie.IsMovie);
            Assert.IsFalse(movie.IsSports);

            var match = new ProgramInfo() { Genres = new List<String>() { "Sport", "Football" } };
            target.PopulateProgramGenres(match);
            Assert.IsFalse(match.IsMovie);
            Assert.IsTrue(match.IsSports);
        }
    }
}
