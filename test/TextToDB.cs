using System;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace test
{
    public class TextToDB
    {
        private Database db;

        public TextToDB(){
            this.db = new Database();  
        }

        public void analiseText(string[] text){
            String allText = buildString(text);
            allText = allText.ToLower();

            string[] sentences = allText.Split('.');
            Console.WriteLine(sentences.Length);
            int counter = 1;
            foreach(string sentence in sentences){
                addToGraphDB(sentence);
                Console.WriteLine(counter + "/" + sentences.Length + " - " + 
                                  Math.Round((double)counter/sentences.Length*100, 1) + "%");
                counter++;
            }
        }

        private void addToGraphDB(string sentence){
            sentence = sentence.Trim();
            sentence = Regex.Replace(sentence, "[ ]{1,}", "|");
            string[] words = sentence.Split('|');
            long[] wordsId = new long[words.Length];
   
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = db.getConnection();


            for (int i = 0; i < words.Length; i++){
                db.transactionOpen();
                cmd.CommandText = "SELECT id FROM nodes where name = '" + words[i] + "'";
                reader = cmd.ExecuteReader();

                bool exist = reader.Read();
                if(exist){
                    wordsId[i] = reader.GetInt32(reader.GetOrdinal("id"));
                }
                db.transactionClose();

                if (!exist)
                {
                    db.transactionOpen();
                    cmd.CommandText = "insert into nodes (name) values ('" + words[i] + "')";
                    reader = cmd.ExecuteReader();
                    wordsId[i] = cmd.LastInsertedId;
                    db.transactionClose();
                }
            }

            for (int i = 0; i < wordsId.Length - 1; i++){
                db.transactionOpen();
                cmd.CommandText = "SELECT id FROM edges where a = " + wordsId[i] + " and b = " + wordsId[i+1];
                reader = cmd.ExecuteReader();
                bool exist = reader.Read();
                db.transactionClose();

                if(!exist){
					db.transactionOpen();
					cmd.CommandText = "insert into edges (a, b) values (" + wordsId[i] + ", " + wordsId[i+1] + ")";
					reader = cmd.ExecuteReader();
					wordsId[i] = cmd.LastInsertedId;
					db.transactionClose();
                }
            }
        }

        private string buildString(string[] text){
            StringBuilder builder = new StringBuilder();
            foreach (string val in text)
            {
                builder.Append(val);
            }
            return builder.ToString();
        }
    }
}
