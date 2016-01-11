using System;
using System.IO;

namespace Syntax.Layout
{
    public class ResourceReader
    {
        public string Read(object target)
        {
            var name = target.GetType().Name.Replace("Controller", string.Empty);

            var path = string.Format("../../Resources/Layouts/{0}.layout", name);

            var json = File.ReadAllText(path);

            return json;
        }

        public string Read(string name)
        {
            var path = string.Format("../../Resources/Layouts/{0}.layout", name);

            var json = File.ReadAllText(path);

            return json;
        }
    }
}

