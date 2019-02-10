namespace Abschlussarbeit
{
    partial class Stundenplan
    {
        public class Zeit
        {
            private string z_name;
            private string z_tag;
            private string z_beginn;
            public string getZeitName()
            {
                return z_name;
            }
            public string getZeitWochentag()
            {
                return z_tag;
            }
            public string getZeitBeginn()
            {
                return z_beginn;
            }
            public void setZeitName(string neuName)
            {
                z_name = neuName;
            }
            public void setZeitWochentag(string neuTag)
            {
                z_tag = neuTag;
            }
            public void setZeitBeginn(string neuBeginn)
            {
                z_beginn = neuBeginn;
            }
        }
    }
}