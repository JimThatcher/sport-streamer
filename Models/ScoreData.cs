namespace WebAPI.Models
{
    public class ScoreData
    {
        public ScoreData() {
            id = 1;
            Pd = 1;
            Hs = Gs = 0;
            Hpo = Gpo = false;
            Htol = Gtol = 3;
            BO = 0;
            Dn = 1;
            Dt = 10;
            Hto = Gto = false;
            Fl = false;
        }
        public long id { get; set; }
        public int Pd { get; set; }
        public int Hs { get; set; }
        public int Gs { get; set; }
        public bool Hpo { get; set; }
        public bool Gpo { get; set; }
        public int Htol { get; set; }
        public int Gtol { get; set; }
        public int BO { get; set; }
        public int Dn { get; set; }
        public int Dt { get; set; }
        public bool Hto { get; set; }
        public bool Gto { get; set; }
        public bool Fl { get; set; }
        public string asJson() {
            return "{\"id\":" + id + ",\"Pd\":" + Pd + ",\"Hs\":" + Hs + ",\"Gs\":" + Gs + 
                ",\"Hpo\":\"" + Hpo + "\",\"Gpo\":\"" + Gpo + "\",\"Htol\":\"" + Htol + "\",\"Gtol\":\"" + Gtol + 
                "\",\"BO\":" + BO + ",\"Dn\":" + Dn + ",\"Dt\":" + Dt + ",\"Hto\":\"" + Hto + 
                "\",\"Gto\":\"" + Gto + "\",\"Fl\":\"" + Fl + "\"}";
        }
    }
}