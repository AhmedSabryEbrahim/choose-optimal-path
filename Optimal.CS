using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace TestTheAlgo
{
    class Node
    {
        public int ES = -1, LS = 99999999, EF, LF = 99999999, Duration, Slack;
        public bool onCriticalBath;
        public string NodeName;
        public string[] Depndances;
        public int[] DepInt;
        public List<int> BackDep = new List<int>();
        public void copyNode(Node Copied) {
            this.LS = Copied.ES;
            this.EF = Copied.EF;
            this.LF = Copied.EF;
            this.ES = Copied.ES;
        }
        public void ModifyNode()
        {
            DepInt = new int[Depndances.Length];
            for (int i = 0; i < Depndances.Length; i++)
            {
                DepInt[i] = (Depndances[i] != null) ? (int)(Depndances[i][0] - 'A') : -1;
            }
            if (Depndances.Length == 1 && Depndances[0] == null)
            { ES = 0; EF = Duration + ES; }
        }
        public override string ToString()
        {
            string str = NodeName;
           // str += " " + Duration;
            str +=/* " " + ES + " " + EF +*/ " " + LS + " " + LF + " ";
            foreach (int item in BackDep)
            {
             //   if (item != -1)
               //     str += " " + (char)(item + 'A');
            }
            return str;
        }
    };
    public static class StringParsers
    {
        public static T Convert<T>(object v)
        {
            T Tmp = default(T);
            try
            {
                Tmp = (T)(object)v;
            }
            catch (Exception ex) { }
            return Tmp;
        }
    }
    class AlgorithmOnNode
    {
        List<Node> ArrNodes;
        string readFun()
        {
            String line = "";
            try
            {
                using (StreamReader sr = new StreamReader("TestFile.txt"))
                {
                    line = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return line;
        }

        public int recusion(int Elem)
        {
            if (Elem >= ArrNodes.Count || Elem == -1) return 0;
            else
            {
                for (int i = 0; i < ArrNodes[Elem].Depndances.Length; i++)
                {
                    ArrNodes[Elem].ES = Math.Max(ArrNodes[Elem].ES, recusion(ArrNodes[Elem].DepInt[i]));
                    ArrNodes[Elem].EF = ArrNodes[Elem].ES + ArrNodes[Elem].Duration;
                }
            }
            return ArrNodes[Elem].EF;
        }
        public int recusion1(int Elem)
        {
            if (Elem >= ArrNodes.Count || Elem == -1) return 0;
            else
            {
                for (int i = 0; i < ArrNodes[Elem].Depndances.Length; i++)
                {
                    ArrNodes[Elem].ES = Math.Max(ArrNodes[Elem].ES, recusion1(ArrNodes[Elem].DepInt[i]));
                    ArrNodes[Elem].EF = ArrNodes[Elem].ES + ArrNodes[Elem].Duration;
                }
            }
            return ArrNodes[Elem].EF;
        }
        public void Calculate()
        {
            bool[] visited = new bool[ArrNodes.Count];
            for (int i = 0; i < visited.Length; i++) visited[i] = true;
            Node LastNode = new Node(); int pos = 0;
            for (int i = 0; i < ArrNodes.Count; i++)
            {
                if (ArrNodes[i].ES == -1)
                {
                    recusion(i);
                }//Back Dependences 
                for (int j = 0; j < ArrNodes[i].DepInt.Length; j++) {
                    if (ArrNodes[i].DepInt[j] != -1)
                    {
                        ArrNodes[i].BackDep.Add(ArrNodes[i].DepInt[j]);
                    }
                }
                foreach (int item in ArrNodes[i].DepInt)
                {
                    if (item != -1) visited[item] = false;
                }
                if (LastNode.EF < ArrNodes[i].EF)
                {
                    LastNode = ArrNodes[i];
                }
            }
            List<int> ArrList = new List<int>();
            ArrNodes.Add(new Node());
            ArrNodes[ArrNodes.Count-1].copyNode(LastNode);
            for (int i = 0; i < visited.Length; i++)
            {
                if (visited[i] == true)
                {
                    ArrList.Add(i);
                    ArrNodes[i].LF = LastNode.EF;
                    ArrNodes[i].LS = ArrNodes[i].LF - ArrNodes[i].Duration;
                    ArrNodes[ArrNodes.Count - 1].BackDep.Add(i);
                }
            }
            for (int i = ArrNodes.Count-1; i >= 0; i--)
            {
                for (int j = 0; j < ArrNodes[i].BackDep.Count; j++) {
                    if (ArrNodes[ArrNodes[i].BackDep[j]].LF > ArrNodes[i].LS && ArrNodes[i].LS >= ArrNodes[i].Duration)
                    {
                        ArrNodes[ArrNodes[i].BackDep[j]].LF = ArrNodes[i].LS;
                        ArrNodes[ArrNodes[i].BackDep[j]].LS = ArrNodes[ArrNodes[i].BackDep[j]].LF - ArrNodes[ArrNodes[i].BackDep[j]].Duration;
                    }
                }
            }

            ArrNodes[ArrNodes.Count - 1].NodeName = "End";
        }
        public AlgorithmOnNode()
        {
            string str = readFun();
            string[] ArrStr = SplitString<string>(str, '\n');
            ArrNodes = GetInput<string>(ArrStr);
            foreach (Node Item in ArrNodes)
            {
                Item.ModifyNode();
            }
            Calculate();
            foreach (Node Item in ArrNodes)
            {
               Console.WriteLine(Item);
            }
        }
        public T[] SplitString<T>(string str, char delimiterChars)
        {
            string[] Tmp = str.Split(delimiterChars);
            T[] arr = new T[Tmp.Length];
            for (int i = 0; i < Tmp.Length; i++)
            {
                if (str[0] == '-') break;
                arr[i] = StringParsers.Convert<T>(Tmp[i]);
            }
            return arr;
        }
        public static string ExceptChars(string str, IEnumerable<char> toExclude)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!toExclude.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
        public List<Node> GetInput<T>(string[] str)
        {
            int N = str.Length, index;
            string[] TMP;
            List<Node> Nodes = new List<Node>();
            for (int i = 0; i < N; i++)
            {
                index = 0;
                Nodes.Add(new Node());
                TMP = SplitString<string>(str[i], ' ');
                Nodes[i].NodeName = TMP[index++];
                Nodes[i].Depndances = SplitString<string>(TMP[1], ',');
                Nodes[i].Duration = int.Parse(TMP[2]);
            }
            return Nodes;
        }
    }
}
