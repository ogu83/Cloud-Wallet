using CloudWallet.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string myKey = "sifrem";
            string myData = "adamKopegiIsirdi";
            string myEncData, myDecData;

            myEncData = Convert.ToBase64String(AES.Encrypt(Convert.FromBase64String(myData), "sifrem"));
            Console.WriteLine("Encrypted : ");
            Console.WriteLine(myEncData.ToString());

            myDecData = Convert.ToBase64String(AES.Decrypt(Convert.FromBase64String(myEncData), "sifrem"));
            Console.WriteLine("Decrypted : ");
            Console.WriteLine(myDecData.ToString());

            Console.ReadLine();
        }
    }
}
