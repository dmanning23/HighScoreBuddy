using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace HighScoreBuddy
{
	/// <summary>
	/// this is a high score list with 10 entries
	/// </summary>
	public class HighScoreList
	{
		#region Members

		/// <summary>
		/// list of 10 high scores
		/// </summary>
		public List<HighScoreData> Entries { get; private set; }

		/// <summary>
		/// The number of high scores entries to hold in each table
		/// </summary>
		public const int NumEntries = 10;

		/// <summary>
		/// The text to display when someone gets onto this list
		/// </summary>
		private string MessageText { get; set; }

		/// <summary>
		/// the name of this list type
		/// </summary>
		public string Name { get; private set; }

		#endregion //Members

		#region Initialization / Cleanup

		/// <summary>
		/// Initializes a new instance of the <see cref="HighScoreBuddy.HighScoreList"/> class.
		/// onyl used for serialization
		/// </summary>
		public HighScoreList()
		{
			Entries = new List<HighScoreData>();
		}

		/// <summary>
		/// initilize the high score list
		/// </summary>
		/// <param name="eType">the type of this list</param>
		public HighScoreList(string ListName, uint startScore, uint scoreStep, string InitialName, string strMessageText)
		{
			//Grab all these parameters we will need later
			Name = ListName;
			MessageText = strMessageText;

			//initialize the list with some default entries
			Entries = new List<HighScoreData>();
			uint iScore = startScore;
			for (uint i = 0; i < NumEntries; i++)
			{
				Entries.Add(new HighScoreData(InitialName, iScore));
				iScore += scoreStep;
			}

			//ok, reverse the entries because they are added from low -> high
			Entries.Reverse();
		}

		/// <summary>
		/// This method just checks if there are any high scores in a scorecard.  it doesnt actually store them or pop up any message boxes.
		/// </summary>
		/// <returns><c>true</c>, if there are any high scores, <c>false</c> otherwise.</returns>
		/// <param name="rScoreCard">the score card to check.</param>
		public bool CheckForHighScores(uint iScore)
		{
			//Check for a high score
			for (int i = 0; i < NumEntries; i++)
			{
				if (iScore > Entries[i].Score)
				{
					//player got a new high score!
					return true;
				}
			}
			
			//no high scores :(
			return false;
		}

		/// <summary>
		/// Check if this player got a high score, store the high score in the table, popup a message
		/// </summary>
		/// <param name="rScoreCard">the scorecard to check against teh current high scores</param>
		public bool AddHighScores(string strName, uint iScore, List<string> strMessages)
		{
			//Check for a high score
			for (int i = 0; i < NumEntries; i++)
			{
				if (iScore > Entries[i].Score)
				{
					//player got a new high score!
					HighScoreData myHighScore = new HighScoreData(strName, iScore);
					myHighScore.NewHighScore = true;
					
					//add to the list, and remove the last entry
					Entries.Insert(i, myHighScore);
					Entries.RemoveAt(NumEntries);
					
					//popup a high score message
					strMessages.Add(strName + MessageText);
					
					//don't keep checking for high scores
					return true;
				}
			}
			
			return false;
		}

		#endregion //Initialization / Cleanup

		#region XML Methods

		public void ReadFromXML(XmlNode rXMLNode)
		{
			//clear out the list of entries
			Entries.Clear();

			//make sure it is actually an xml node
			if (rXMLNode.NodeType == XmlNodeType.Element)
			{
				//make sure is correct type of node
				Debug.Assert("highscorelist" == rXMLNode.Name);

				//get the name of this dude
				XmlNamedNodeMap mapAttributes = rXMLNode.Attributes;
				for (int i = 0; i < mapAttributes.Count; i++)
				{
					//will only have the name attribute
					string strName = mapAttributes.Item(i).Name;
					string strValue = mapAttributes.Item(i).Value;
					if ("ListName" == strName)
					{
						Name = strValue;
					}
					else
					{
						//unknwon attribute in the xml file!!!
						Debug.Assert(false);
					}
				}

				//read high score entries
				if (rXMLNode.HasChildNodes)
				{
					//only read 10 entries
					XmlNode childNode = rXMLNode.FirstChild;
					while ((childNode != null) && (Entries.Count <= NumEntries))
					{
						//create, read, and store a new high score entry from this xml node
						HighScoreData rMyData = new HighScoreData();
						rMyData.ReadFromXML(childNode);
						Entries.Add(rMyData);

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

		public void WriteToXML(XmlTextWriter rXMLFile)
		{
			//write high score list element
			rXMLFile.WriteStartElement("highscorelist");

			//add attribute for type
			rXMLFile.WriteAttributeString("ListName", Name);

			//write out all the scores
			foreach (HighScoreData iter in Entries)
			{
				iter.WriteToXML(rXMLFile);
			}

			rXMLFile.WriteEndElement();
		}

		#endregion //XML Methods
	}
}