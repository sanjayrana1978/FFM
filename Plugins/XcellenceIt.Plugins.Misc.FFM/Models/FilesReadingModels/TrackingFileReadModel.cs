namespace XcellenceIt.Plugins.Misc.FFM.Models.FilesReadingModels
{
    /// <summary>
    /// Represents an Tracking FileRead Model
    /// </summary>
    public class TrackingFileReadModel
    {
        public string Invoice { get; set; }
        public string ShipToName { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCityAndState { get; set; }
        public string ShipToZipCode { get; set; }
        public string Zip { get; set; }
        public string Blank { get; set; }
        public string PONumber { get; set; }
        public string A { get; set; }
        public string UNFIOrder { get; set; }
        public string DateShipped { get; set; }
        public string[] TrackingValue { get; set; }

    }
}
