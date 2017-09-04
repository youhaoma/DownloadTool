using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure
{
    public class FileLocation
    {
        /// <summary>
        ///  Gets a regular expression for splitting the file full path string
        ///  In the right case, the group will have four elements;
        ///  [0] : FullPath
        ///  [1] : Folder Name
        ///  [2] : File Name
        ///  [3] : File Extension
        /// </summary>

        private static readonly Regex RegexFileLocation = new Regex(
            @"^([\\/]?(?:\w:)?(?:[^\\/]+?[\\/]*?)([^\\/]+?(?:\.(\w+?))?))?$",
            RegexOptions.Compiled);

        /// <summary>
        ///  Gets a string representing the full path of file
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Gets a string representing the folder where the file is located
        /// </summary>
        public string FolderPath { get; }

        /// <summary>
        ///  Gets a string representing the file name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets a string representing the file extension.
        /// </summary>
        public string FileExtension { get; }



        public FileLocation(string fileFullPath)
        {

            Debug.WriteLine($"{DateTime.Now} : {fileFullPath}");

            if (fileFullPath == "/" || string.IsNullOrEmpty(fileFullPath))
            {
                return;
            }
            //var matchResult = RegexFileLocation.Match(fileFullPath);

            //if (matchResult.Groups == null || matchResult.Groups.Count != 4)
            //{
            //    return;
            //    //throw new ArgumentException($"The file path is not valid : {fileFullPath}", fileFullPath);
            //}

            FullPath = fileFullPath;
            //var temp = matchResult.Groups[1].Value;

            //if (!string.IsNullOrEmpty(temp))
            //    FolderPath = temp.Remove(temp.Length - 1);  // Remove the "\" or "/"at the end

            string strPath = fileFullPath.Clone().ToString();
            string[] names = strPath.Split('/');
            FileName = names[names.Length -1];

            string[] exNames = names[names.Length - 1].Split('.');
            if (exNames.Length <= 0)
            {
                FileExtension = "";
            }
            FileExtension = exNames[exNames.Length - 1].ToLower();

        }


        public override string ToString()
        {
            return FullPath;
        }




    }
}
