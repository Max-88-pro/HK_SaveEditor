using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Windows.Forms;

namespace HK_SaveEditor {
	class Program {
		static void Main(string[] args) {

			if(args.Length == 0 || args[0].Equals("-h")) {
				var str = "";
				str += "This tool converts a Hollow Knight .dat file into a editable .json file. The tool can then converts the edited .json back to .dat.\n";
				str += "\n\n";
				str += "Usage: \n";
				str += @"1- Drag the .dat file you want to edit on this .exe to generate a .json file with the same name. The .dat files are located in C:\Users\${USER}\AppData\LocalLow\Team Cherry\Hollow Knight." + "\n";
				str += "2- Edit the .json file using any text editor (ex: notepad).\n";
				str += "3- Drag the .json file on this .exe to generate back the .dat file.\n";
				str += "\n\n";
				str += "This tool was made by Raining Chain. Contact me in the Hollow Knight discord channel for info.";
				if(args.Length == 0){
					//MessageBox.Show(str);
                } else
					Console.Write(str);
				return;
			}

			if(args[0].IndexOf(".dat") != -1) {
				dat2json(args[0], args[0].Replace(".dat", ".json"));
			} else if(args[0].IndexOf(".json") != -1) {
				json2dat(args[0], args[0].Replace(".json", ".dat"));
			} else {
				//MessageBox.Show("Error: The file provided is neither a .dat nor a .json file.");
			}
		}
		static void dat2json(string file, string dest) {
			try {
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream fileStream = File.Open(file, FileMode.Open);
				string datStr = (string)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				string jsonStr = StringEncrypt.DecryptData(datStr);
				File.WriteAllText(dest, jsonStr);
			} catch(Exception ex) {
				//MessageBox.Show("An error occured: " + ex.Message);
			}
		}
		static void json2dat(string file, string dest) {
			try {
				string jsonStr = File.ReadAllText(file);
				string datStr = StringEncrypt.EncryptData(jsonStr);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream fileStream = File.Open(dest, FileMode.OpenOrCreate);
				binaryFormatter.Serialize(fileStream, datStr);
				fileStream.Close();
			} catch(Exception ex) {
				//MessageBox.Show("An error occured: " + ex.Message);
			}
		}

	}

	public class StringEncrypt {
		public static string EncryptData(string toEncrypt) {
			byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
			ICryptoTransform cryptoTransform = new RijndaelManaged {
				Key = StringEncrypt.keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		public static string DecryptData(string toDecrypt) {
			byte[] array = Convert.FromBase64String(toDecrypt);
			ICryptoTransform cryptoTransform = new RijndaelManaged {
				Key = StringEncrypt.keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateDecryptor();
			byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes);
		}

		private static byte[] keyArray = Encoding.UTF8.GetBytes("UKu52ePUBwetZ9wNX88o54dnfKRu0T1l");
	}
}
