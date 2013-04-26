namespace Website.Mvc.Models
{
    public class ImageModel
    {
        public string   MediaId     { get; set; }
        public int      ImageWidth  { get; set; }
        public int      ImageHeight { get; set; }
        public bool     Constrain   { get; set; }
        public string   BgColor     { get; set; }
        public string   AltText     { get; set; }
        public string   ImageClass  { get; set; }
        public bool     FloatImage  { get; set; }

        // Constructor - set defaults
        public ImageModel()
        {
            ImageWidth = 0;
            ImageHeight = 0;
            Constrain = false;
        }

    }

    

    // TODO ImageModel constructor

}