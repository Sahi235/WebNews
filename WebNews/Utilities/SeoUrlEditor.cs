using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public class SeoUrlEditor : ISeoUrlEditor
    {
        public string FixSeoUrl(string seoUrl)
        {
            char[] a = new char[] { ' ', '?', '/'};

            StringBuilder stringBuilder = new StringBuilder();
            bool shouldChange;
            bool previous = false;
            for (int i = 0; i < seoUrl.Length; i++)
            {
                shouldChange = false;
                foreach (var c in a)
                {
                    if (seoUrl[i] == c || seoUrl[i].ToString() == "'")
                    {
                        shouldChange = true;
                    }
                }
                if (!shouldChange) stringBuilder.Append(seoUrl[i]);
                else if ((shouldChange) && (previous != true) && !(i == seoUrl.Length - 2)) stringBuilder.Append('-');
                previous = shouldChange;
            }
            return stringBuilder.ToString();
        }
    }
}
