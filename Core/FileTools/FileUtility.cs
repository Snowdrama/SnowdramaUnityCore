using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.IO
{
	public class FileUtility
	{
		/// <summary>
		/// Takes a path and filename and reads in that file and converts to an object T
		/// </summary>
		/// <typeparam name="T">The object to write the data to</typeparam>
		/// <param name="path">The path of the file relative to Application.persistentDataPath</param>
		/// <param name="filename">The name of the json file</param>
		/// <param name="output">true if the file was read and loaded correctly</param>
		/// <returns></returns>
		public static bool LoadObjectFromJSONFile<T>(string path, string filename, ref T output)
        {
			//first load from file 
			string jsonString = "";
			if(LoadStringFromExternalFile(path, filename, ref jsonString))
			{
				//convert to the object
				output = JsonUtility.FromJson<T>(jsonString);
				//return true because we successfully loaded into the ref output
				return true;
            }
			return false;
		}

		/// <summary>
		/// takes a file T and converts it to JSON then writes it to a file
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		/// <param name="input"></param>
		public static void WriteObjectToJSONFile<T>(string path, string filename, T input)
        {
            //convert to json
            string jsonString = JsonUtility.ToJson(input);
			//write to file
            WriteStringToExternalFile(path, filename, jsonString);
		}



		/*
		██╗      ██████╗  █████╗ ██████╗      █████╗ ███████╗    ██████╗ ███████╗███████╗ ██████╗ ██╗   ██╗██████╗  ██████╗███████╗
		██║     ██╔═══██╗██╔══██╗██╔══██╗    ██╔══██╗██╔════╝    ██╔══██╗██╔════╝██╔════╝██╔═══██╗██║   ██║██╔══██╗██╔════╝██╔════╝
		██║     ██║   ██║███████║██║  ██║    ███████║███████╗    ██████╔╝█████╗  ███████╗██║   ██║██║   ██║██████╔╝██║     █████╗
		██║     ██║   ██║██╔══██║██║  ██║    ██╔══██║╚════██║    ██╔══██╗██╔══╝  ╚════██║██║   ██║██║   ██║██╔══██╗██║     ██╔══╝
		███████╗╚██████╔╝██║  ██║██████╔╝    ██║  ██║███████║    ██║  ██║███████╗███████║╚██████╔╝╚██████╔╝██║  ██║╚██████╗███████╗
		╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝     ╚═╝  ╚═╝╚══════╝    ╚═╝  ╚═╝╚══════╝╚══════╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝ ╚═════╝╚══════╝
		*/


		/// <summary>
		/// READ ONLY: Load from a file in the Resources folder
		/// </summary>
		/// <param name="path">Path relative to Resources folder</param>
		/// <param name="filename">The file name</param>
		/// <param name="res">the variable to put the string</param>
		/// <returns>A bool representing successful loading of the file</returns>
		public static bool LoadStringFromResources(string path, string filename, ref string res)
		{
			string filePath = path + "/" + filename.Split('.')[0].Trim();
			TextAsset loadedFile = Resources.Load<TextAsset>(filePath);
			if (loadedFile == null)
			{
				return false;
			}
			res = loadedFile.text;
			return true;
		}

		/*
		██╗      ██████╗  █████╗ ██████╗      █████╗ ███████╗    ███████╗██╗  ██╗████████╗███████╗██████╗ ███╗   ██╗ █████╗ ██╗
		██║     ██╔═══██╗██╔══██╗██╔══██╗    ██╔══██╗██╔════╝    ██╔════╝╚██╗██╔╝╚══██╔══╝██╔════╝██╔══██╗████╗  ██║██╔══██╗██║
		██║     ██║   ██║███████║██║  ██║    ███████║███████╗    █████╗   ╚███╔╝    ██║   █████╗  ██████╔╝██╔██╗ ██║███████║██║
		██║     ██║   ██║██╔══██║██║  ██║    ██╔══██║╚════██║    ██╔══╝   ██╔██╗    ██║   ██╔══╝  ██╔══██╗██║╚██╗██║██╔══██║██║
		███████╗╚██████╔╝██║  ██║██████╔╝    ██║  ██║███████║    ███████╗██╔╝ ██╗   ██║   ███████╗██║  ██║██║ ╚████║██║  ██║███████╗
		╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝     ╚═╝  ╚═╝╚══════╝    ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝

		*/

		/// <summary>
		/// Load a string from a file in the Application.persistentDataPath
		/// </summary>
		/// <param name="path">Path relative to Application.persistentDataPath</param>
		/// <param name="filename">The file name</param>
		/// <param name="res">the variable to put the string</param>
		/// <returns>A bool representing successful loading of the file</returns>
		public static bool LoadStringFromExternalFile(string path, string filename, ref string res)
		{
			string filePath = Application.persistentDataPath + "/" + path + "/" + filename;
			//Debug.LogWarning("Read Data from" + filePath);
			if (!System.IO.File.Exists(filePath))
			{
				Debug.LogError($"Tried reading file {filePath} which did not exist.");
				return false;
			}
			StreamReader reader = new StreamReader(filePath);
			string response = "";
			while (!reader.EndOfStream)
			{
				response += reader.ReadLine();
			}
			res = response;
			reader.Close();

			if (string.IsNullOrEmpty(res))
			{
				Debug.LogError($"File at {filePath} was null or empty");
				return false;
			}
			return true;
		}

		/*
		██╗    ██╗██████╗ ██╗████████╗███████╗     █████╗ ███████╗    ███████╗██╗  ██╗████████╗███████╗██████╗ ███╗   ██╗ █████╗ ██╗
		██║    ██║██╔══██╗██║╚══██╔══╝██╔════╝    ██╔══██╗██╔════╝    ██╔════╝╚██╗██╔╝╚══██╔══╝██╔════╝██╔══██╗████╗  ██║██╔══██╗██║
		██║ █╗ ██║██████╔╝██║   ██║   █████╗      ███████║███████╗    █████╗   ╚███╔╝    ██║   █████╗  ██████╔╝██╔██╗ ██║███████║██║
		██║███╗██║██╔══██╗██║   ██║   ██╔══╝      ██╔══██║╚════██║    ██╔══╝   ██╔██╗    ██║   ██╔══╝  ██╔══██╗██║╚██╗██║██╔══██║██║
		╚███╔███╔╝██║  ██║██║   ██║   ███████╗    ██║  ██║███████║    ███████╗██╔╝ ██╗   ██║   ███████╗██║  ██║██║ ╚████║██║  ██║███████╗
		 ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝   ╚═╝   ╚══════╝    ╚═╝  ╚═╝╚══════╝    ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝

		*/

		/// <summary>
		/// Write a string to a file relative to Application.persistentDataPath
		/// </summary>
		/// <param name="path">Path relative to Application.persistentDataPath</param>
		/// <param name="filename">The file name</param>
		/// <param name="content">The string to write to the file</param>
		/// <returns></returns>
		public static void WriteStringToExternalFile(string path, string filename, string content)
		{
			string directoryPath = Application.persistentDataPath + "/" + path;
			string filePath = directoryPath + "/" + filename;
			//Debug.LogFormat("Writing to path {0}", filePath);
			if (!System.IO.Directory.Exists(directoryPath))
			{
				Debug.LogWarning($"Directory {path} didn't exist, creating.");
				Directory.CreateDirectory(directoryPath);
			}
			FileStream stream = File.Create(filePath);
			byte[] contentBytes = new UTF8Encoding(true).GetBytes(content);
			stream.Write(contentBytes, 0, contentBytes.Length);
			stream.Dispose();
		}

		/*
		██╗      ██████╗  █████╗ ██████╗     ███████╗██████╗  ██████╗ ███╗   ███╗    ██╗   ██╗██████╗ ██╗
		██║     ██╔═══██╗██╔══██╗██╔══██╗    ██╔════╝██╔══██╗██╔═══██╗████╗ ████║    ██║   ██║██╔══██╗██║
		██║     ██║   ██║███████║██║  ██║    █████╗  ██████╔╝██║   ██║██╔████╔██║    ██║   ██║██████╔╝██║
		██║     ██║   ██║██╔══██║██║  ██║    ██╔══╝  ██╔══██╗██║   ██║██║╚██╔╝██║    ██║   ██║██╔══██╗██║
		███████╗╚██████╔╝██║  ██║██████╔╝    ██║     ██║  ██║╚██████╔╝██║ ╚═╝ ██║    ╚██████╔╝██║  ██║███████╗
		╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝     ╚═╝     ╚═╝  ╚═╝ ╚═════╝ ╚═╝     ╚═╝     ╚═════╝ ╚═╝  ╚═╝╚══════╝

		*/


		//public delegate void OnWebLoaded(string responseText);
		////this creates a 
		//public static IEnumerator LoadJsonFromURL(string url, OnWebLoaded callback){
		//	WWW request = new WWW(url);
		//	yield return request;
		//	callback(request.text);
		//}

		////this creates a new file
		//public static IEnumerator PostJsonToURL(string url, string body, OnWebLoaded callback){
		//	Dictionary<string, string> headers = new Dictionary<string, string>();
		//	//put headers here as the api requires
		//	headers["content-type"] = "application/json; charset=utf-8";

		//	WWW request = new WWW(url, Encoding.ASCII.GetBytes(body), headers);
		//	yield return request;
		//	callback(request.text);
		//}

	}

}
