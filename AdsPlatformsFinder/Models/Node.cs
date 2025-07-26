namespace AdsPlatformsFinder.Models
{
    public class Node
    {
        public new List<string> TitleLocation { get; set; } = new List<string>();
        public string PathValue { get; set; }

        public List<Node> Nodes { get; set; } = new List<Node>();

        public Node(bool isRoot = false)
        {
            if (isRoot)
            {
                PathValue = "global";
            }
        }

        public void FindNextChild(string location, List<string> path, List<string> path2, int index)
        {
            var nextRoot = Nodes.Find(x => x.PathValue == path[index]);
            if (nextRoot == null)
            {
                nextRoot = nextRoot ?? new Node();
                Nodes.Add(nextRoot);
            }
            nextRoot.Processing(location, path2);
        }

        public Node Processing(string location, List<string> path)
        {
            if (path.Count != 0)
            {
                if (PathValue == "global")
                {
                    FindNextChild(location, path, path, 0);
                }
                else if (path.Count > 1)
                {
                    FindNextChild(location, path, path.GetRange(1, path.Count - 1), 1);
                    PathValue = path[0];
                }
                else
                {
                    TitleLocation.Add(location);
                    PathValue = path[0];
                }
            }

            return this;
        }

        public List<string> GetTitles(List<string> path)
        {
            var titles = new List<string>();
            if (path.Count > 0)
            {
                var node = Nodes.Find(x => x.PathValue == path[0]);

                if (node != null)
                {
                    titles.AddRange(node.TitleLocation);
                    titles.AddRange(node.GetTitles(path.GetRange(1, path.Count - 1)));
                }
            }

            return titles;
        }
    }
}