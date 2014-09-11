using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;

//Wrapper around the .net framework's Json serializer.

namespace SolverCore
{
    public class Loader
    {
        public static void LoadFromFile(string FileName, SudokuGrid Grid)
        {
            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(byte[]));
            FileStream fs = new FileStream(FileName, FileMode.Open);
            byte[] Array = (byte[])Serializer.ReadObject(fs);
            fs.Close();
            Grid.LinearArr = Array;
        }

        public static void SaveToFile(string FileName, SudokuGrid Grid)
        {
            byte[] Array = Grid.LinearArr;
            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(byte[]));
            FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate);
            Serializer.WriteObject(fs, Array);
            fs.Close();
        }
    }
}
