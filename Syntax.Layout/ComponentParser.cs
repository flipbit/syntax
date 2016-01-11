using System;
using System.Collections.Generic;
using Syntax.Layout.Components;

namespace Syntax.Layout
{
    public class ComponentParser
    {
        /// <summary>
        /// Parses the given layout.
        /// </summary>
        /// <param name="layout">Layout.</param>
        public IList<Component> Parse(string layout)
        {
            var results = new List<Component>();

            if (!string.IsNullOrEmpty(layout))
            {
                var lines = layout.Split('\n');

                Component current = null;

                foreach (var line in lines)
                {
                    var name = line.SubstringBeforeString(":").Trim().ToLower();
                    var value = line.SubstringAfterString(":").Trim();

                    switch (name)
                    {
                    case "- type":
                        current = new Label();
                        break;

                    case "id":
                        current.Id = value;
                        break;

                    case "text":
                        ((Label) current).Text = value;
                        break;
                    }
                }

                results.Add(current);
            }

            return results;
        }
    }
}

