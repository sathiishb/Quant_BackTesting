using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Quant.BackTesting.Test
{
    [TestClass]
    public class FileRenameUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var path = @"C:\Satz\Market-Data\BackTesting\BN Historical Data\2019\Expiry 25th April\csv";
            //* is strike # CE or PE
            //Weekly
            //string format = "BANKNIFTYWK**##";

            //Monthly
            string format = "BANKNIFTY**##";
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] infos = d.GetFiles();
            foreach (FileInfo f in infos)
            {
                var oldFileName = Path.GetFileNameWithoutExtension(f.Name);
                //string newfileName = FileNameWithSpace(format, oldFileName);
                string newfileName = FileNameWithDate(oldFileName);

                File.Move(f.FullName, f.FullName.Replace(oldFileName, newfileName));
            }
        }

        private static string FileNameWithDate(string oldFileName)
        {
            var aStringBuilder = new StringBuilder(oldFileName);
            aStringBuilder.Remove(9, 7);
            //aStringBuilder.Insert(9, "WK");
            var newFileName = aStringBuilder.ToString();
            return newFileName;
        }

        private static string FileNameWithSpace(string format, string oldFileName)
        {
            var fileInformation = oldFileName.Split(' ');
            var newfileName = format.Replace("**", fileInformation[1])
                .Replace("##", fileInformation[0]);
            return newfileName;
        }
    }
}
