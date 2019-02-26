using System.Collections;
using System.Web;

namespace SFA.DAS.StuctureMap.Extensions
{
    public class NullHttpContext : HttpContextBase
    {
        private readonly Hashtable _items;

        public NullHttpContext()
        {
            _items = new Hashtable
            {
                {"Nested.Container.Key", null}
            };
        }

        public override IDictionary Items => _items;
    }
}
