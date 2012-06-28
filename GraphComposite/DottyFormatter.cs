using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Digraph
{
    public class DottyGraphBuilder
    {
        GraphCompositeBuilder<string, Attributed> _builder;

        public DottyGraphBuilder(string rootKey)
        {
            _builder = new GraphCompositeBuilder<string, Attributed>(StringComparer.InvariantCultureIgnoreCase, rootKey, new Attributed("label", rootKey));
        }

        public void addVertex(string containingKey, string thisKey)
        {
            Attributed val = new Attributed("label", thisKey);
            _builder.add(containingKey, thisKey, val, false);
        }

        public void addSubgraph(string containingKey, string thisKey)
        {
            Attributed val = new Attributed("label", thisKey);
            _builder.add(containingKey, thisKey, val, true);
        }

        public void addEdge(string fromKey, string toKey)
        {
            _builder.edge(fromKey, toKey);
        }

        public GraphComposite<string, Attributed> generateCopy()
        {
            return _builder.generateCopy();
        }

    }

    public class DottyFormatter
    {
        private bool _useHashCodeForId = false;

        private Attributed _mainGraphAttribs;
        private Attributed _subgraphAttribs;
        private Attributed _mainGraphNodeAttribs;
        private Attributed _subgraphNodeAttribs;

        private StreamWriter _sw;

        List<KeyValuePair<GraphComposite<string, Attributed>, GraphComposite<string, Attributed>>> _discoveredEdges
             = new List<KeyValuePair<GraphComposite<string, Attributed>, GraphComposite<string, Attributed>>>();
        int _depth = 0;

        #region properties
        public bool useHashCodeForId
        {
            set
            {
                _useHashCodeForId = value;
            }
        }

        public Attributed mainGraphAttribs
        {
            get
            {
                return _mainGraphAttribs;
            }
        }

        public Attributed subgraphAttribs
        {
            get
            {
                return _subgraphAttribs;
            }
        }

        public Attributed mainGraphNodeAttribs
        {
            get
            {
                return _mainGraphNodeAttribs;
            }
        }

        public Attributed subgraphNodeAttribs
        {
            get
            {
                return _subgraphNodeAttribs;
            }
        }
        #endregion properties

        public DottyFormatter()
        {
            clearAttribs();
        }

        public void dottyPrint(GraphComposite<string, Attributed> c, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            _sw = File.CreateText(filePath);

            _depth = 0;
            _discoveredEdges.Clear();

            GraphCompositeTraversal<string, Attributed>.traverseSubgraphs(c, printVertex, openGraphSection, closeGraphSection);

            _sw.Close();
        }

        private void printVertex(GraphComposite<string, Attributed> c)
        {
            string nodeId;
            string indent = "";

            if (_useHashCodeForId)
                nodeId = Convert.ToString(c.GetHashCode());
            else
                nodeId = c.value.attribs["label"];

            for (int i = 0; i < _depth; i++)
                indent += "   ";

            foreach (GraphComposite<string, Attributed> dest in c.outgoing)
                _discoveredEdges.Add(
                    new KeyValuePair<GraphComposite<string, Attributed>, GraphComposite<string, Attributed>>(c, dest));

            if (!c.IsGraph)
                _sw.WriteLine(indent + nodeId + "[label=\"" + convertLabelForDotty(c.value.attribs["label"]) + "\"];");
        }

        private void openGraphSection(GraphComposite<string, Attributed> c)
        {
            string id;
            string val;
            string indent = "";

            if (_useHashCodeForId)
                val = Convert.ToString(c.GetHashCode());
            else
                val = c.value.attribs["label"];

            for (int i = 0; i < _depth; i++)
                indent += "   ";

            if (_depth == 0)
            {
                _sw.WriteLine("digraph cluster_" + val + " {");
                _sw.WriteLine("   compound=true;");
                foreach (string key in _mainGraphNodeAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   node [" + key + "=" + _mainGraphNodeAttribs[key] + "];");
                foreach (string key in _mainGraphAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   " + key + "=" + _mainGraphAttribs[key] + ";");
            }
            else
            {
                _sw.WriteLine(indent + "subgraph cluster_" + val + " {");
                foreach (string key in _subgraphNodeAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   node [" + key + "=" + _subgraphNodeAttribs[key] + "];");
                foreach (string key in _subgraphAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   " + key + "=" + _subgraphAttribs[key] + ";");
            }

            _sw.WriteLine(indent + "   " + "label=\"" + convertLabelForDotty(c.value.attribs["label"]) + "\";");
            // add a dotty node that will act as the edge snap-point for this subgraph
            _sw.WriteLine(indent + "   " + val + " [style=invis];");

            _depth++;
        }

        private void closeGraphSection(GraphComposite<string, Attributed> c)
        {
            _depth--;

            // if we're closing the main graph, print all the edges
            if (_depth == 0)
            {
                foreach (KeyValuePair<GraphComposite<string, Attributed>, GraphComposite<string, Attributed>> edge
                         in _discoveredEdges)
                {
                    string start, end;
                    if (_useHashCodeForId)
                    {
                        start = Convert.ToString(edge.Key.GetHashCode());
                        end = Convert.ToString(edge.Value.GetHashCode());
                    }
                    else
                    {
                        start = edge.Key.value.attribs["label"];
                        end = edge.Value.value.attribs["label"];
                    }
                    _sw.WriteLine("   " + start + " -> " + end + addTailHeadInfoForEdge(edge.Key, edge.Value) + ";");
                }
            }

            string indent = "";
            for (int i = 0; i < _depth; i++)
                indent += "   ";

            _sw.WriteLine(indent + "}");
        }

        private string convertLabelForDotty(string id)
        {
            string result = "";
            foreach (char c in id.ToCharArray())
            {
                // convert escape characters
                if (c == '\\')
                    result += "/";
                else
                    result += c;
            }
            return result;
        }

        private string addTailHeadInfoForEdge(GraphComposite<string, Attributed> start, GraphComposite<string, Attributed> end)
        {
            string result = "";
            string c1Id, c2Id;

            if (_useHashCodeForId)
            {
                c1Id = Convert.ToString(start.GetHashCode());
                c2Id = Convert.ToString(end.GetHashCode());
            }
            else
            {
                c1Id = start.value.attribs["label"];
                c2Id = end.value.attribs["label"];
            }

            if (start.IsGraph)
            {
                result += " [ltail=cluster_" + c1Id;
                if (end.IsGraph)
                    result += ", lhead=cluster_" + c2Id;
                result += "]";
            }
            else if (end.IsGraph)
            {
                result += " [lhead=cluster_" + c2Id + "]";
            }
            return result;
        }

        public void clearAttribs()
        {
            _mainGraphAttribs = new Attributed();
            _subgraphAttribs = new Attributed();
            _mainGraphNodeAttribs = new Attributed();
            _subgraphNodeAttribs = new Attributed();
        }
    }

    /*
    public class DottyFormatter
    {
        private bool _useHashCodeForId = false;

        private Attributed _mainGraphAttribs;
        private Attributed _subgraphAttribs;
        private Attributed _mainGraphNodeAttribs;
        private Attributed _subgraphNodeAttribs;

        private StreamWriter _sw;

        private int _depth; // recursion depth, for indentating output

        #region properties
        public bool useHashCodeForId
        {
            set
            {
                _useHashCodeForId = value;
            }
        }

        public Attributed mainGraphAttribs
        {
            get
            {
                return _mainGraphAttribs;
            }
        }

        public Attributed subgraphAttribs
        {
            get
            {
                return _subgraphAttribs;
            }
        }

        public Attributed mainGraphNodeAttribs
        {
            get
            {
                return _mainGraphNodeAttribs;
            }
        }

        public Attributed subgraphNodeAttribs
        {
            get
            {
                return _subgraphNodeAttribs;
            }
        }
        #endregion properties

        public DottyFormatter()
        {
            clearAttribs();
        }

        public void dottyPrint(DigraphComposite c, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            _sw = File.CreateText(filePath);

            _depth = 0;

            List<DigraphComposite> visited = new List<DigraphComposite>();
            c.traverse(printVertex, openGraphSection, closeGraphSection);

            _sw.Close();
        }

        private void printVertex(DigraphComposite c)
        {
            string thisId;
            string nodeId;
            string indent = "";

            if (!c.attribs.TryGetValue("id", out thisId))
                throw new DigraphCompositeBuilder.VertexMissingIdException(c);

            if (_useHashCodeForId)
                nodeId = Convert.ToString(c.GetHashCode());
            else
                nodeId = thisId;

            for (int i = 0; i < _depth; i++)
                indent += "   ";

            if (!c.isAGraph)
                _sw.WriteLine(indent + nodeId + "[label=\"" + convertLabelForDotty(thisId) + "\"];");
        }

        private void openGraphSection(DigraphComposite c)
        {
            string id;
            string val;
            string indent = "";

            if (!c.attribs.TryGetValue("id", out id))
                throw new DigraphCompositeBuilder.VertexMissingIdException(c);

            if (_useHashCodeForId)
                val = Convert.ToString(c.GetHashCode());
            else
                val = id;

            for (int i = 0; i < _depth; i++)
                indent += "   ";

            if (_depth == 0)
            {
                _sw.WriteLine("digraph cluster_" + val + " {");
                _sw.WriteLine("   compound=true;");
                foreach (string key in _mainGraphNodeAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   node [" + key + "=" + _mainGraphNodeAttribs[key] + "];");
                foreach (string key in _mainGraphAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   " + key + "=" + _mainGraphAttribs[key] + ";");
            }
            else
            {
                _sw.WriteLine(indent + "subgraph cluster_" + val + " {");
                foreach (string key in _subgraphNodeAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   node [" + key + "=" + _subgraphNodeAttribs[key] + "];");
                foreach (string key in _subgraphAttribs.attribs.Keys)
                    _sw.WriteLine(indent + "   " + key + "=" + _subgraphAttribs[key] + ";");
            }

            _sw.WriteLine(indent + "   " + "label=\"" + convertLabelForDotty(id) + "\";");
            // add a dotty node that will act as the edge snap-point for this subgraph
            _sw.WriteLine(indent + "   " + val + " [style=invis];");

            _depth++;
        }

        private void closeGraphSection(DigraphComposite c)
        {
            _depth--;

            // if we're closing the main graph, print all the edges
            if (_depth == 0)
            {
                List<DigraphEdge> l = c.getListOfEdges();

                foreach (DigraphEdge e in l)
                {
                    string startVal, endVal;
                    if (_useHashCodeForId)
                    {
                        startVal = Convert.ToString(e.startVertex.GetHashCode());
                        endVal = Convert.ToString(e.endVertex.GetHashCode());
                    }
                    else if (!e.startVertex.attribs.TryGetValue("id", out startVal))
                        throw new DigraphCompositeBuilder.VertexMissingIdException(e.startVertex as DigraphComposite);
                    else if (!e.endVertex.attribs.TryGetValue("id", out endVal))
                        throw new DigraphCompositeBuilder.VertexMissingIdException(e.endVertex as DigraphComposite);

                    _sw.WriteLine(
                        "   " + startVal + " -> " + endVal + addTailHeadInfoForEdge(e) + ";");
                }
            }

            string indent = "";
            for (int i = 0; i < _depth; i++)
                indent += "   ";

            _sw.WriteLine(indent + "}");
        }

        private string convertLabelForDotty(string id)
        {
            string result = "";
            foreach (char c in id.ToCharArray())
            {
                // convert escape characters
                if (c == '\\')
                    result += "/";
                else
                    result += c;
            }
            return result;
        }

        private string addTailHeadInfoForEdge(DigraphEdge e)
        {
            string result = "";
            DigraphComposite c1 = e.startVertex as DigraphComposite;
            DigraphComposite c2 = e.endVertex as DigraphComposite;
            string c1Id, c2Id;
            if (_useHashCodeForId)
            {
                c1Id = Convert.ToString(c1.GetHashCode());
                c2Id = Convert.ToString(c2.GetHashCode());
            }
            else if (!c1.attribs.TryGetValue("id", out c1Id))
                throw new DigraphCompositeBuilder.VertexMissingIdException(c1);
            else if (!c2.attribs.TryGetValue("id", out c2Id))
                throw new DigraphCompositeBuilder.VertexMissingIdException(c2);
            if (c1.isAGraph)
            {
                result += " [ltail=cluster_" + c1Id;
                if (c2.isAGraph)
                    result += ", lhead=cluster_" + c2Id;
                result += "]";
            }
            else if (c2.isAGraph)
            {
                result += " [lhead=cluster_" + c2Id + "]";
            }
            return result;
        }

        public void clearAttribs()
        {
            _mainGraphAttribs = new Attributed();
            _subgraphAttribs = new Attributed();
            _mainGraphNodeAttribs = new Attributed();
            _subgraphNodeAttribs = new Attributed();
        }
    }*/
}
