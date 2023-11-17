using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ZXClient.model
{
    [XmlRoot("root")]
    public class EvalBtnModel
    {
        [XmlElement("button")]
        public List<AttrButton> Buttons { get; set; }
    }

    [XmlRoot("button")]
    public class AttrButton
    {
        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("img")]
        public string Img { get; set; }
    }
}
