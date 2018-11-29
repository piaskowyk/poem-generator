using System;
using System.IO;
using System.Data;

namespace test
{
    public class Mleko
    {
        public Mleko()
        {
        }

        public static void Main()
        {
            Console.WriteLine("Start:");

            if(false){
				string[] text = File.ReadAllLines("/home/mleko/Desktop/book.txt");
				TextToDB analisator = new TextToDB();
				analisator.analiseText(text);
            } else {
                Graph graph = new Graph();
                Poem poem = new Poem(graph);
                string poemText = poem.Generate();
                Console.WriteLine(poemText);
                //File.WriteAllText("/home/mleko/Desktop/poem.txt", poemText);
            }
        }
    }
}
