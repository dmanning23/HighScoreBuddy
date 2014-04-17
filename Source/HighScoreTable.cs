using EasyStorage;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using FileBuddyLib;

namespace HighScoreBuddy
{
	/// <summary>
	/// This is a list of all the lists used to store high scores for this game.
	/// </summary>
	public abstract class HighScoreTable : FileBuddy
	{
		#region Member Variables

		/// <summary>
		/// Collection of all the high scores lists
		/// Maps the high score list name to the instance of the high score list.
		/// </summary>
		protected Dictionary<string, HighScoreList> HighScoreLists { get; private set; }

		#endregion //Member Variables

		#region Methods

		/// <summary>
		/// hello standard constructor!
		/// </summary>
		public HighScoreTable(string FolderLocation)
			: base(FolderLocation, "HighScoreTable.xml")
		{
			HighScoreLists = new Dictionary<string, HighScoreList>();
			SaveMethod = WriteHighScores;
			LoadMethod = ReadHighScores;
		}

		/// <summary>
		/// Adds all the lists to the table.
		/// </summary>
		protected abstract void AddLists();

		/// <summary>
		/// called once at the beginning of the program
		/// gets the storage device
		/// </summary>
		/// <param name="myGame">the current game.</param>
		public override void Initialize(Game myGame)
		{
			//initialize the filebuddy stuff
			base.Initialize(myGame);

			//First add the default lists to the table
			AddLists();

			//load the high score table
			Load();
		}

		/// <summary>
		/// Gets the high score list by name
		/// </summary>
		/// <returns>The high score list.</returns>
		/// <param name="ListName">name of teh high score list to fetch.</param>
		public HighScoreList GetHighScoreList(string ListName)
		{
			if (HighScoreLists.ContainsKey(ListName))
			{
				return HighScoreLists[ListName];
			}
			else
			{
				//list does not exist
				return null;
			}
		}

		#endregion //Methods

		#region XML Methods

		/// <summary>
		/// do the actual writing out to disk
		/// </summary>
		/// <param name="myFileStream">My file stream.</param>
		private void WriteHighScores(Stream myFileStream)
		{
			try
			{
				//open the file, create it if it doesnt exist yet
				XmlTextWriter rFile = new XmlTextWriter(myFileStream, null);
				rFile.Formatting = Formatting.Indented;
				rFile.Indentation = 1;
				rFile.IndentChar = '\t';
				
				//save all the high scores!
				rFile.WriteStartDocument();

				//write the high score table element
				rFile.WriteStartElement("highscoretable");

				//write out all the lists
				foreach(KeyValuePair<string, HighScoreList> entry in HighScoreLists)
				{
					// do something with entry.Value or entry.Key
					entry.Value.WriteToXML(rFile);
				}

				rFile.WriteEndElement();

				rFile.WriteEndDocument();
				
				// Close the file.
				rFile.Flush();
				rFile.Close();
			}
			catch (Exception ex)
			{
				// just write some debug output for our verification
				Debug.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// to the actual reading in from disk
		/// </summary>
		/// <param name="myFileStream">My file stream.</param>
		private void ReadHighScores(Stream myFileStream)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(myFileStream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//make sure is correct type of node
				Debug.Assert("highscoretable" == rootNode.Name);

				//read high score lists
				if (rootNode.HasChildNodes)
				{
					XmlNode childNode = rootNode.FirstChild;
					while (childNode != null)
					{
						//get the name of this list
						string ListName = "";
						XmlNamedNodeMap mapAttributes = childNode.Attributes;
						for (int i = 0; i < mapAttributes.Count; i++)
						{
							//will only have the name attribute
							string strName = mapAttributes.Item(i).Name;
							string strValue = mapAttributes.Item(i).Value;
							if ("ListName" == strName)
							{
								ListName = strValue;
							}
							else
							{
								//unknwon attribute in the xml file!!!
								Debug.Assert(false);
							}
						}

						//find the list from the xml file
						Debug.Assert(!String.IsNullOrEmpty(ListName));
						HighScoreList rHighScoreList = HighScoreLists[ListName];
						Debug.Assert(null != rHighScoreList);

						//create, read, and store a new high score list from this xml node
						rHighScoreList.ReadFromXML(childNode);
						HighScoreLists[rHighScoreList.Name] = rHighScoreList;
						
						//next node!
						childNode = childNode.NextSibling;
					}
				}
			}
			else
			{
				//should be an xml node!!!
				Debug.Assert(false);
			}
		}

		#endregion //XML Methods
	}
}