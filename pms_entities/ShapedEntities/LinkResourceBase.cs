using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.ShapedEntities
{
    public class LinkResourceBase
    {
        public List<Link> Links { get; set; } = new List<Link>();

        public LinkResourceBase()
        {

        }
    }
}
