using System;
using System.Collections.Generic;
using System.Text;

namespace CorrectBin.Model
{
    public class ImageInfo
    {
        public List<Categories> Categories { get; set; }
        public Description Description { get; set; }
    }

    public class Categories
    {
        public string Name { get; set; }
        public double Score { get; set; }
    }

    public class Description
    {
        public List<string> Tags { get; set; }
        public List<Captions> Captions { get; set; }
    }

    public class Captions
    {
        public string Text { get; set; }
        public double Confidence { get; set; }
    }
}
