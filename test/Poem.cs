using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using test.Exceptions;

namespace test
{
    public class Poem
    {
        private readonly Graph graph;

        public Poem(Graph graph){
            this.graph = graph;
        }

        public string Generate(){
            int counter1 = 0;
            int counter2 = 0;
            bool retry = false;
            string line1 = "";
            string line2 = "";
            do
            {
                try
                {
                    line1 = makeLine();
                    retry = false;
                }
                catch(NotEnoughtVertexToGenerateLine)
                {
                    retry = true;
                    counter1++;
                }

                if(!retry)
                {
                    do
                    {
						try
						{
                            line2 = makeLine();
                            if(isRhyme(line1, line2))
                            {
								retry = false;
                            } else {
                                retry = true;
                                counter2++;
                            }
						}
						catch (NotEnoughtVertexToGenerateLine)
						{
							retry = true;
							counter2++;
						}
                        //Console.WriteLine(counter2);
                    }
                    while (retry && (counter2 < 20));
                }
                //Console.WriteLine(counter1);
            }
            while (retry && counter1 < 10);

            return line1 + "\n" + line2;
        }

        private string makeLine()
        {
            StringBuilder builder = new StringBuilder();
            int syllabsPerLine = 13;
            Stack path = new Stack();
            List<int> usedVertex = new List<int>();
            int syllable = 0;
            int counter = 0;

            while (syllable != syllabsPerLine /*&& counter < 200*/)
            {
                counter++;
                if (syllable == 0)
                {
                    int vertex = 0;
                    int trying = 0;
                    do
                    {
                        trying++;
                        vertex = graph.randomFirstLevelVertex();
                    } while (!validVertex(usedVertex, vertex) && trying < graph.NodesName.Count);

                    if (validVertex(usedVertex, vertex))
                    {
                        path.Push(vertex);
                        usedVertex.Add(vertex);
                        syllable += countSyllabs(graph.getNameOfVertex(vertex));
                    }
                    else
                    {
                        throw new NotEnoughtVertexToGenerateLine();
                    }

                }
                else if (path.Count > 0 && syllable < syllabsPerLine)
                {
                    int vertex = 0;
                    int trying = 0;
                    int lastVertex = getLsatVertex(path);
                    do
                    {
                        trying++;
                        vertex = graph.randomNeighbourVertex(lastVertex);
                    } while (!validVertex(usedVertex, vertex) && trying < graph.countNeihbours(vertex));

                    if (validVertex(usedVertex, vertex))
                    {
                        path.Push(vertex);
                        usedVertex.Add(vertex);
                        syllable += countSyllabs(graph.getNameOfVertex(vertex));
                    }
                    else
                    {
                        lastVertex = (int)path.Pop();
                        syllable -= countSyllabs(graph.getNameOfVertex(lastVertex));
                    }
                }
                else
                {
                    if (path.Count > 0)
                    {
                        int lastVertex = (int)path.Pop();
                        syllable -= countSyllabs(graph.getNameOfVertex(lastVertex));
                    }
                    else
                    {
                        throw new NotEnoughtVertexToGenerateLine();
                    }
                }
            }

            if (syllable != syllabsPerLine) throw new NotEnoughtVertexToGenerateLine();
                
            return buildLineFromPath(path);
        }

        private bool isRhyme(string a, string b)
        {
            string[] words;
            words = a.Split(' ');
            a = words[words.Length - 1];
            words = b.Split(' ');
            b = words[words.Length - 1];

            Stack stackA = new Stack();
            Stack stackB = new Stack();
            List<char> vowels = new List<char>(); 
            char[] items = { 'a', 'ą', 'e', 'ę', 'i', 'o', 'ó', 'u', 'y' };

            foreach (char item in items)
            {
                vowels.Add(item);
            }

            foreach(char item in a)
            {
                stackA.Push(item);
            }

            foreach(char item in b)
            {
                stackB.Push(item);
            }

			char charA, charB;
            while(stackA.Count > 0 && stackB.Count > 0)
            {
                charA = (char)stackA.Pop();
                charB = (char)stackB.Pop();
                if(charA == charB)
                {
                    if (vowels.Contains(charA)) return true;
                }
                else break;
            }

            return false;
        }

        private int getLsatVertex(Stack path){
            int vertex = (int)path.Pop();
            path.Push(vertex);
            return vertex;
        }

        private bool validVertex(List<int> usedVertex, int vertex){
            if (usedVertex.Contains(vertex) || !graph.hasNeighbour(vertex)){
                return false;
            } else {
                return true;
            }
        }

        private int countSyllabs(string word){
            char[] items = { 'a', 'ą', 'e', 'ę', 'i', 'o', 'ó', 'u', 'y' };
            int counter = 0;
            foreach(char letter in word){
                foreach(char item in items){
                    if (letter == item) counter++;
                }
            }
            return counter;
        }

        private string buildLineFromPath(Stack path)
        {
            Stack revesPath = new Stack();
            Stack copyPath = (Stack)path.Clone();
            StringBuilder builder = new StringBuilder();

            while(copyPath.Count > 0)
            {
                revesPath.Push(copyPath.Pop());
            }
            while (revesPath.Count > 0)
            {
                int key = (int)revesPath.Pop();
                builder.Append(graph.NodesName[key]);
                if(revesPath.Count > 0) builder.Append(" ");
            }

            return builder.ToString();
        }

    }
}
