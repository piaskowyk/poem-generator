using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace test
{
    public class Graph
    {
        private readonly Dictionary<int, List<int>> neighbourList = new Dictionary<int, List<int>>();
        private readonly Dictionary<int, string> nodeNames = new Dictionary<int, string>();
        private readonly List<int> mapIndex = new List<int>();
        private readonly Database db = new Database();
        private readonly Random random = new Random();

        public Dictionary<int, List<int>> NeighbourList{
            get { return this.neighbourList; }
        }

        public Dictionary<int, string> NodesName{
            get { return this.nodeNames; }
        }

        public Graph(){
            BuildGraph();
        }

        public int getVertexByIndex(int index){
            return mapIndex[index];
        }

        public int getNeighbouByIndex(int key, int index){
            return getFromNeighbourList(neighbourList[key], index);
        }

        public bool hasNeighbour(int key){
            return neighbourList[key].Count > 0;
        }

        public int randomFirstLevelVertex(){
            int vertex;
            int index;
            do
            {
                index = random.Next(0, nodeNames.Count - 1);
                vertex = getVertexByIndex(index);
            } while (!hasNeighbour(vertex));

            return vertex;
        }

        public int randomNeighbourVertex(int vertex){
            int index = random.Next(0, neighbourList[vertex].Count - 1);
            return getVertexByIndex(index);
        }

        public int countNeihbours(int vertex){
            return neighbourList[vertex].Count;
        }

        public string getNameOfVertex(int vertex){
            return nodeNames[vertex];
        }

        private int getFromNeighbourList(List<int> neighbour, int index){
            return neighbour[index];
        }

        private void BuildGraph(){
            List<int> allNodesID = new List<int>();
            DataSet ds = db.query("select * from nodes");
            DataTable dt = ds.Tables["data"];
            foreach (DataRow dr in dt.Rows){
                int id = Convert.ToInt32(dr["id"]);
                allNodesID.Add(id);
                nodeNames.Add(id, dr["name"].ToString());
                neighbourList.Add(id, new List<int>());
                mapIndex.Add(id);
            }

            ds = db.query("select * from edges");
            dt = ds.Tables["data"];
            foreach(DataRow dr in dt.Rows){
                int a = Convert.ToInt32(dr["a"]);
                int b = Convert.ToInt32(dr["b"]);
                neighbourList[a].Add(b);
            }
        }

    }
}
