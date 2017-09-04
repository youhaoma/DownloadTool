using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces
{

    /// <summary>
    /// The global configuration of the software
    /// </summary>
    public  interface ILocalConfigInfo
    {

        /// <summary>
        ///  The Theme of this software
        /// </summary>
        string Theme { get; set; }

        /// <summary>
        /// The background of this software
        /// </summary>
        string Background { get; set; }

        /// <summary>
        ///  The software's language 
        /// </summary>
        LanguageEnum Language { get; set; }

        /// <summary>
        /// whether a dialog box is displayed or not while downloading;
        /// </summary>
        bool IsDisplayDownloadDialog { get; set; }

        /// <summary>
        /// The Downlaod Path
        /// </summary>
        string DownloadDictionary { get; set; }


        /// <summary>
        ///  The number of download Task At the same time
        /// </summary>
        int ParalleTaskNum { get; set; }


        /// <summary>
        ///  Maxmum Download Speed 
        /// </summary>
        double SpeedLimited { get; set; }

        /// <summary>
        /// Persistents data;
        /// </summary>
        void Save();

    }
}
