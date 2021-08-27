using System.Xml.Serialization;

namespace Common.model
{
    [XmlRoot("employInfoSet")]
    public class InfoSetModel
    {
        [XmlElement("employeeName")]
        public bool employeeName;
        [XmlElement("job_desc")]
        public bool job_desc;
        [XmlElement("employeeJobNum")]
        public bool employeeJobNum;
        [XmlElement("star")]
        public bool star;
        [XmlElement("employeeCardNum")]
        public bool employeeCardNum;
        [XmlElement("phone")]
        public bool phone;
        [XmlElement("windowName")]
        public bool windowName;
        [XmlElement("deptname")]
        public bool deptname;
        [XmlElement("unitName")]
        public bool unitName; 
    }
}
