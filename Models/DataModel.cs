using System;
using System.Text;

namespace Models
{
    public class DataModel
    {
        public DataModel()
        { }

        public DataModel(
            string firstName,
            string lastName,
            string city,
            string postCode,
            string appVersion,
            string email,
            string music,
            string performer,
            int year,
            int hour,
            string md5Hash,
            string fileName,
            string fileType,
            DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            City = city;
            PostCode = postCode;
            AppVersion = appVersion;
            Email = email;
            Music = music;
            Performer = performer;
            Year = year;
            Hour = hour;
            MD5Hash = md5Hash;
            FileName = fileName;
            FileType = fileType;
            Date = date;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string AppVersion { get; set; }

        public string Email { get; set; }

        public string Music { get; set; }

        public string Performer { get; set; }

        public int Year { get; set; }

        public int Hour { get; set; }

        public string MD5Hash { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public DateTime Date { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"FirstName: {this.FirstName}");
            builder.AppendLine($"LastName: {this.LastName}");
            builder.AppendLine($"City: {this.City}");
            builder.AppendLine($"PostCode: {this.PostCode}");
            builder.AppendLine($"AppVersion: {this.AppVersion}");
            builder.AppendLine($"Email: {this.Email}");
            builder.AppendLine($"Music: {this.Music}");
            builder.AppendLine($"Performer: {this.Performer}");
            builder.AppendLine($"Year: {this.Year}");
            builder.AppendLine($"Hour: {this.Hour}");
            builder.AppendLine($"MD5Hash: {this.MD5Hash}");
            builder.AppendLine($"FileName: {this.FileName}");
            builder.AppendLine($"FileType: {this.FileType}");
            builder.AppendLine($"Date: {this.Date}");

            return builder.ToString();
        }
    }
}
