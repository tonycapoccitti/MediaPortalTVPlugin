using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Entities;
using System.Linq;

namespace MediaPortalTVPlugin.Entities
{
    /// <summary>
    /// Class TrailerCollectionFolder
    /// </summary>
    class MyPluginCollectionFolder : BasePluginFolder
    {
        public MyPluginCollectionFolder()
        {
            Name = "MediaPortalTVPlugin";
        }

    }

    /// <summary>
    /// Class PluginFolderCreator
    /// </summary>
    public class PluginFolderCreator : IVirtualFolderCreator
    {
        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <returns>BasePluginFolder.</returns>
        public BasePluginFolder GetFolder()
        {
            return new MyPluginCollectionFolder
            {
                Id = "MediaPortalTVPlugin".GetMBId(typeof(MyPluginCollectionFolder)),
            };

        }
    }
}
