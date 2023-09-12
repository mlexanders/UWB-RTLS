using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWB.Models
{
    public class JsonParseLink
    {
        public string A;
        public float R;
    }
    public class JsonParseLinks
    {
        public string Id;
        public JsonParseLink[] Links;
    }
}
