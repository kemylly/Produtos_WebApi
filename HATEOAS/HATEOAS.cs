using System.Collections.Generic;

namespace estudo_api.HATEOAS
{
    public class HATEOAS
    {
        private string url; //url para chegar até o controller
        private string protocol = "https://";
        public List<Link> actions = new List<Link>();  //acoes na lista que vem do controller

        public HATEOAS(string url)
        {
            this.url = url;
        }

        public HATEOAS(string url, string protocol)
        {
            this.url = url;
            this.protocol = protocol;
        }

        public void AddAction(string rel, string method)
        {
            //https:// localhost:5001/api/v1/Produtos"
            actions.Add(new Link(this.protocol + this.url,rel,method));
        }

        public Link[] GetActions(string sufix)
        {
            Link[] tempLinks = new Link[actions.Count];
            
            for(int i=0; i < tempLinks.Length;i++)
            {
                tempLinks[i] = new Link(actions[i].href, actions[i].rel, actions[i].method);
            }

            /* montagem do link */
            foreach(var link in tempLinks){
                // https:// localhost:5001/api/v1/Produtos/ 2/32/kemylly
                link.href = link.href+"/"+sufix;
            }
            return tempLinks;
        }
    }
}