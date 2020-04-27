namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Response model for files
    /// </summary>
    public class FileResponseModel
    {
        /// <summary>Represents the identifier of the file</summary>
        public string Id { get; set; }

        /// <summary>File's name</summary>
        public string Name { get; set; }

        /// <summary>File's type</summary>
        public string Type { get; set; }

        /// <summary>File's BlobUrl</summary>
        public string BlobUrl { get; set; }
    }
}
